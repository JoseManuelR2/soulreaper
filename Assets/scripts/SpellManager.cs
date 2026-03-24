using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public partial class SpellManager : MonoBehaviour
{
    private static SpellManager instance;
    public static void SetTeleport()
    {
        // Aquí puedes implementar la lógica de teletransporte
        Debug.Log("Ejecutando Teleport...");
        // Por ejemplo, podrías mover al jugador a una posición específica o a la posición del controlador
    }
}