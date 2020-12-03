using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteAlways]
public class FrameVideoPlayer : MonoBehaviour {
    public string MaterialFrameKey;
    public Material Material;
    public List<Texture2D> Frames;

    public float FrameRate = 30f;
    public bool Enabled;

    private void Update() {
        if(!Enabled) return;
        
        var currentFrame = Mathf.FloorToInt(Time.time * FrameRate) % Frames.Count;
        Material.SetTexture(MaterialFrameKey, Frames[currentFrame]);
    }

    private void OnEnable() {
        EditorApplication.update += Update;
    }

    private void OnDisable() {
        EditorApplication.update -= Update;
    }
}