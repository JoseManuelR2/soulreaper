using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 35f;
    public float lifeTime = 3f; // Tiempo antes de que se destruya solo si no choca

    void Start()
    {
        Destroy(gameObject, lifeTime);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destruir la bola al impactar
        }
    }
}