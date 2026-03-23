using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Object level1Scene;
    public Object level2Scene;

    private bool hasPreloaded = false;

    void Start()
    {
        if (!hasPreloaded)
        {
            StartCoroutine(PreloadScenes());
            hasPreloaded = true;
        }
    }

    IEnumerator PreloadScenes()
    {
        Debug.Log("Precargando escenas...");

        yield return StartCoroutine(PreloadScene(level1Scene.name));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(PreloadScene(level2Scene.name));

        // Limpieza (muy importante)
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();

        Debug.Log("Precarga completa");
    }

    IEnumerator PreloadScene(string sceneName)
    {
        Debug.Log("Precargando: " + sceneName);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!loadOp.isDone)
        {
            yield return null;
        }

        yield return null;

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);

        while (!unloadOp.isDone)
        {
            yield return null;
        }
    }
}