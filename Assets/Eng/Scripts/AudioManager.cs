using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;

    void Awake()
    {
        //Singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy((gameObject));
            return;
        }
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = Random.Range(s.pitchMin, s.pitchMax);
            s.source.loop = s.loop;
        }
        PlayRandomSound();
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound: " + name + " could not be found!");
            return;
        }
        sound.source.Play();
        Debug.Log("Playing " + name);
    }
    
    public void PlayRandomSound()
    {
        var randomSoundIndex = Random.Range(0, sounds.Length);
        Sound randomSound = sounds[randomSoundIndex];
        if (randomSound == null)
        {
            Debug.LogWarning("Sound: " + name + " could not be found!");
            return;
        }
        randomSound.source.Play();
        Debug.Log("Playing " + randomSound.name);
    }

}
