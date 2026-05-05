using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Patrón Singleton para acceder fácilmente desde otros scripts
    public static PlayerController Instance { get; private set; }

    [Header("Atributos")]
    public float maxHealth = 100f;
    public float maxMana = 100f;

    [Header("Debug")]
    public float health;
    public float mana;

    [Header("Configuración de Daño")]
    [Tooltip("Tiempo en segundos durante el cual el jugador no puede recibir daño tras un golpe")]
    public float invulnerabilityTime = 1.5f; 
    private bool isInvulnerable = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        health = maxHealth;
        mana = maxMana;
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable) return;

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        
        Debug.Log("Jugador recibe daño. Vida actual: " + health);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityCooldown());
        }
    }

    private IEnumerator InvulnerabilityCooldown()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
    }
}