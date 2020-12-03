using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public static class BetterVideoSampler {
    private const string command = "ffmpeg.exe";
    private const string argsFormat = "-i \"{0}\" \"{1}/%4d.jpg\"";
    private const string videoFramesPath = "Resources/Video Frames";
    private static string AssetsPath => Application.dataPath;
    
    public static void Sample(VideoClip targetClip, Action onComplete = null) {
        var framesPath = Path.Combine(AssetsPath, videoFramesPath);
        var assetPath = AssetDatabase.GetAssetPath(targetClip);
        var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        var subPath = assetPath.Substring(assetPath.IndexOf('/') + 1);

        var targetVideoPath = Path.Combine(framesPath, assetGuid).Replace('\\', '/');
        var videoPath = Path.Combine(AssetsPath, subPath).Replace('\\', '/');
        Debug.Log($"{videoPath}\n{targetVideoPath}");
        if (!Directory.Exists(targetVideoPath)) Directory.CreateDirectory(targetVideoPath);
        var args = string.Format(argsFormat, videoPath, targetVideoPath);
        var prc = new System.Diagnostics.Process {
            StartInfo = {
                FileName = $"{command}",
                Arguments = $"{args}",
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            }
        };
        prc.Start();
        prc.WaitForExit();
        onComplete?.Invoke();
    }
}