using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using SimplexNoise;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class Perlin : MonoBehaviour {
    public int TextureSize = 1024;
    public float Frequency = 4f;
    public int Seed = 0;
    public bool SeparateRGB;
    public bool Alpha;
    [HideInInspector] public Texture3D texture;

    public void Generate() {
        var texture = new Texture3D(TextureSize, TextureSize, TextureSize, TextureFormat.Alpha8, false);
        var colors = new Color[TextureSize * TextureSize * TextureSize];
        Noise.Seed = Seed;
        var min = 10000f;
        var max = -100000f;
        var noise = Noise.Calc3D(TextureSize, TextureSize, TextureSize, Frequency);
        for (int x = 0; x < TextureSize; x++) {
            for (int y = 0; y < TextureSize; y++) {
                for (int z = 0; z < TextureSize; z++) {
                    var noiseV = noise[x, y, z].Map(0, 255, 0, 1);
                    if (noise[x, y, z] < min) min = noise[x, y, z]; 
                    if (noise[x, y, z] >max) max = noise[x, y, z]; 
                    colors[x * TextureSize*TextureSize + y*TextureSize + z] = new Color(noiseV,noiseV,noiseV,noiseV);
                }
            }
        }
        
        Debug.Log($"{min} -> {max}");

        texture.SetPixels(colors);
        texture.Apply();
        this.texture = texture;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Perlin))]
public class PerlinMapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var generator = target as Perlin;
        if (GUILayout.Button("Generate Seed")) {
            generator.Seed = Random.Range(0, (1 << 28));
        }

        if (GUILayout.Button("Generate Image")) {
            generator.Generate();
        }

        if (generator.texture != null) {
            GUILayout.Space(6);
            var rect = GUILayoutUtility.GetAspectRect(1f, GUIStyle.none);

            // rect.height = rect.width;
            GUI.DrawTexture(rect, generator.texture);
            if (GUILayout.Button("Save As...")) {
                var savePath = EditorUtility.SaveFilePanelInProject("Save As...", "NoiseTexture3d", "asset", "", Application.dataPath);
                savePath = savePath.Replace(Application.dataPath, "Assets");
                AssetDatabase.CreateAsset(generator.texture, savePath);
            }
        }
    }
}
#endif