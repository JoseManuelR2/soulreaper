using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public partial class WandController : MonoBehaviour
{
    public static List<string> Spell = new List<string>();

    private static int FIREINDEX = 0;
    private static int ICEINDEX = 1;
    private static int POISONINDEX = 2;

    private static int YELLOWINDEX = 0;
    private static int PURPLEINDEX = 1;
    private static int BLUEINDEX = 2;

    public Transform firePoint; // EL PUNTO DESDE DONDE SALE EL PROYECTIL (Punta de la varita)
    
    [Header("Referencias de Varitas")]
    public GameObject[] balls;
    public GameObject[] feathers;


    [Header("Hechizos Normales")]
    public GameObject fireProjectilePrefab;
    public GameObject iceProjectilePrefab;
    public GameObject poisonProjectilePrefab;

    [Header("Hechizos Fuertes")]
    public GameObject fireUpProjectilePrefab;
    public GameObject iceUpProjectilePrefab;
    public GameObject poisonUpProjectilePrefab;

    [Header("Hechizos Area")]
    public GameObject fireAOEProjectilePrefab;
    public GameObject iceAOEProjectilePrefab;
    public GameObject poisonAOEProjectilePrefab;

    // Guardamos qué hechizo está cargado actualmente
    public enum ActiveSpell { None, Fire, Ice, Poison, FireUp, IceUp, PoisonUp, FireAOE, IceAOE, PoisonAOE, TP, Heal, Shield, Mana}
    private static ActiveSpell currentActiveSpell = ActiveSpell.None;

    private static int? currentBallIndex = null;
    private static int? currentFeatherIndex = null;

    private static WandController instance;
    public static WandController Instance => instance;

    [Header("Input")]
    public InputActionProperty triggerAction;

    [Header("Audio y Háptica")]
    public AudioSource audioSource;
    public AudioClip runeSelectSound;
    public AudioClip spellReadySound;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource != null) Debug.Log("WandController: AudioSource listo.");
        else Debug.LogWarning("WandController: No se encontró AudioSource en la varita.");
    }

    void OnEnable()
    {
        triggerAction.action.Enable();
        triggerAction.action.performed += OnTriggerPressed;
    }

    void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
        triggerAction.action.Disable();
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        Shoot(); // Llama a la función de disparo
    }

    private void Shoot()
    {
        if (currentActiveSpell == ActiveSpell.None || firePoint == null) return;

        GameObject prefabToShoot = null;

        // Elegir la bala correcta según el estado actual
        switch (currentActiveSpell)
        {
            case ActiveSpell.Fire: prefabToShoot = fireProjectilePrefab; break;
            case ActiveSpell.Ice: prefabToShoot = iceProjectilePrefab; break;
            case ActiveSpell.Poison: prefabToShoot = poisonProjectilePrefab; break;
            case ActiveSpell.FireUp: prefabToShoot = fireUpProjectilePrefab; break;
            case ActiveSpell.IceUp: prefabToShoot = iceUpProjectilePrefab; break;
            case ActiveSpell.PoisonUp: prefabToShoot = poisonUpProjectilePrefab; break;
            case ActiveSpell.FireAOE: prefabToShoot = fireAOEProjectilePrefab; break;
            case ActiveSpell.IceAOE: prefabToShoot = iceAOEProjectilePrefab; break;
            case ActiveSpell.PoisonAOE: prefabToShoot = poisonAOEProjectilePrefab; break;
        }

        if (prefabToShoot != null)
        {
            // Instanciar el proyectil en la posición y rotación de la punta de la varita
            Quaternion rotationToAdd = Quaternion.Euler(-90f, 0, 0);

            Instantiate(prefabToShoot, firePoint.position, firePoint.rotation * rotationToAdd);
        }
    }

    public static void WandTypeSelector(int? ball, int? feather)
    {
        if (instance == null) return;

        if (currentBallIndex.HasValue) instance.balls[currentBallIndex.Value].SetActive(false);
        if (currentFeatherIndex.HasValue) instance.feathers[currentFeatherIndex.Value].SetActive(false);

        if (ball.HasValue)
        {
            instance.balls[ball.Value].SetActive(true);
            currentBallIndex = ball.Value;
        }
        else { currentBallIndex = null; }

        if (feather.HasValue)
        {
            instance.feathers[feather.Value].SetActive(true);
            currentFeatherIndex = feather.Value;
        }
        else { currentFeatherIndex = null; }
    }

    public void AddRune(string colorRune)
    {
        Spell.Add(colorRune);

        if (audioSource != null && runeSelectSound != null)
        {
            // Incrementa un semitono por cada runa añadida
            int runeIndex = Spell.Count - 1;
            audioSource.pitch = Mathf.Pow(1.059463f, runeIndex);
            audioSource.PlayOneShot(runeSelectSound);
        }

        SendHaptic(0.5f, 0.1f); // Vibración media para la selección de runa
    }

    public void SendHaptic(float amplitude, float duration)
    {
        if (triggerAction.action != null && triggerAction.action.controls.Count > 0)
        {
            if (triggerAction.action.controls[0].device is UnityEngine.InputSystem.XR.XRControllerWithRumble xrController)
            {
                xrController.SendImpulse(amplitude, duration);
                return;
            }
        }

        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Controller, devices);
        foreach (var device in devices)
        {
            device.SendHapticImpulse(0u, amplitude, duration);
        }
    }

    public static void ProcessSpell()
    {
        Debug.Log("Procesando spell...");

        Debug.Log("Spell: " + string.Join(" - ", Spell));

        // amarillo + cualquiera de los bases, hechizo potenciado
        // amarillo + purpura + cualquiera de los bases, hechizo en area
        // morado + verde = hechizo de curacion
        // morado + azul = hechizo de escudo
        // morado + rojo = hechizo de teleport
        // rojo + verde + azul = recuperacion/regeneracion de mana
        var key = string.Join(",", Spell);

        switch (key)
        {
            // Hechizos base
            case "red":
                Debug.Log("Hechizo de fuego");
                currentActiveSpell = ActiveSpell.Fire;
                WandTypeSelector(FIREINDEX, null);
                break;

            case "blue":
                Debug.Log("Hechizo de hielo");  
                currentActiveSpell = ActiveSpell.Ice;
                WandTypeSelector(ICEINDEX, null);
                break;

            case "green":
                Debug.Log("Hechizo de veneno");
                currentActiveSpell = ActiveSpell.Poison;
                WandTypeSelector(POISONINDEX, null);
                break;

            // Amarillo + cualquiera de las bases -> Hechizo potenciado
            case "yellow,red":
                Debug.Log("Hechizo potenciado (Fuego)");
                currentActiveSpell = ActiveSpell.FireUp;
                WandTypeSelector(FIREINDEX, YELLOWINDEX);
                break;

            case "yellow,blue":
                Debug.Log("Hechizo potenciado (Hielo)");
                currentActiveSpell = ActiveSpell.IceUp;
                WandTypeSelector(ICEINDEX, YELLOWINDEX);
                break;

            case "yellow,green":
                Debug.Log("Hechizo potenciado (Veneno)");
                currentActiveSpell = ActiveSpell.PoisonUp;
                WandTypeSelector(POISONINDEX, YELLOWINDEX);
                break;

            // Amarillo + morado + cualquiera de las bases -> Hechizo en area potenciado
            case "yellow,purple,red":
                Debug.Log("Hechizo en area (Fuego)");
                currentActiveSpell = ActiveSpell.FireAOE;
                WandTypeSelector(FIREINDEX, BLUEINDEX);
                break;

            case "yellow,purple,blue":
                Debug.Log("Hechizo en area (Hielo)");
                currentActiveSpell = ActiveSpell.IceAOE;
                WandTypeSelector(ICEINDEX, BLUEINDEX);
                break;

            case "yellow,purple,green":
                Debug.Log("Hechizo en area (Veneno)");
                currentActiveSpell = ActiveSpell.PoisonAOE;
                WandTypeSelector(POISONINDEX, BLUEINDEX);
                break;

            // Hechizos especiales
            case "purple,red":
                Debug.Log("Teleport");
                currentActiveSpell = ActiveSpell.TP;
                WandTypeSelector(FIREINDEX, PURPLEINDEX);
                SpellManager.SetTeleport();
                break;

            case "purple,green":
                Debug.Log("Curacion");
                currentActiveSpell = ActiveSpell.Heal;
                WandTypeSelector(POISONINDEX, PURPLEINDEX);
                break;

            case "purple,blue":
                Debug.Log("Escudo");
                currentActiveSpell = ActiveSpell.Shield;
                WandTypeSelector(ICEINDEX, PURPLEINDEX);
                break;

            case "green,red,blue":
                Debug.Log("Recuperacion de mana");
                currentActiveSpell = ActiveSpell.Mana;
                WandTypeSelector(null, null);
                break;

            default:
                Debug.Log("Hechizo invalido");
                currentActiveSpell = ActiveSpell.None;
                break;
        }

        if (currentActiveSpell != ActiveSpell.None)
        {
            if (instance.audioSource != null && instance.spellReadySound != null)
            {
                instance.audioSource.pitch = 1f; // Restauramos el pitch a normal
                instance.audioSource.PlayOneShot(instance.spellReadySound);
            }
            instance.SendHaptic(1.0f, 0.4f); // Vibración fuerte al confirmar hechizo
        }

        Spell.Clear();

        // IMPORTANTE: limpiar después

    }
}