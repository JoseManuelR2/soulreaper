using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Música - Lobby")]
    public AudioClip lobbyLoop;

    [Header("Música - Niveles (Loop)")]
    public AudioClip level1Loop;
    public AudioClip level2Loop;
    public AudioClip level3Loop;

    [Header("Narraciones")]
    public AudioClip startGameNarrative;
    public AudioClip selectLevelNarrative;
    public AudioClip completeLevelNarrative;

    [Header("SFX Globales (UI/Eventos)")]
    public AudioClip gameOverSound;
    public AudioClip victorySound;
    public AudioClip nextWaveSound;
    public AudioClip lowHealthSound;
    public AudioClip lowManaSound;
    public AudioClip wrongSpellSound;

    [Header("Audio Mixers")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup narrativeGroup;

    private AudioSource musicIntroSource;
    private AudioSource musicLoopSource;
    private AudioSource narrativeSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else Destroy(gameObject);
    }

    private void InitializeAudioSources()
    {
        musicIntroSource = gameObject.AddComponent<AudioSource>();
        musicLoopSource = gameObject.AddComponent<AudioSource>();
        narrativeSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicIntroSource.spatialBlend = 0f;
        musicLoopSource.spatialBlend = 0f;
        narrativeSource.spatialBlend = 0f;
        sfxSource.spatialBlend = 0f;

        if (musicGroup != null) { musicIntroSource.outputAudioMixerGroup = musicGroup; musicLoopSource.outputAudioMixerGroup = musicGroup; }
        if (narrativeGroup != null) narrativeSource.outputAudioMixerGroup = narrativeGroup;
        if (sfxGroup != null) sfxSource.outputAudioMixerGroup = sfxGroup;
    }

    private IEnumerator Start()
    {
        if (lobbyLoop != null) PlayLoopMusic(lobbyLoop);
        
        yield return new WaitForSeconds(2f);
        
        if (startGameNarrative != null) PlayNarrative(startGameNarrative);
    }

    public void PlayLoopMusic(AudioClip loop)
    {
        musicIntroSource.Stop(); musicLoopSource.Stop();
        musicLoopSource.clip = loop; musicLoopSource.loop = true; musicLoopSource.Play();
    }

    public void StopMusic()
    {
        musicIntroSource.Stop();
        musicLoopSource.Stop();
    }

    public void PlayNarrative(AudioClip clip) { if (clip != null) { narrativeSource.Stop(); narrativeSource.clip = clip; narrativeSource.Play(); } }
    public void PlaySFX(AudioClip clip) { if (clip != null) sfxSource.PlayOneShot(clip); }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 1f;
        if (sfxGroup != null) source.outputAudioMixerGroup = sfxGroup;
        source.Play();
        Destroy(tempGO, clip.length);
    }
}