using UnityEngine;
using UnityEngine.InputSystem; // Necesario para procesar el Input moderno

public partial class WandSwitcher : MonoBehaviour
{
    [Header("Referencias de Varitas")]
    public GameObject[] wands; // Arrastra aquí tus 3 modelos
    private int currentWandIndex = 0;

    [Header("Input")]
    public InputActionProperty triggerAction; // Acción del gatillo

    void OnEnable()
    {
        // Nos suscribimos al evento de "pulsado"
        triggerAction.action.performed += OnTriggerPressed;
    }

    void OnDisable()
    {
        triggerAction.action.performed -= OnTriggerPressed;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        RotateWand();
    }

    void RotateWand()
    {
        // 1. Desactivar varita actual
        wands[currentWandIndex].SetActive(false);

        // 2. Calcular siguiente índice (0, 1, 2 y vuelve a 0)
        currentWandIndex = (currentWandIndex + 1) % wands.Length;

        // 3. Activar nueva varita
        wands[currentWandIndex].SetActive(true);
        
        Debug.Log($"Varita cambiada al modelo: {wands[currentWandIndex].name}");
    }
}