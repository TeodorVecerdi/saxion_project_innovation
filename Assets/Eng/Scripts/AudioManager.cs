using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour {
    public Sound Sound;
    private AudioSource source;

    private void OnEnable() {
        if (source == null) source = GetComponent<AudioSource>();
        Debug.Log("OnEnable called");
        if (!source.isPlaying) {
            source.pitch = Random.Range(Sound.pitchMin, Sound.pitchMax);
            source.Play();
        }
    }

    private void OnDisable() {
        if (source.isPlaying)
            source.Stop();
    }
}