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

    [Header("Configuración de Animación")]
    [Tooltip("Duración en segundos del fundido a rojo.")]
    [SerializeField] private float fadeDuration = 2.5f;

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
        if (redFadeScreenObj != null) redFadeScreenObj.SetActive(false); // Lo mantenemos apagado al iniciar

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
        // Detener las oleadas inmediatamente
        WaveManager waveManager = UnityEngine.Object.FindFirstObjectByType<WaveManager>();
        if (waveManager != null) waveManager.StopWaves();

        // Destruir a todos los enemigos que estén vivos en el mapa
        EnemyController[] enemies = UnityEngine.Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        if (redFadeScreenObj != null) redFadeScreenObj.SetActive(true); // Lo encendemos justo al morir

        if (redFadeImage != null)
        {
            // Lo dejamos en false para que el láser VR pueda traspasarlo y pulsar los botones
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
            // Colocamos el menú delante del jugador antes de activarlo
            if (Camera.main != null)
            {
                Transform camTransform = Camera.main.transform;
                Vector3 forwardDirection = camTransform.forward;
                forwardDirection.y = 0; // Ignoramos si el jugador está mirando arriba o abajo
                if (forwardDirection == Vector3.zero) forwardDirection = camTransform.up;
                forwardDirection.Normalize();

                // Posicionamos a 1.5 metros y a la altura exacta de la cámara
                gameOverMenu.transform.position = camTransform.position + forwardDirection * 1.5f;
                // Lo rotamos para que encare la misma dirección que el jugador
                gameOverMenu.transform.rotation = Quaternion.LookRotation(forwardDirection);
            }

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