using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }
    
    public static bool isRetry = false;
    
    [Header("Referencias UI")]
    [Tooltip("Arrastra aquí el objeto RedFadeScreen que cuelga de la Main Camera")]
    public GameObject redFadeScreenObj;

    private Image redFadeImage;
    private GameObject gameOverMenu;

    public GameObject leftLineVisual;
    public GameObject rightLineVisual;

    [Header("Configuración de Animación")]
    [Tooltip("Duración en segundos del fundido a rojo.")]
    [SerializeField] private float fadeDuration = 2.5f;

    private void SetLineVisualsEnabled(bool enabled)
    {
        if (leftLineVisual != null) leftLineVisual.SetActive(enabled);
        if (rightLineVisual != null) rightLineVisual.SetActive(enabled);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (redFadeScreenObj != null)
        {
            redFadeImage = redFadeScreenObj.GetComponent<Image>();
        }
        else Debug.LogWarning("GameOverManager: No se ha asignado 'RedFadeScreenObj' en el inspector.");

        Transform menuTransform = transform.Find("GameOverMenu");
        if (menuTransform != null)
        {
            gameOverMenu = menuTransform.gameObject;

            Transform retryTransform = menuTransform.Find("RetryButton");
            if (retryTransform != null) retryTransform.GetComponent<Button>().onClick.AddListener(RetryGame);
            else Debug.LogWarning("GameOverManager: No se encontró 'RetryButton' dentro del menú.");

            Transform exitTransform = menuTransform.Find("ExitButton");
            if (exitTransform != null) exitTransform.GetComponent<Button>().onClick.AddListener(QuitGame);
            else Debug.LogWarning("GameOverManager: No se encontró 'ExitButton' dentro del menú.");
        }
        else Debug.LogWarning("GameOverManager: No se encontró el objeto vacío 'GameOverMenu' como hijo.");
    }

    private void Start()
    {
        if (redFadeScreenObj != null) redFadeScreenObj.SetActive(false);

        if (redFadeImage != null)
        {
            Color c = redFadeImage.color;
            c.a = 0f;
            redFadeImage.color = c;
            redFadeImage.raycastTarget = false;
        }

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }
    }

    public void TriggerGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        WaveManager waveManager = UnityEngine.Object.FindFirstObjectByType<WaveManager>();
        if (waveManager != null) waveManager.StopWaves();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverSound);
        }

        EnemyController[] enemies = UnityEngine.Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        if (redFadeScreenObj != null) redFadeScreenObj.SetActive(true);

        if (redFadeImage != null)
        {
            redFadeImage.raycastTarget = false; 
            Color c = redFadeImage.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 0.50f, elapsedTime / fadeDuration);
                redFadeImage.color = c;
                yield return null;
            }

            c.a = 0.75f; 
            redFadeImage.color = c;
        }

        if (gameOverMenu != null)
        {
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                Vector3 forwardDirection = camTransform.forward;
                forwardDirection.y = 0;
                if (forwardDirection == Vector3.zero) forwardDirection = camTransform.up;
                forwardDirection.Normalize();

                gameOverMenu.transform.position = camTransform.position + forwardDirection * 1.5f;
                gameOverMenu.transform.rotation = Quaternion.LookRotation(forwardDirection);
            }

            gameOverMenu.SetActive(true);
            SetLineVisualsEnabled(true);

        }
    }

    public void RetryGame()
    {
        isRetry = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLoopMusic(AudioManager.Instance.lobbyLoop);
        SetLineVisualsEnabled(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}