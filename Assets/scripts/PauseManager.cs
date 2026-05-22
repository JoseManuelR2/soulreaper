using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool IsPaused = false;

    [Header("Input")]
    public InputActionProperty pauseAction;

    private GameObject pauseMenu;

    // locomotion references (auto-found)
    private ContinuousMoveProvider moveProvider;
    private TeleportationProvider teleportProvider;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Transform menuTransform = transform.Find("PauseMenu");

        if (menuTransform != null)
        {
            pauseMenu = menuTransform.gameObject;

            menuTransform.Find("ResumeButton")
                ?.GetComponent<Button>()
                ?.onClick.AddListener(Resume);

            menuTransform.Find("LobbyButton")
                ?.GetComponent<Button>()
                ?.onClick.AddListener(GoToLobby);

            menuTransform.Find("ExitButton")
                ?.GetComponent<Button>()
                ?.onClick.AddListener(QuitGame);
        }
    }

    private void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        // auto-find locomotion in scene
        moveProvider = FindFirstObjectByType<ContinuousMoveProvider>();
        teleportProvider = FindFirstObjectByType<TeleportationProvider>();
    }

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
        pauseAction.action.Disable();
    }
    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (GameOverManager.Instance != null)
        {
            Transform gameOverMenu =
                GameOverManager.Instance.transform.Find("GameOverMenu");

            if (gameOverMenu != null && gameOverMenu.gameObject.activeSelf)
            {
                Debug.Log("PAUSE BLOQUEADO: GAME OVER ACTIVO");
                return;
            }
        }

        if (IsPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        IsPaused = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
            PositionMenuInFrontOfCamera();
        }

        SetLocomotionEnabled(false);

        Debug.Log("PAUSE ENABLED");
    }

    public void Resume()
    {
        IsPaused = false;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        SetLocomotionEnabled(true);

        Debug.Log("PAUSE DISABLED");
    }

    // CORE LOGIC
    private void SetLocomotionEnabled(bool enabled)
    {
        if (moveProvider != null)
            moveProvider.enabled = enabled;

        if (teleportProvider != null)
            teleportProvider.enabled = enabled;
    }

    private void PositionMenuInFrontOfCamera()
    {
        if (pauseMenu == null || Camera.main == null) return;

        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        forward.y = 0;
        if (forward == Vector3.zero) forward = cam.forward;
        forward.Normalize();

        pauseMenu.transform.position = cam.position + forward * 1.5f;
        pauseMenu.transform.rotation = Quaternion.LookRotation(forward);
    }

    public void GoToLobby()
    {
        Resume();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLoopMusic(AudioManager.Instance.lobbyLoop);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}