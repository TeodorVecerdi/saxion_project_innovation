using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Projector))]
public class ProjectorEditor : Editor {
    public override void OnInspectorGUI() {
        var projector = target as Projector;
        
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset")) {
            projector.ResetCalibration();
        }

        if (GUILayout.Button("Calibration [Stage 1]")) {
            projector.FirstStageCalibration();
        }
        if (GUILayout.Button("Interactive Calibration [Stage 1]")) {
            projector.FirstStageInteractive();
        }

        if (GUILayout.Button("Calibration [Stage 2]")) {
            projector.SecondStageCalibration();
        }
        if (GUILayout.Button("Interactive Calibration [Stage 2]")) {
            projector.SecondStageInteractive();
        }
        
        if (GUILayout.Button("Calibration [Final Stage]")) {
        }
        if (GUILayout.Button("Interactive Calibration [Final Stage]")) {
            projector.ThirdStageInteractive();
        }
    }
}