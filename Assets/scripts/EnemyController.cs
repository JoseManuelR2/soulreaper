using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public bool canMove = false;
    public float detectionRange = 20.0f;
    public float moveSpeed = 2.0f;

    [Header("Atributos")]
    public float health = 100f;
    public float resistance = 10f;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;

    void Start()
    {
        
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
    if (player == null) return;

    float distance = Vector3.Distance(transform.position, player.position);

    if (canMove && distance <= detectionRange)
    {
        // 1. Calculamos un punto de destino que esté a la misma altura que el zombi
        // Esto evita que "vuele" hacia tu cabeza
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);

        // 2. Rotar para mirar al jugador
        transform.LookAt(targetPosition);

        // 3. Moverse hacia ese punto en el suelo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // 4. Animación
        anim.Play("Z_Walk"); 
    }
    else
    {
        anim.Play("Z_idle");
    }

    if (health <= 0) Die();
}

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - resistance, 0);
        health -= finalDamage;
    }

    private void Die()
    {
        // Podrías añadir anim.Play("Z_Death") aquí antes de destruir
        Destroy(gameObject);
    }
}