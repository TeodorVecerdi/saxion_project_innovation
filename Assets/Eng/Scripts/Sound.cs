using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    [HideInInspector]
    public AudioSource source;
    public AudioClip clip;

    public float volume = 0.2f;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.5f;

    public bool loop = false;
}

