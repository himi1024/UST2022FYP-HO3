using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuMusic : MonoBehaviour
{
    public AudioSource clip;

    void Start()
    {
        clip.Play();
        DontDestroyOnLoad(gameObject);
    }
}
