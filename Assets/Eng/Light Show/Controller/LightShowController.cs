using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LightShowController : MonoBehaviour {
    [HideInInspector] public ProjectionType ProjectionType;
    private Material material;
    private static readonly int CurrentFrame = Shader.PropertyToID("_currentFrame");
    private static readonly int ChromaKeyColor = Shader.PropertyToID("_chromaKeyColor");
    private static readonly int EmissionColor = Shader.PropertyToID("_emissionColor");
    private static readonly int GlobalTransparency = Shader.PropertyToID("_globalTransparency");
    private static readonly int UseScreenBlending = Shader.PropertyToID("_useScreenBlending");
    private static readonly int BorderSize = Shader.PropertyToID("_borderSize");
    private static readonly int Offset = Shader.PropertyToID("_offset");
    private static readonly int Tiling = Shader.PropertyToID("_tiling");
    private static readonly int BaseColor = Shader.PropertyToID("_baseColor");

    public void CloneMaterial() {
        var meshRenderer = GetComponent<MeshRenderer>();
        var mat = new Material(meshRenderer.sharedMaterial.shader);
        mat.CopyPropertiesFromMaterial(meshRenderer.sharedMaterial);
        meshRenderer.sharedMaterial = mat;
        material = mat;
        // material = meshRenderer.sharedMaterial = mat;
    }

    public void SetFrame(Texture2D frame) {
        material.SetTexture(CurrentFrame, frame);
    }

    public void SetEmissionColor(Color color) {
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

    public void SetOffset(Vector2 offset) {
        material.SetVector(Offset, offset.V4());
    }

    public void SetTiling(Vector2 tiling) {
        material.SetVector(Tiling, tiling.V4());
    }

    public void SetBaseColor(Color baseColor) {
        material.SetColor(BaseColor, baseColor);
    }
}