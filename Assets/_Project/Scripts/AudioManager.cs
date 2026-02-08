using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip moveSound;
    public AudioClip mergeSound;
    public AudioClip powerupSound;
    public AudioClip gameOverSound;
    
    private bool isMuted = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Cargar el estado del audio guardado
            isMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
            ApplyMuteState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (musicSource != null && backgroundMusic != null && !isMuted)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlayMove()
    {
        if (sfxSource != null && moveSound != null && !isMuted)
        {
            sfxSource.PlayOneShot(moveSound);
        }
    }

    public void PlayMerge(int value)
    {
        if (sfxSource != null && mergeSound != null && !isMuted)
        {
            // Variar el pitch según el valor para hacer el sonido más interesante
            float pitch = 1f + (value * 0.05f);
            pitch = Mathf.Clamp(pitch, 0.8f, 2f);
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(mergeSound);
            sfxSource.pitch = 1f; // Resetear el pitch
        }
    }

    public void PlayPowerup()
    {
        if (sfxSource != null && powerupSound != null && !isMuted)
        {
            sfxSource.PlayOneShot(powerupSound);
        }
    }

    public void PlayGameOver()
    {
        if (sfxSource != null && gameOverSound != null && !isMuted)
        {
            sfxSource.PlayOneShot(gameOverSound);
        }
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;
        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }
        
        if (sfxSource != null)
        {
            sfxSource.mute = isMuted;
        }
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}