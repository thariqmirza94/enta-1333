using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip attackClip;
    public AudioClip damageClip;
    public AudioClip selectClip;
    public AudioClip placeClip;
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private Dictionary<string, AudioClip> sfxClips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxClips = new Dictionary<string, AudioClip>
        {
            {"attack", attackClip},
            {"damage", damageClip},
            {"select", selectClip},
            {"place", placeClip}
        };
    }

    public void PlaySFX(string clipName)
    {
        if (sfxClips.ContainsKey(clipName))
            sfxSource.PlayOneShot(sfxClips[clipName]);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();
    public void MuteSFX(bool mute) => sfxSource.mute = mute;
    public void MuteMusic(bool mute) => musicSource.mute = mute;

    public void MainMenuClick()
    {
        StopMusic();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameMusic()
    {
        PlayMusic(gameMusic);
    }
}