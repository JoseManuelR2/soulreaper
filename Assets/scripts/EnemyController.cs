using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public bool canMove = true; // Lo pongo en true por defecto para que pruebes
    public float detectionRange = 20.0f;
    public float moveSpeed = 2.0f;

    [Header("Atributos")]
    public float health = 100f;
    public float resistance = 10f;

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
            // 1. Activar el agente y darle el destino (el NavMesh calcula el camino)
            agent.isStopped = false;
            agent.SetDestination(player.position);

            // 2. Animación
            anim.Play("Z_Walk");
        }
        else
        {
            // Detener al agente si está fuera de rango
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
        agent.isStopped = true; // Que deje de caminar al morir
        anim.Play("Z_FallingBack");
        capsuleCollider.enabled = false;
        Destroy(gameObject, 5f); // Le damos medio segundo antes de desaparecer
    }
}