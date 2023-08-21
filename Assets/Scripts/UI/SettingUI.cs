using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class SettingUI : MonoBehaviour
{

    public AudioMixer audioMusicMixer;
    public AudioMixer audioEffectMixer;

    public void Start()
    {
        Screen.fullScreen = true;
    }

    public void SetMusicVolume (float volume)
    {
        audioMusicMixer.SetFloat("musicVolume", volume);
    }

    public void SetEffectVolume(float volume)
    {
        audioEffectMixer.SetFloat("effectVolume", volume);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void Quit()
    {
        Destroy(GameObject.Find("BGM"));
        SceneManager.LoadScene("Menu");
    }
}
