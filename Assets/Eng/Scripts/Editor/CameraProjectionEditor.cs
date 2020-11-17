using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraProjection))]
public class CameraProjectionEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Project")) {
            (target as CameraProjection).Generate();
        }
    }
}