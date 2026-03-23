using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    public Object level1Scene;
    public Object level2Scene;
    private bool isLoading = false;

    private string currentSceneLoaded = "";

    private void OnTriggerEnter(Collider other)
    {
        if (isLoading) return; // 👈 BLOQUEO

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

        isLoading = true; // 👈 EMPIEZA BLOQUEO

        // Descargar escena anterior
        if (!string.IsNullOrEmpty(currentSceneLoaded))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneLoaded);
            yield return unloadOp;
        }

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
        {
            Debug.Log("Cargando: " + (loadOp.progress * 100f) + "%");
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        loadOp.allowSceneActivation = true;

        yield return null;

        GameObject door = GameObject.Find("Door_02_reinforced");

        if (door != null)
        {
            StartCoroutine(RotateDoor(door.transform, 3.3f, 100f));
        }

        currentSceneLoaded = sceneName;

        Debug.Log("Escena cargada: " + sceneName);

        isLoading = false; // 👈 DESBLOQUEO
    }

    IEnumerator RotateDoor(Transform door, float duration, float angle)
    {
        Quaternion startRotation = door.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, angle, 0);

        float time = 0f;

        while (time < duration)
        {
            door.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        door.rotation = endRotation;
    }
}