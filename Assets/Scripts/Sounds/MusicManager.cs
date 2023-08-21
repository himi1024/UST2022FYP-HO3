using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public AudioClip currentAudioClip;
    private MusicSO nextAudio;

    private Coroutine fadeOut;
    private Coroutine fadeIn;

    private float fadeOutTime = Settings.musicFadeOutTime;
    private float fadeInTime = Settings.musicFadeInTime;

    public int volume = 10;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();

        currentAudioClip = audioSource.clip;
    }

    private void Start()
    {
        float mute = -80f;

        if (volume == 0)
        {
            GameResources.Instance.masterSound.audioMixer.SetFloat("musicVolume", mute);
        }
        else
        {
            float decibels = Mathf.Log10((float)volume / 20f) * 20f;
            GameResources.Instance.masterSound.audioMixer.SetFloat("musicVolume", decibels);
        }
    }

    public void Play(MusicSO music)
    {
        nextAudio = music;
        if (music.music != currentAudioClip)
            FadeOut();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(1, 0, t / fadeOutTime);
            yield return null;
        }
        audioSource.Stop();
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        audioSource.clip = nextAudio.music;
        audioSource.Play();
        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }
        currentAudioClip = nextAudio.music;
    }
}
