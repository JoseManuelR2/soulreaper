using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }
    
    public static bool isRetry = false;

    private Image redFadeImage;
    private GameObject gameOverMenu;

    [Header("Configuración de Animación")]
    [Tooltip("Duración en segundos del fundido a rojo.")]
    [SerializeField] private float fadeDuration = 2.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Transform redFadeTransform = transform.Find("RedFadeScreen");
        if (redFadeTransform != null)
        {
            redFadeImage = redFadeTransform.GetComponent<Image>();
        }
        else Debug.LogWarning("GameOverManager: No se encontró 'RedFadeScreen' como hijo.");

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
        if (redFadeImage != null)
        {
            redFadeImage.raycastTarget = true; 
            Color c = redFadeImage.color;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 0.75f, elapsedTime / fadeDuration);
                redFadeImage.color = c;
                yield return null;
            }

            c.a = 0.75f; 
            redFadeImage.color = c;
        }

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
        }
    }

    public void RetryGame()
    {
        isRetry = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}