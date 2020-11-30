using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public Sound sound;

    void Awake()
    {
        sound.source = gameObject.AddComponent<AudioSource>(); 
        sound.source.clip = sound.clip;
        sound.source.volume = sound.volume;
        //sound.source.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
        sound.source.playOnAwake = false;
        sound.source.loop = sound.loop;
    }

    private void Start()
    {
        sound.source.Stop(); //starts when pressing play for some reason
    }

    private void OnEnable(){
        if (!sound.source.isPlaying)
        {
            sound.source.pitch = Random.Range(sound.pitchMin, sound.pitchMax);
            sound.source.Play();
        }
    }

    private void OnDisable() {
     if(sound.source.isPlaying)
         sound.source.Stop();
    }
}
