using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    [Header("Configuración de Puerta y Niveles")]
    [Tooltip("Puedes arrastrar la puerta aquí si está en la misma escena, o dejarlo vacío para que la busque sola.")]
    public GameObject doorObj;

    public Object level1Scene;
    public Object level2Scene;
    public Object level3Scene;
    private bool isLoading = false;

    private string currentSceneLoaded = "";

    [Header("Objetos Interactuables (Calaveras)")]
    public Transform level1Object; // Arrastra aquí la calavera del nivel 1

    public Transform level2Object; // Arrastra aquí la calavera del nivel 2

    public Transform level3Object; // Arrastra aquí la calavera del nivel 3


    private Vector3 lvl1StartPos;
    private Quaternion lvl1StartRot;


    private Vector3 lvl2StartPos;
    private Quaternion lvl2StartRot;

    private Vector3 lvl3StartPos;
    private Quaternion lvl3StartRot;

    private void Start()
    {
        if (level1Object != null)
        {
            lvl1StartPos = level1Object.position;
            lvl1StartRot = level1Object.rotation;
        }
        
        if (level2Object != null)
        {
            lvl2StartPos = level2Object.position;
            lvl2StartRot = level2Object.rotation;
        }

        if (level3Object != null)
        {
            lvl3StartPos = level3Object.position;
            lvl3StartRot = level3Object.rotation;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;
        // Buscar el ancestro más cercano con la tag "Interactable" para mayor robustez
        Transform t = other.transform;
        Transform interactableAncestor = null;
        while (t != null)
        {
            if (t.CompareTag("Interactable"))
            {
                interactableAncestor = t;
                break;
            }
            t = t.parent;
        }

        if (interactableAncestor != null)
        {
            string sceneToLoad = "";

            if (interactableAncestor.name == "level1")
                sceneToLoad = level1Scene != null ? level1Scene.name : "";
            else if (interactableAncestor.name == "level2")
                sceneToLoad = level2Scene != null ? level2Scene.name : "";
            else if (interactableAncestor.name == "level3")
                sceneToLoad = level3Scene != null ? level3Scene.name : "";

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

        isLoading = true; 

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

        while (!loadOp.isDone)
        {
            yield return null;
        }

        yield return null;
        yield return null;

        if (doorObj == null)
        {
            doorObj = GameObject.Find("Door_02_reinforced");
        }

        if (doorObj != null)
        {
            DoorController door = doorObj.GetComponent<DoorController>();
            if (door != null) 
            {
                door.OpenDoor();

                // Buscamos el trigger de entrada y lo encendemos
                GameObject startTriggerObj = GameObject.Find("DoorTrigger_Start");
                if (startTriggerObj != null)
                {
                    Collider startCollider = startTriggerObj.GetComponent<Collider>();
                    if (startCollider != null)
                    {
                        startCollider.enabled = true; // ¡Listo para que el jugador cruce!
                    }
                }
                else
                {
                    Debug.LogWarning("LevelSelector: No encontré 'DoorTrigger_Start' para activarlo.");
                }
            }
            else
            {
                Debug.LogWarning("El objeto 'Door_02_reinforced' no tiene el script DoorController puesto.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto de la puerta");
        }

        currentSceneLoaded = sceneName;

        Debug.Log("Escena cargada: " + sceneName);

        isLoading = false;
    }


    public void ResetInteractables()
    {
        ForceResetObject(level1Object, lvl1StartPos, lvl1StartRot);
        ForceResetObject(level2Object, lvl2StartPos, lvl2StartRot);
        ForceResetObject(level3Object, lvl3StartPos, lvl3StartRot);
        Debug.Log("Objetos del lobby devueltos a su posición original.");
    }

    private void ForceResetObject(Transform obj, Vector3 pos, Quaternion rot)
    {
        if (obj == null) return;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; 
            
            obj.position = pos;
            obj.rotation = rot;
            
            // usar las propiedades correctas de Rigidbody para esta versión
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            rb.isKinematic = false;
        }
        else
        {
            obj.position = pos;
            obj.rotation = rot;
        }
    }

    public string GetCurrentSceneName()
    {
        return currentSceneLoaded;
    }

    public void UnloadCurrentLevel()
    {
        if (!string.IsNullOrEmpty(currentSceneLoaded))
        {
            StartCoroutine(UnloadRoutine());
        }
    }

    private IEnumerator UnloadRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneLoaded);
        yield return unloadOp;

        currentSceneLoaded = ""; 
        Debug.Log("Nivel descargado");
    }
}