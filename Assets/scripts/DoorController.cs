using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))] 
public class DoorController : MonoBehaviour
{
    [Header("Configuración de la puerta")]
    public float openAngle = 100f;
    public float duration = 3.3f;

    [Header("Configuración de Audio")]
    public AudioSource audioSource;
    [Tooltip("Sonido de la puerta")]
    public AudioClip doorSound;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine currentCoroutine;

    void Awake()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (audioSource != null && AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
        {
            audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
        }
    }

    public void OpenDoor()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(RotateDoor(openRotation));

        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
    }

    public void CloseDoor()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(RotateDoor(closedRotation));

        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
    }

    IEnumerator RotateDoor(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float time = 0f;

        while (time < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
    }
}