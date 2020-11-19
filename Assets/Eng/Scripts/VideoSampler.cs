using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[ExecuteInEditMode]
public class VideoSampler : MonoBehaviour {
    public VideoClip Clip;
    public Material VideoMaterial;
    
    [HideInInspector] public List<Texture2D> Frames;
    [HideInInspector] public int SelectedFrame;
    private Texture2DArray texture2DArray;
    
    public void Sample() {
        var videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.clip = Clip;
        videoPlayer.Stop();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.prepareCompleted += Prepared;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += FrameReady;
        videoPlayer.loopPointReached += DestroyImmediate;
        
        texture2DArray = new Texture2DArray((int) videoPlayer.width, (int) videoPlayer.height, (int) videoPlayer.frameCount, TextureFormat.RGB24, false);
        videoPlayer.Prepare();
    }

    public void SaveAs() {
        texture2DArray.Apply();
        var savePath = EditorUtility.SaveFilePanelInProject("Save As...", "Texture2DArray", "asset", "", Application.dataPath);
        savePath = savePath.Replace(Application.dataPath, "Assets");
        AssetDatabase.CreateAsset(texture2DArray, savePath);
    }

    public void ExportFrames() {
        var folderPath = EditorUtility.OpenFolderPanel("Export Frames", Application.dataPath, "Assets");
        folderPath = folderPath.Replace(Application.dataPath, "Assets");
        for (var i = 0; i < Frames.Count; i++) {
            var frame = Frames[i];
            var bytes = frame.EncodeToPNG();
            File.WriteAllBytes($"{folderPath}/{i}.png", bytes);
        }
        AssetDatabase.ImportAsset(folderPath);
    }

    private void Prepared(VideoPlayer source) {
        source.Pause();
    }

    private void FrameReady(VideoPlayer source, long frameidx) {
        var renderTexture = source.texture as RenderTexture;
        RenderTexture.active = renderTexture;
        var videoFrame = new Texture2D(renderTexture.width, renderTexture.height);
        videoFrame.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        videoFrame.Apply();
        RenderTexture.active = null;
        Frames.Add(videoFrame);
        texture2DArray.SetPixels(videoFrame.GetPixels(), (int) frameidx);
        source.frame = frameidx + 1;
    }
}