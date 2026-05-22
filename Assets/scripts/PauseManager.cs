using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused = false;

    [Header("UI")]
    public GameObject pauseMenu;

    [Header("References")]
    public LevelSelector levelSelector;
    public Transform xrOrigin;

    void Start()
    {
        pauseMenu.SetActive(false);

        if (levelSelector == null)
            levelSelector = FindFirstObjectByType<LevelSelector>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        IsPaused = true;

        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        IsPaused = false;

        pauseMenu.SetActive(false);

        Time.timeScale = 1f;
    }

    public void GoToLobby()
    {
        Resume(); // importante: limpia pausa primero

        if (levelSelector != null)
        {
            levelSelector.GoToLobby();
        }
        else
        {
            Debug.LogError("No LevelSelector found!");
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}