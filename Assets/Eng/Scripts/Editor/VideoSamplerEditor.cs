using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CustomEditor(typeof(VideoSampler))]
public class VideoSamplerEditor : Editor {
    public override void OnInspectorGUI() {
        var sampler = target as VideoSampler;
        base.OnInspectorGUI();

        var hasComponent = sampler.gameObject.GetComponent<VideoPlayer>() != null;
        if (hasComponent) GUI.enabled = false;
        if (GUILayout.Button("Sample")) {
            sampler.Sample();
        }

        if (GUILayout.Button("Clear Frames")) {
            sampler.Frames.Clear();
            sampler.SelectedFrame = 0;
        }
        if (sampler.Frames.Count == 0)
            GUI.enabled = false;
        if (GUILayout.Button("Save as Texture2DArray...")) {
            sampler.SaveAs();
        }
        if (GUILayout.Button("Export Frames (as png)...")) {
            sampler.ExportFrames();
        }

        GUILayout.Space(4);

        sampler.SelectedFrame = EditorGUILayout.IntSlider("Frame", sampler.SelectedFrame, 0, sampler.Frames.Count - 1);
        if(!hasComponent)
            GUI.enabled = true;

        if (sampler.SelectedFrame < sampler.Frames.Count) {
            var selected = sampler.Frames[sampler.SelectedFrame];
            sampler.VideoMaterial.mainTexture = selected;
            var rect = GUILayoutUtility.GetAspectRect((float) selected.width / selected.height, GUIStyle.none);
            GUI.DrawTexture(rect, selected);
        }

        GUI.enabled = true;
    }
}