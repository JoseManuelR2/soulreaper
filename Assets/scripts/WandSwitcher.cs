using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class WandSwitcher : MonoBehaviour
{
    public static List<string> Spell = new List<string>();

    private static int FIREINDEX = 0;
    private static int ICEINDEX = 1;
    private static int POISONINDEX = 2;

    private static int YELLOWINDEX = 0;
    private static int PURPLEINDEX = 1;
    private static int BLUEINDEX = 2;
    
    [Header("Referencias de Varitas")]
    public GameObject[] balls; 
    public GameObject[] feathers;
    
    private static int? currentBallIndex = null;
    private static int? currentFeatherIndex = null; // índice relativo de pluma (0-2)
    private static bool activatedFeather = false;
    
    private static WandSwitcher instance; // Singleton reference

    [Header("Input")]
    public InputActionProperty triggerAction; // Acción del gatillo

    void Awake()
    {
        // Initialize singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
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
        /* RotateWand(); */
    }

    public static void WandTypeSelector(int? ball,  int? feather)
    {
        if (instance == null)
        {
            Debug.LogError("WandSwitcher instance not found!");
            return;
        }

        if (currentBallIndex.HasValue)
        {
            instance.balls[currentBallIndex.Value].SetActive(false);
        }

        if (currentFeatherIndex.HasValue)
        {
            instance.feathers[currentFeatherIndex.Value].SetActive(false);
        }


        if (ball.HasValue)
        {
            instance.balls[ball.Value].SetActive(true);
            currentBallIndex = ball.Value;
        } else
        {
            currentBallIndex = null;
        }

        if (feather.HasValue)
        {
            instance.feathers[feather.Value].SetActive(true);
            currentFeatherIndex = feather.Value;
        } else
        {
            currentFeatherIndex = null;
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
            WandTypeSelector(FIREINDEX, null);
            break;

            case "blue":
            Debug.Log("Hechizo de hielo");
            WandTypeSelector(ICEINDEX, null);
            break;

            case "green":
            Debug.Log("Hechizo de veneno");
            WandTypeSelector(POISONINDEX, null);
            break;

            // Amarillo + cualquiera de las bases -> Hechizo potenciado
            case "yellow,red":
            Debug.Log("Hechizo potenciado");
            WandTypeSelector(FIREINDEX, YELLOWINDEX);
            break;

            case "yellow,blue":
            Debug.Log("Hechizo potenciado");
            WandTypeSelector(ICEINDEX, YELLOWINDEX);
            break;
            
            case "yellow,green":
            Debug.Log("Hechizo potenciado");
            WandTypeSelector(POISONINDEX, YELLOWINDEX);
            break;

            // Amarillo + morado + cualquiera de las bases -> Hechizo en area potenciado
            case "yellow,purple,red":
            Debug.Log("Hechizo en area potenciado");
            WandTypeSelector(FIREINDEX, BLUEINDEX);
            break;

            case "yellow,purple,blue":
            Debug.Log("Hechizo en area potenciado");
            WandTypeSelector(ICEINDEX, BLUEINDEX);
            break;
            
            case "yellow,purple,green":
            Debug.Log("Hechizo en area potenciado");
            WandTypeSelector(POISONINDEX, BLUEINDEX);
            break;

            // Hechizos especiales
            case "purple,red":
            WandTypeSelector(FIREINDEX, PURPLEINDEX);
            SpellManager.SetTeleport();  // 👈 Llama al teleport automáticamente
            break;

            case "purple,green":
            Debug.Log("Curacion");
            WandTypeSelector(POISONINDEX, PURPLEINDEX);
            break;

            case "purple,blue":
            Debug.Log("Escudo");
            WandTypeSelector(ICEINDEX, PURPLEINDEX);
            break;

            case "green,red,blue":
            Debug.Log("Recuperacion de mana");
            WandTypeSelector(null, null);
            break;
            
            
            default:
            Debug.Log("Hechizo invalido");
            break;
        }

        Spell.Clear();

        // IMPORTANTE: limpiar después
        
    }
}