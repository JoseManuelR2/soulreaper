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

    [Header("Configuración de Curación y Maná")]
    public float manaRegenRate = 10f;
    public float maxManaToRegen = 50f;
    private float currentManaRegenerated = 0f;
    private bool isRecoveringMana = false;

    public float healthRegenRate = 5f;
    public float maxHealthToRegen = 25f;
    private float currentHealthRegenerated = 0f;
    private bool isRecoveringHealth = false;

    [Header("Configuración de Daño")]
    [Tooltip("Tiempo en segundos durante el cual el jugador no puede recibir daño tras un golpe")]
    public float invulnerabilityTime = 1.5f; 
    private bool isInvulnerable = false;

    [Header("Configuración de Escudo")]
    private bool hasShield = false;
    private float shieldDuration = 30f;
    private float shieldTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        health = maxHealth;
        mana = maxMana;
    }

    private void Start()
    {
        if (GameOverManager.isRetry)
        {
            transform.rotation *= Quaternion.Euler(0, 180f, 0);
            GameOverManager.isRetry = false; // La apagamos para la próxima vez
        }
    }

    private void Update()
    {
        if (isRecoveringMana)
        {
            float regenStep = manaRegenRate * Time.deltaTime;
            if (currentManaRegenerated + regenStep > maxManaToRegen)
            {
                regenStep = maxManaToRegen - currentManaRegenerated;
                isRecoveringMana = false;
            }

            mana += regenStep;
            currentManaRegenerated += regenStep;

            if (mana >= maxMana)
            {
                mana = maxMana;
                isRecoveringMana = false;
            }
        }

        if (isRecoveringHealth)
        {
            float regenStep = healthRegenRate * Time.deltaTime;
            if (currentHealthRegenerated + regenStep > maxHealthToRegen)
            {
                regenStep = maxHealthToRegen - currentHealthRegenerated;
                isRecoveringHealth = false;
            }

            health += regenStep;
            currentHealthRegenerated += regenStep;

            if (health >= maxHealth)
            {
                health = maxHealth;
                isRecoveringHealth = false;
            }
        }

        if (hasShield)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                hasShield = false;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable) return;

        if (hasShield)
        {
            amount /= 2f; // Reducimos el daño a la mitad con el escudo
            Debug.Log("Daño reducido a la mitad por el escudo activo.");
        }

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

    public bool ConsumeMana(float amount)
    {
        if (mana >= amount)
        {
            mana -= amount;
            return true;
        }
        
        return false; // No hay suficiente maná
    }

    public void StartManaRecovery()
    {
        isRecoveringMana = true;
        currentManaRegenerated = 0f;
    }

    public void StartHealthRecovery()
    {
        isRecoveringHealth = true;
        currentHealthRegenerated = 0f;
    }

    public void ActivateShield()
    {
        hasShield = true;
        shieldTimer = shieldDuration;
    }

    public void CancelRegenerations()
    {
        isRecoveringMana = false;
        isRecoveringHealth = false;
    }

    public void AddMana(float amount)
    {
        mana += amount;
        mana = Mathf.Clamp(mana, 0, maxMana);
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
        GameOverManager.Instance?.TriggerGameOver();
    }
}