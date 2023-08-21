using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Music_", menuName = "Scriptable Objects/Sounds/Music")]
public class MusicSO : ScriptableObject
{
    public string musicName;
    public AudioClip music;
    public float volume = 1f;
}
