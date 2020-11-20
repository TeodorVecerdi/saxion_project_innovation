using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FileUtility {
    public static void CreateLightShowVideo(int instanceID, string assetPath, LightShowVideo video, bool refreshAsset = true) {
        if (video == null || string.IsNullOrEmpty(assetPath)) return;
        var jsonString = JsonUtility.ToJson(video.Data, true);
        Debug.Log(jsonString);
        File.WriteAllText(assetPath, jsonString);
        if (refreshAsset) AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    }

    public static LightShowVideo LoadLightShowVideo(string assetPath) {
        if (string.IsNullOrEmpty(assetPath)) return null;

        var jsonString = File.ReadAllText(assetPath);
        try {
            var data = JsonUtility.FromJson<LightShowVideoData>(jsonString);
            var lightShowVideo = ScriptableObject.CreateInstance<LightShowVideo>();
            lightShowVideo.Initialize(data);
            return lightShowVideo;
        } catch (ArgumentNullException exception) {
            Debug.LogException(exception);
            return null;
        }
    }

    public static bool SaveLightShowVideo(LightShowVideo video, bool reimport = true) {
        if (video == null) return false;
        if (string.IsNullOrEmpty(video.Data.AssetGuid)) return false;

        var assetPath = AssetDatabase.GUIDToAssetPath(video.Data.AssetGuid);
        if (string.IsNullOrEmpty(assetPath)) return false;

        var jsonString = JsonUtility.ToJson(video.Data, true);
        File.WriteAllText(assetPath, jsonString);
        if (reimport) AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        Debug.Log("Saved");
        return true;
    }
}