using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnsureAudioSources()
    {
        if (musicSource == null || musicSource.gameObject == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null || sfxSource.gameObject == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        if (ambientSource == null || ambientSource.gameObject == null)
            ambientSource = gameObject.AddComponent<AudioSource>();
    }

    // =========================
    // MUSIC
    // =========================

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public bool IsMusicOn()
    {
        return !musicSource.mute;
    }

    // =========================
    // SFX
    // =========================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        if (sfxSource == null)
        {
            Debug.LogWarning(
                "SFX Source no asignado en AudioManager"
            );

            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    // =========================
    // AMBIENT
    // =========================

    public void PlayAmbient(AudioClip clip, bool loop = true)
    {
        if (clip == null)
            return;

        ambientSource.clip = clip;
        ambientSource.loop = loop;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }
}