using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonoBehaviour<SoundEffectManager>
{
    public int effectsVol = 8;
    public bool isPlaying = false;

    // For handling pressing problem
    [HideInInspector] GameObject pressingObject;
    [HideInInspector] SoundEffect pressingSound;

    private void Start()
    {
        float mute = -80f;
        /*
        if (effectsVol == 0) GameResources.Instance.masterSound.audioMixer.SetFloat("effectVolume", mute);
        else
        {
            float decibels = Mathf.Log10((float)effectsVol / 20f) * 20f;
            GameResources.Instance.masterSound.audioMixer.SetFloat("effectVolume", decibels);
        }
        */
    }

    public void Play(SoundEffectSO soundEffect, GameObject sourceObject = null)
    {
        if (soundEffect == null) return;
        
        GameObject soundObject = ObjectPooling.Instance.GetGameObject(soundEffect.sound);
        SoundEffect sound = soundObject.GetComponent<SoundEffect>();
        sound.SetSound(soundEffect);
        sound.audioSource.Play();

        if (sourceObject == null)
            StartCoroutine(DisableSound(soundObject, soundEffect.audioClip.length));
        else
            StartCoroutine(DisableSound(soundObject, soundEffect.audioClip.length, sourceObject));
    }

    // Prevent sound keeps creating when pressing
    public void PlayPressing(SoundEffectSO soundEffect)
    {
        if (soundEffect == null || isPlaying) return;

        isPlaying = true;

        pressingObject = ObjectPooling.Instance.GetGameObject(soundEffect.sound);
        pressingSound = pressingObject.GetComponent<SoundEffect>();

        pressingSound.SetSound(soundEffect);
        pressingSound.audioSource.Play();
    }

    public void DisablePlayPressing()
    {
        isPlaying = false;
        // Hardcoded to 2 second, really on method bro...
        // StartCoroutine(DisableSound(pressingObject, 2));
    }

    private IEnumerator DisableSound(GameObject soundObject, float duration, GameObject sourceObject = null)
    {
        if (sourceObject == null)
        {
            yield return new WaitForSeconds(duration);
            if (soundObject != null) ObjectPooling.Instance.PushGameObject(soundObject);
            yield break;
        }


        float endTime = Time.time + duration;
        float defaultVolume = 0f;
        defaultVolume = soundObject.GetComponent<AudioSource>().volume;

        while (endTime >= Time.time)
        {
            if(GameManager.Instance.GetPlayer()!= null)
                soundObject.GetComponent<AudioSource>().volume = defaultVolume * 5 / Mathf.Max(Vector2.Distance(GameManager.Instance.GetPlayer().playerPosition(), sourceObject.transform.position), 5);
            yield return null;
        }
        yield return new WaitForSeconds(duration);

        soundObject.GetComponent<AudioSource>().Stop();
        soundObject.GetComponent<AudioSource>().volume = defaultVolume;

        if (soundObject != null) ObjectPooling.Instance.PushGameObject(soundObject);
    }

    /* private IEnumerator DisableSound(GameObject soundObject, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (soundObject != null) ObjectPooling.Instance.PushGameObject(soundObject);
    }*/
}
