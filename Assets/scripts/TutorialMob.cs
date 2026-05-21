using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;

public class TutorialMob : MonoBehaviour
{
    [Header("Atributos")]
    public float maxHealth = 100f;
    private float currentHealth;
    private float lastDamage = 0f;

    private Vector3 startPos;
    private Quaternion startRot;
    private bool isDead = false;
    private bool isDespawned = false;

    private Animator anim;
    private CapsuleCollider capsuleCollider;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    [Header("UI Barra de Vida (Asignar en Inspector)")]
    [Tooltip("El objeto Canvas que contiene la barra de vida")]
    public Transform healthCanvasTransform;
    [Tooltip("La imagen verde de la barra (Image Type debe ser Filled)")]
    public Image greenBarFill;
    [Tooltip("La imagen roja de la barra (Image Type debe ser Filled)")]
    public Image redBarFill;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        SetupAudio();
        SetupHealthBar();
    }

    void SetupAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;
        audioSource.playOnAwake = false;

        if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
        {
            audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
        }
    }

    void SetupHealthBar()
    {
        if (healthCanvasTransform != null)
        {
            healthCanvasTransform.SetParent(null);
            UpdateUI();
        }
        else
        {
            Debug.LogError("Falta asignar el Health Canvas en el inspector de TutorialMob.");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isDespawned) return;

        lastDamage = damage;
        currentHealth -= damage;
        
        if (hurtSound != null && audioSource != null) audioSource.PlayOneShot(hurtSound);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateUI();
            Die();
        }
        else 
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (greenBarFill != null && redBarFill != null)
        {
            float greenPct = currentHealth / maxHealth;
            float redPct = Mathf.Clamp01((currentHealth + lastDamage) / maxHealth);

            greenBarFill.fillAmount = greenPct;
            redBarFill.fillAmount = redPct;
        }
    }

    void Die()
    {
        isDead = true;
        
        if (deathSound != null && audioSource != null) audioSource.PlayOneShot(deathSound);

        if (anim != null) anim.Play("Z_FallingBack");
        if (capsuleCollider != null) capsuleCollider.enabled = false;

        if (healthCanvasTransform != null) healthCanvasTransform.gameObject.SetActive(false);

        StartCoroutine(ReviveRoutine());
    }

    IEnumerator ReviveRoutine()
    {
        yield return new WaitForSeconds(3f);

        isDead = false;
        currentHealth = maxHealth;
        lastDamage = 0f;
        
        UpdateUI();

        transform.position = startPos;
        transform.rotation = startRot;
        
        if (anim != null) 
        {
            anim.Rebind();
            anim.Update(0f);
            anim.Play("Z_idle", -1, 0f);
        }
        
        if (capsuleCollider != null) capsuleCollider.enabled = true;
        if (healthCanvasTransform != null) healthCanvasTransform.gameObject.SetActive(true);
    }

    public void DespawnMob()
    {
        if (isDespawned) return;
        isDespawned = true;
        StopAllCoroutines();
        StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        if (!isDead)
        {
            isDead = true;
            if (deathSound != null && audioSource != null) audioSource.PlayOneShot(deathSound);
            if (anim != null) anim.Play("Z_FallingBack");
            if (capsuleCollider != null) capsuleCollider.enabled = false;
            if (healthCanvasTransform != null) healthCanvasTransform.gameObject.SetActive(false);
            yield return new WaitForSeconds(3f);
        }

        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers) r.enabled = false;
    }

    public void RespawnMob()
    {
        if (!isDespawned) return;
        
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers) r.enabled = true;

        isDespawned = false;
        StopAllCoroutines();
        
        isDead = false;
        currentHealth = maxHealth;
        lastDamage = 0f;
        UpdateUI();

        transform.position = startPos;
        transform.rotation = startRot;
        
        if (anim != null) 
        {
            anim.Rebind();
            anim.Update(0f);
            anim.Play("Z_idle", -1, 0f);
        }
        
        if (capsuleCollider != null) capsuleCollider.enabled = true;
        if (healthCanvasTransform != null) healthCanvasTransform.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (Camera.main != null && healthCanvasTransform != null)
        {
            healthCanvasTransform.position = transform.position + Vector3.up * 2.2f;
            healthCanvasTransform.rotation = Quaternion.LookRotation(healthCanvasTransform.position - Camera.main.transform.position);
        }
    }

    void OnDestroy()
    {
        if (healthCanvasTransform != null)
        {
            Destroy(healthCanvasTransform.gameObject);
        }
    }
}