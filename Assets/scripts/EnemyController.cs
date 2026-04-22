using UnityEngine;
using UnityEngine.AI;
using System; // Necesario para usar Action

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public bool canMove = true; 
    public float detectionRange = 20.0f;
    public float moveSpeed = 2.0f;

    [Header("Atributos")]
    public float health = 100f;
    public float resistance = 10f;

    // Evento que avisa al WaveManager cuando muere
    public event Action OnEnemyDeath; 

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private bool isDead = false;
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.speed = moveSpeed;

        if (Camera.main != null)
        {
            player = Camera.main.transform;
        }
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (canMove && distance <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.Play("Z_Walk");
        }
        else
        {
            agent.isStopped = true;
            anim.Play("Z_idle");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        float finalDamage = Mathf.Max(damage - resistance, 0);
        health -= finalDamage;

        Debug.Log("Zombi recibe daño. Vida restante: " + health);

        if (health <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true; 

        // --- Lógica para mirar al jugador al morir ---
        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0; // Evita que el zombie rote hacia arriba o hacia abajo si estás en terreno elevado

            if (directionToPlayer != Vector3.zero) 
            {
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }
        }

        anim.Play("Z_FallingBack");
        capsuleCollider.enabled = false;

        // Avisamos al WaveManager que hemos muerto
        OnEnemyDeath?.Invoke(); 

        Destroy(gameObject, 5f); 
    }
}