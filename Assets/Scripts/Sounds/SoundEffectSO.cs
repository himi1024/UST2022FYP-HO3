using UnityEngine;

[CreateAssetMenu(fileName = "Effect_", menuName = "Scriptable Objects/Sounds/Effect")]
public class SoundEffectSO : ScriptableObject
{
    public string effectName;

    public GameObject sound;

    public AudioClip audioClip;

    [Range(0.1f, 1.5f)]
    public float minPitch = 0.8f;

    [Range(0.1f, 1.5f)]
    public float maxPitch = 1.2f;

    [Range(0f, 1f)]
    public float volume = 1f;
}
