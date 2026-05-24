using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelSelector : MonoBehaviour
{
    [Header("Configuración de Puerta y Niveles")]
    [Tooltip("Puedes arrastrar la puerta aquí si está en la misma escena, o dejarlo vacío para que la busque sola.")]
    public GameObject doorObj;

    [Tooltip("Escribe el nombre de la escena del nivel 1")]
    public string level1Scene;
    [Tooltip("Escribe el nombre de la escena del nivel 2")]
    public string level2Scene;
    [Tooltip("Escribe el nombre de la escena del nivel 3")]
    public string level3Scene;
    
    private bool isLoading = false;
    private string currentSceneLoaded = "";

    [Header("Objetos Interactuables (Calaveras)")]
    public Transform level1Object; 
    public Transform level2Object; 
    public Transform level3Object; 

    private Vector3 lvl1StartPos;
    private Quaternion lvl1StartRot;
    private Vector3 lvl2StartPos;
    private Quaternion lvl2StartRot;
    private Vector3 lvl3StartPos;
    private Quaternion lvl3StartRot;

    public static bool level1Completed = false;
    public static bool level2Completed = false;
    private static int easterEggCounter = 0;

    [Header("Lighting Settings")]
    public Light directionalLight;

    [Range(0f, 8f)] public float world1Intensity = 1f;
    [Range(1000f, 20000f)] public float world1Temperature = 6500f;

    [Range(0f, 8f)] public float world2Intensity = 0.7f;
    [Range(1000f, 20000f)] public float world2Temperature = 3500f;

    [Range(0f, 8f)] public float world3Intensity = 0.4f;
    [Range(1000f, 20000f)] public float world3Temperature = 13000f;

    [Header("Light Transition")]
    public float lightTransitionDuration = 2f;

    private Coroutine lightTransitionCoroutine;

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
        ApplyWorldLighting(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLoading) return;
        
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
            bool isLocked = false;

            if (interactableAncestor.name == "level1")
                sceneToLoad = level1Scene;
            else if (interactableAncestor.name == "level2")
            {
                if (level1Completed || easterEggCounter >= 10) sceneToLoad = level2Scene;
                else isLocked = true;
            }
            else if (interactableAncestor.name == "level3")
            {
                if (level2Completed || easterEggCounter >= 10) sceneToLoad = level3Scene;
                else isLocked = true;
            }

            if (isLocked)
            {
                easterEggCounter++;
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.wrongSpellSound);
                if (WandController.Instance != null) WandController.Instance.SendHaptic(0.8f, 0.25f);
                return;
            }

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                if (WandController.Instance != null) WandController.Instance.SendHaptic(0.5f, 0.2f);

                TutorialMob mob = Object.FindFirstObjectByType<TutorialMob>();
                if (mob != null) mob.DespawnMob();

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

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayNarrative(AudioManager.Instance.selectLevelNarrative);
                }

                GameObject startTriggerObj = GameObject.Find("DoorTrigger_Start");
                if (startTriggerObj != null)
                {
                    Collider startCollider = startTriggerObj.GetComponent<Collider>();
                    if (startCollider != null)
                    {
                        startCollider.enabled = true; 
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
        if (sceneName == level1Scene)
        {
            ApplyWorldLighting(1);
        }
        else if (sceneName == level2Scene)
        {
            ApplyWorldLighting(2);
        }
        else if (sceneName == level3Scene)
        {
            ApplyWorldLighting(3);
        }
        else
        {
            ApplyWorldLighting(1);
        }
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
        ApplyWorldLighting(1);
        Debug.Log("Nivel descargado");
    }

    public void GoToLobby()
    {
        StartCoroutine(GoToLobbyRoutine());
    }

    private IEnumerator GoToLobbyRoutine()
    {
        if (!string.IsNullOrEmpty(currentSceneLoaded))
        {
            yield return SceneManager.UnloadSceneAsync(currentSceneLoaded);
            currentSceneLoaded = "";
            ApplyWorldLighting(1);
        }

        ResetInteractables();

        ResetPlayerToLobby();
    }
    private void ResetPlayerToLobby()
    {
        GameObject xrRig = GameObject.FindGameObjectWithTag("Player");

        if (xrRig != null)
        {
            xrRig.transform.position = Vector3.zero;
            xrRig.transform.rotation = Quaternion.identity;
        }
    }

    private void ApplyWorldLighting(int world)
    {
        if (directionalLight == null) return;

        directionalLight.useColorTemperature = true;

        float targetIntensity = world1Intensity;
        float targetTemperature = world1Temperature;

        switch (world)
        {
            case 1:
                targetIntensity = world1Intensity;
                targetTemperature = world1Temperature;
                break;

            case 2:
                targetIntensity = world2Intensity;
                targetTemperature = world2Temperature;
                break;

            case 3:
                targetIntensity = world3Intensity;
                targetTemperature = world3Temperature;
                break;
        }

        // Si ya había una transición, la cancelamos
        if (lightTransitionCoroutine != null)
        {
            StopCoroutine(lightTransitionCoroutine);
        }

        lightTransitionCoroutine = StartCoroutine(
            SmoothLightTransition(targetIntensity, targetTemperature)
        );
    }
    private IEnumerator SmoothLightTransition(float targetIntensity, float targetTemperature)
    {
        float startIntensity = directionalLight.intensity;
        float startTemperature = directionalLight.colorTemperature;

        float elapsed = 0f;

        while (elapsed < lightTransitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / lightTransitionDuration;

            // SmoothStep hace el movimiento más natural
            t = Mathf.SmoothStep(0f, 1f, t);

            directionalLight.intensity = Mathf.Lerp(
                startIntensity,
                targetIntensity,
                t
            );

            directionalLight.colorTemperature = Mathf.Lerp(
                startTemperature,
                targetTemperature,
                t
            );

            yield return null;
        }

        // Asegurar valores finales exactos
        directionalLight.intensity = targetIntensity;
        directionalLight.colorTemperature = targetTemperature;

        lightTransitionCoroutine = null;
    }
}