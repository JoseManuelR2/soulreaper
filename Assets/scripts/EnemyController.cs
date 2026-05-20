using UnityEngine;
using UnityEngine.AI;
using System; 

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
    public float damageToPlayer = 20f;
    public float manaRecovered = 30f;

    [Header("Audio Espacial Zombie (3D)")]
    public AudioSource audioSource;
    public AudioClip ambientSound;
    public AudioClip hurtSound;
    public AudioClip attackSound;
    public AudioClip deathSound;

    private float lastAttackSoundTime = 0f;

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

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource != null) 
        {
            audioSource.spatialBlend = 1.0f;
            if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
                audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
        }
        
        StartCoroutine(AmbientSoundRoutine());
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

    private System.Collections.IEnumerator AmbientSoundRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 8f));
            if (ambientSound != null && audioSource != null && !audioSource.isPlaying)
                audioSource.PlayOneShot(ambientSound);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayAttackSound();
            PlayerController.Instance?.TakeDamage(damageToPlayer);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") || (other.transform.parent != null && other.transform.parent.CompareTag("Player")))
        {
            PlayAttackSound();
            PlayerController.Instance?.TakeDamage(damageToPlayer);
        }
    }

    private void PlayAttackSound()
    {
        if (Time.time - lastAttackSoundTime > 1.2f)
        {
            if (attackSound != null && audioSource != null) audioSource.PlayOneShot(attackSound);
            lastAttackSoundTime = Time.time;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        float finalDamage = Mathf.Max(damage - resistance, 0);
        health -= finalDamage;

        if (hurtSound != null && audioSource != null) audioSource.PlayOneShot(hurtSound);

        Debug.Log("Zombi recibe daño. Vida restante: " + health);

        if (health <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true; 

        if (player != null)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero) 
            {
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }
        }

        if (deathSound != null) 
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFXAtPoint(deathSound, transform.position);
            else
                AudioSource.PlayClipAtPoint(deathSound, transform.position); 
        }

        anim.Play("Z_FallingBack");
        capsuleCollider.enabled = false;

        PlayerController.Instance?.AddMana(manaRecovered);

    
        OnEnemyDeath?.Invoke(); 

        Destroy(gameObject, 5f); 
    }
}