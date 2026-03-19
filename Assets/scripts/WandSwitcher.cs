using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public partial class WandSwitcher : MonoBehaviour
{
    public static List<string> Spell = new List<string>();

    [Header("Referencias de Varitas")]
    public GameObject[] wands; // 0-2 bolas, 3-5 plumas
    private int currentBallIndex = 0;
    private int currentFeatherIndex = 0; // índice relativo de pluma (0-2)
    private bool activatedFeather = false;

    [Header("Input")]
    public InputActionProperty triggerAction; // Acción del gatillo

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
        RotateWand();
    }

    void RotateWand()
    {
        int ballsCount = 3;
        int feathersStart = 3; // índice inicial de las plumas en el array

        // 1️⃣ Desactivar la bola actual
        wands[currentBallIndex].SetActive(false);

        // 2️⃣ Calcular siguiente índice de bola
        currentBallIndex = (currentBallIndex + 1) % ballsCount;

        // 3️⃣ Activar la nueva bola
        wands[currentBallIndex].SetActive(true);

        // 4️⃣ Cada vez que volvemos a la bola 0, rotamos la pluma
        if (currentBallIndex == 0)
        {
            // Si ya activamos pluma antes, desactivamos la anterior
            if (activatedFeather)
            {
                wands[feathersStart + currentFeatherIndex].SetActive(false);
            }

            // Activar la pluma actual
            wands[feathersStart + currentFeatherIndex].SetActive(true);

            // Preparar índice de pluma para la próxima vez
            currentFeatherIndex = (currentFeatherIndex + 1) % ballsCount;

            activatedFeather = true;
        }

        Debug.Log($"Bola: {wands[currentBallIndex].name}, Pluma: {(activatedFeather ? wands[feathersStart + ((currentFeatherIndex + ballsCount - 1) % ballsCount)].name : "Ninguna")}");
    }

    public static void ProcessSpell()
    {
        Debug.Log("Procesando spell...");

        Debug.Log("Spell: " + string.Join(" - ", Spell));

        // Ejemplo de lógica simple
        if (Spell.Count == 0)
        {
            Debug.Log("No se ha seleccionado nada");
        }
        else if (Spell.Count == 3)
        {
            Debug.Log("Hechizo de nivel 3!");
        }

        // Aquí puedes meter combinaciones reales:
        // if (Spell[0] == "FireBlue" && Spell[1] == "FireRed") ...

        // IMPORTANTE: limpiar después
        Spell.Clear();
    }
}