using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LightShowController : MonoBehaviour {
    private Material material;
    private static readonly int CurrentFrame = Shader.PropertyToID("_currentFrame");
    private static readonly int ChromaKeyColor = Shader.PropertyToID("_chromaKeyColor");
    private static readonly int EmissionColor = Shader.PropertyToID("_emissionColor");
    private static readonly int GlobalTransparency = Shader.PropertyToID("_globalTransparency");
    private static readonly int UseScreenBlending = Shader.PropertyToID("_useScreenBlending");
    private static readonly int BorderSize = Shader.PropertyToID("_borderSize");

    public void SetFrame(Texture2D frame) {
        material.SetTexture(CurrentFrame, frame);
    }

    public void CloneMaterial() {
        var meshRenderer = GetComponent<MeshRenderer>();
        var newMaterial = new Material(meshRenderer.sharedMaterial);
        material = meshRenderer.material = newMaterial;
    }

    public void SetColor(Color color) {
        material.SetColor(EmissionColor, color);
    }

    public void SetChromaKeyColor(Color color) {
        material.SetColor(ChromaKeyColor, color);
    }

    public void SetTransparency(float transparency) {
        material.SetFloat(GlobalTransparency, transparency);
    }

    public void SetBorderSize(float borderSize) {
        material.SetFloat(BorderSize, borderSize);
    }

    public void SetUseScreenBlending(bool useScreenBlending) {
        material.SetFloat(UseScreenBlending, useScreenBlending ? 1.0f : 0.0f);
    }
}