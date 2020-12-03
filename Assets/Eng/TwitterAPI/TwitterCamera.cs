using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TwitterCamera : MonoBehaviour {
    public RenderTexture TargetTexture;
    private Camera renderCamera;
    
    private void Awake() {
        renderCamera = GetComponent<Camera>();
        renderCamera.targetTexture = null;
    }

    private void Start() {
        Render();
    }

    public void Render() {
        renderCamera.targetTexture = TargetTexture;
        renderCamera.Render();
        renderCamera.targetTexture = null;
    }
}