using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music Source")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip _defaultMusic;
    [SerializeField] private float _musicVolume = 0.5f;

    [Header("SFX Source")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private float _sfxVolume = 1f;

    [Header("Game SFX Clips")]
    [SerializeField] private AudioClip _winClip;
    [SerializeField] private AudioClip _loseClip;
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _popClip;
    [SerializeField] private AudioClip _explosionClip;

    public enum Sfx { Click, Pop, Win, Lose, Explosion }

    public static AudioManager I { get; private set; }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        _musicSource.loop = true;
        _musicSource.volume = _musicVolume;
        _sfxSource.volume = _sfxVolume;

        if (_defaultMusic != null)
            PlayMusic(_defaultMusic);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void StopMusic() => _musicSource.Stop();

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, _sfxVolume);
    }

    public void PlaySFX(Sfx type)
    {
        switch (type)
        {
            case Sfx.Click: PlaySFX(_clickClip); break;
            case Sfx.Pop: PlaySFX(_popClip); break;
            case Sfx.Win: PlaySFX(_winClip); break;
            case Sfx.Lose: PlaySFX(_loseClip); break;
            case Sfx.Explosion: PlaySFX(_explosionClip); break;
        }
    }

    public void SetMusicVolume(float v) => _musicSource.volume = Mathf.Clamp01(v);
    public void SetSFXVolume(float v) => _sfxSource.volume = Mathf.Clamp01(v);
}
