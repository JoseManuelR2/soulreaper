using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    [Header("Referencias UI (Left Canvas)")]
    public Image hpBar;
    public Image manaBar;

    void Update()
    {
        // Asegurarnos de que el jugador existe antes de actualizar
        if (PlayerController.Instance != null)
        {
            if (hpBar != null)
            {
                // fillAmount va de 0 a 1, así que dividimos la vida actual por la máxima
                hpBar.fillAmount = PlayerController.Instance.health / PlayerController.Instance.maxHealth;
            }

            if (manaBar != null)
            {
                // Hacemos lo mismo para el maná
                manaBar.fillAmount = PlayerController.Instance.mana / PlayerController.Instance.maxMana;
            }
        }
    }
}