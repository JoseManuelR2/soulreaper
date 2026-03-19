using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Arrastra aquí tus Canvases desde el Inspector de Unity
    public GameObject mainMenuPanel;
    public GameObject levelSelectionPanel;

    // 1. Mostrar selección de niveles
    public void OpenLevelSelection()
    {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
    }

    // 2. Volver al inicio
    public void BackToMain()
    {
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
    }

    // 3. Cargar Niveles (Asegúrate de que el nombre de la escena coincida)
    public void LoadLevel1() { SceneManager.LoadScene("Prueba_Optimizacion"); }
    public void LoadLevel2() { SceneManager.LoadScene("TheGraveyard"); }
    public void LoadLevel3() { SceneManager.LoadScene("MefistoCastle"); }

    // 4. Salir
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed"); // Solo para ver que funciona en el editor
    }
}