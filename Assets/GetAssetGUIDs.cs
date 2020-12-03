using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetAssetGUIDs : MonoBehaviour {
    public List<Object> Objects;

    public void GetGUIDs() {
        Objects.ForEach(obj => {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            Debug.Log($"{assetPath} => {assetGuid}");
        });
    }
}

[CustomEditor(typeof(GetAssetGUIDs))]
public class a : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Get guids")) {
            (target as GetAssetGUIDs).GetGUIDs();
        }
    }
}

