using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    public Object level1Scene;
    public Object level2Scene;

    private string currentSceneLoaded = "";

    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.parent;

        if (parent != null && parent.CompareTag("Interactable"))
        {
            string sceneToLoad = "";

            if (parent.name == "level1")
                sceneToLoad = level1Scene.name;
            else if (parent.name == "level2")
                sceneToLoad = level2Scene.name;

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                StartCoroutine(LoadLevelAsync(sceneToLoad));
            }
        }
    }

    IEnumerator LoadLevelAsync(string sceneName)
    {
        if (currentSceneLoaded == sceneName)
            yield break;

        // Descargar escena anterior
        if (!string.IsNullOrEmpty(currentSceneLoaded))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneLoaded);
            yield return unloadOp;
        }

        // Cargar nueva escena SIN activarla aún
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        // Carga progresiva
        while (loadOp.progress < 0.9f)
        {
            Debug.Log("Cargando: " + (loadOp.progress * 100f) + "%");
            yield return null;
        }

        // Aquí puedes meter un fade o esperar
        yield return new WaitForSeconds(0.2f);

        // Activar escena
        loadOp.allowSceneActivation = true;

        currentSceneLoaded = sceneName;

        Debug.Log("Escena cargada: " + sceneName);
    }
}