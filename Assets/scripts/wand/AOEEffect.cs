using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AOEEffect : MonoBehaviour
{
    public float duration = 3f;

    [Header("Audio")]
    public AudioClip areaSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (areaSound != null && audioSource != null)
        {
            audioSource.spatialBlend = 1.0f;
            if (AudioManager.Instance != null && AudioManager.Instance.sfxGroup != null)
            {
                audioSource.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
            }
            audioSource.loop = true;
            audioSource.clip = areaSound;
            audioSource.Play();
        }

        Destroy(gameObject, duration);
    }
}