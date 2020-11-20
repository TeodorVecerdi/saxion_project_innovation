using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Video;

[CustomEditor(typeof(VideoImporter))]
public class VideoImporterEditor : ScriptedImporterEditor {
    public bool Modified;
    private int selectedFrame;

    public override void OnInspectorGUI() {
        var importer = target as VideoImporter;

        if (importer.Instance.Sampling) {
            var style = new GUIStyle(EditorStyles.boldLabel) {fontSize = 24, alignment = TextAnchor.MiddleCenter};
            GUILayout.Label("VIDEO SAMPLING IN PROGRESS", style);
            GUI.enabled = false;
        }

        if (GUILayout.Button("Sample Video")) {
            Debug.Log(Application.dataPath);
            importer.Instance.SampleFrames();
        }

        EditorGUI.BeginChangeCheck();
        var videoClip = EditorGUILayout.ObjectField("Source Video", importer.Instance.Data.VideoClip, typeof(VideoClip), false) as VideoClip;
        if (EditorGUI.EndChangeCheck()) {
            if (videoClip != importer.Instance.Data.VideoClip) {
                importer.Instance.Data.SampledFrames = false;
                importer.Instance.Data.LoadedFrames = false;
                Modified = true;
                importer.Instance.Data.VideoClip = videoClip;
                if (videoClip != null) {
                    importer.Instance.Data.VideoClipAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(videoClip));
                    importer.Instance.SampleFrames();
                }
            }
        }

        EditorGUI.BeginChangeCheck();
        var color = EditorGUILayout.ColorField("Background Color", importer.Instance.Data.BackgroundRemovalColor);
        if (EditorGUI.EndChangeCheck()) {
            Modified = true;
            importer.Instance.Data.BackgroundRemovalColor = color;
        }

        if (!importer.Instance.Sampling && importer.Instance.Data.Frames != null && importer.Instance.Data.Frames.Count > 0) {
            selectedFrame = EditorGUILayout.IntSlider("Frame", selectedFrame, 0, importer.Instance.Data.Frames.Count - 1);
            GUI.enabled = true;

            if (selectedFrame >= 0 && selectedFrame < importer.Instance.Data.Frames.Count) {
                var selected = importer.Instance.Data.Frames[selectedFrame];
                var rect = GUILayoutUtility.GetAspectRect((float) selected.width / selected.height, GUIStyle.none);
                GUI.DrawTexture(rect, selected);
            }
        }

        GUI.enabled = true;
        EditorGUILayout.Space();
        ApplyRevertGUI();
    }

    protected override void ResetValues() {
        Modified = false;
        var importer = target as VideoImporter;
        AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(importer.Instance.AssetGuid));
    }

    protected override bool needsApplyRevert => true;

    public override bool HasModified() {
        return Modified || base.HasModified();
    }

    protected override void Apply() {
        Modified = false;
        var importer = target as VideoImporter;
        FileUtility.SaveLightShowVideo(importer.Instance);

        // base.Apply();
    }
}