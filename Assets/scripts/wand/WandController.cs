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

    public Transform firePoint;
    
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

    [Header("Costes de Maná")]
    public float baseSpellCost = 0f;
    public float upSpellCost = 25f;
    public float aoeSpellCost = 25f;
    public float specialSpellCost = 30f;
    public float manaSpellCost = 10f;

    public enum ActiveSpell { None, Fire, Ice, Poison, FireUp, IceUp, PoisonUp, FireAOE, IceAOE, PoisonAOE, TP, Heal, Shield, Mana}
    private static ActiveSpell currentActiveSpell = ActiveSpell.None;

    [Header("Configuración de Disparo")]
    public float spellCooldown = 1.0f;
    private float nextSpellTime = 0f;

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

    [Header("Sonidos de Hechizos (Casteo Compartido)")]
    public AudioClip castFireSound;
    public AudioClip castIceSound;
    public AudioClip castPoisonSound;
    public AudioClip castHealSound;
    public AudioClip castManaSound;
    public AudioClip castShieldSound;
    public AudioClip castBuffSound;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this) { Destroy(gameObject); }

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource != null) Debug.Log("WandController: AudioSource listo.");
        else Debug.LogWarning("WandController: No se encontró AudioSource en la varita.");
    }

    void Start()
    {
        if (audioSource != null && AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
        {
            audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
        }

        SendHaptic(0.6f, 0.15f);
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
        Shoot();
    }

    private void Shoot()
    {
        if (currentActiveSpell == ActiveSpell.None || firePoint == null) return;

        if (Time.time < nextSpellTime)
        {
            return;
        }

        float manaCost = 0f;
        switch (currentActiveSpell)
        {
            case ActiveSpell.Fire:
            case ActiveSpell.Ice:
            case ActiveSpell.Poison:
                manaCost = baseSpellCost; break;
            case ActiveSpell.FireUp:
            case ActiveSpell.IceUp:
            case ActiveSpell.PoisonUp:
                manaCost = upSpellCost; break;
            case ActiveSpell.FireAOE:
            case ActiveSpell.IceAOE:
            case ActiveSpell.PoisonAOE:
                manaCost = aoeSpellCost; break;
            case ActiveSpell.TP:
            case ActiveSpell.Heal:
            case ActiveSpell.Shield:
                manaCost = specialSpellCost; break;
            case ActiveSpell.Mana:
                manaCost = manaSpellCost; break;
        }

        if (PlayerController.Instance != null && !PlayerController.Instance.ConsumeMana(manaCost))
        {
            Debug.Log("No hay suficiente maná para lanzar el hechizo.");
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.lowManaSound);
            return;
        }

        nextSpellTime = Time.time + spellCooldown;

        if (currentActiveSpell == ActiveSpell.Mana)
        {
            if (PlayerController.Instance != null)
                PlayerController.Instance.StartManaRecovery();
            
            if (audioSource != null && castManaSound != null) audioSource.PlayOneShot(castManaSound);
            return; 
        }

        if (currentActiveSpell == ActiveSpell.Heal)
        {
            if (PlayerController.Instance != null)
                PlayerController.Instance.StartHealthRecovery();

            if (audioSource != null && castHealSound != null) audioSource.PlayOneShot(castHealSound);
            return;
        }

        if (currentActiveSpell == ActiveSpell.Shield)
        {
            if (PlayerController.Instance != null)
                PlayerController.Instance.ActivateShield();

            if (audioSource != null && castShieldSound != null) audioSource.PlayOneShot(castShieldSound);
            return;
        }

        GameObject prefabToShoot = null;
        AudioClip castClip = null;

        switch (currentActiveSpell)
        {
            case ActiveSpell.Fire: prefabToShoot = fireProjectilePrefab; castClip = castFireSound; break;
            case ActiveSpell.Ice: prefabToShoot = iceProjectilePrefab; castClip = castIceSound; break;
            case ActiveSpell.Poison: prefabToShoot = poisonProjectilePrefab; castClip = castPoisonSound; break;
            case ActiveSpell.FireUp: prefabToShoot = fireUpProjectilePrefab; castClip = castFireSound; break;
            case ActiveSpell.IceUp: prefabToShoot = iceUpProjectilePrefab; castClip = castIceSound; break;
            case ActiveSpell.PoisonUp: prefabToShoot = poisonUpProjectilePrefab; castClip = castPoisonSound; break;
            case ActiveSpell.FireAOE: prefabToShoot = fireAOEProjectilePrefab; castClip = castFireSound; break;
            case ActiveSpell.IceAOE: prefabToShoot = iceAOEProjectilePrefab; castClip = castIceSound; break;
            case ActiveSpell.PoisonAOE: prefabToShoot = poisonAOEProjectilePrefab; castClip = castPoisonSound; break;
            case ActiveSpell.TP: castClip = castBuffSound; break;
        }

        if (prefabToShoot != null)
        {
            Quaternion rotationToAdd = Quaternion.Euler(-90f, 0, 0);

            Instantiate(prefabToShoot, firePoint.position, firePoint.rotation * rotationToAdd);
        }

        if (castClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(castClip);
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
            int runeIndex = Spell.Count - 1;
            audioSource.pitch = Mathf.Pow(1.059463f, runeIndex);
            audioSource.PlayOneShot(runeSelectSound);
        }

        SendHaptic(0.75f, 0.15f);
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

        if (Spell.Count == 0)
        {
            Debug.Log("No se trazó ningún hechizo. Manteniendo el anterior.");
            return;
        }

        if (PlayerController.Instance != null) PlayerController.Instance.CancelRegenerations();

        var key = string.Join(",", Spell);
        bool isValidSpell = true;

        switch (key)
        {
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
                Debug.Log("Hechizo invalido. Manteniendo el anterior.");
                isValidSpell = false;
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.wrongSpellSound);
                instance.SendHaptic(0.8f, 0.25f);
                break;
        }

        if (isValidSpell && currentActiveSpell != ActiveSpell.None)
        {
            if (instance.audioSource != null && instance.spellReadySound != null)
            {
                instance.audioSource.pitch = 1f;
                instance.audioSource.PlayOneShot(instance.spellReadySound);
            }
            instance.SendHaptic(1.0f, 0.6f);
        }

        Spell.Clear();


    }
}