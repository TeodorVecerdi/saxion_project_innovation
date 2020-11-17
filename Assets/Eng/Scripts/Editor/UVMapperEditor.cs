using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UVMapper))]
public class UVMapperEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Map")) {
            (target as UVMapper).Generate();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Backup Target Mesh")) {
            (target as UVMapper).BackupMesh();
        }
        if (GUILayout.Button("Restore Target Mesh")) {
            (target as UVMapper).RestoreBackupMesh();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Copy Mesh From Source")) {
            (target as UVMapper).CopyMesh();
        }
        if (GUILayout.Button("Random UVs")) {
            (target as UVMapper).RandomUV();
        }
    }
}