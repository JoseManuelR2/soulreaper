using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AOEProjectile : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    public float speed = 30f;
    public float damage = 40f;
    public float lifeTime = 3f; 

    [Header("Configuración AOE")]
    public float aoeRadius = 5f;
    [Tooltip("Efecto visual que se instanciará al explotar")]
    public GameObject aoeEffectPrefab;
    
    [Tooltip("Distancia a la que se buscará el suelo para colocar el efecto correctamente")]
    public float floorSearchDistance = 10f;

    [Header("Audio")]
    public AudioClip impactSound;

    private bool hasExploded = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.GetComponent<Projectile>() != null || other.GetComponent<AOEProjectile>() != null) return;
        
        if (other.isTrigger && other.GetComponent<EnemyController>() == null) return;

        if (!hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        if (impactSound != null)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFXAtPoint(impactSound, transform.position);
            else
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
        }

        if (aoeEffectPrefab != null)
        {
            Vector3 effectPosition = transform.position;
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, Vector3.down, out hit, floorSearchDistance))
            {
                effectPosition = hit.point;
            }
            
            Instantiate(aoeEffectPrefab, effectPosition, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);
        foreach (Collider col in colliders)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}