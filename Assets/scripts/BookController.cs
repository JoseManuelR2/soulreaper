using UnityEngine;
using UnityEngine.UI;

public class BookController : MonoBehaviour
{
    [Header("Referencias UI (Left Canvas)")]
    public Image hpBar;
    public Image manaBar;

    void Update()
    {
        if (PlayerController.Instance != null)
        {
            if (hpBar != null)
            {
                hpBar.fillAmount = PlayerController.Instance.health / PlayerController.Instance.maxHealth;
            }

            if (manaBar != null)
            {
                manaBar.fillAmount = PlayerController.Instance.mana / PlayerController.Instance.maxMana;
            }
        }
    }
}