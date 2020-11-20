using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightShowMaster))]
public class LightShowMasterEditor : Editor {
    public override void OnInspectorGUI() {
        if (GUILayout.Button("Reset Pool")) {
            (target as LightShowMaster).ResetPool();
            LightShowMaster.Instance = target as LightShowMaster;
        }
        base.OnInspectorGUI();
    }
}