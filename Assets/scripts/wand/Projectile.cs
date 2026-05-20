using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 35f;
    public float lifeTime = 3f;

    [Header("Audio")]
    public AudioClip impactSound;

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
            if (impactSound != null)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFXAtPoint(impactSound, transform.position);
                else
                    AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }
            enemy.TakeDamage(damage);
        Destroy(gameObject);
        }
    }
}