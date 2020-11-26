using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

[ExecuteInEditMode]
public class VideoSampler : MonoBehaviour {
    public VideoClip Clip;

    [HideInInspector] public List<Texture2D> Frames;
    [HideInInspector] public int SelectedFrame;
    [HideInInspector] public bool Sampling;
    private List<(int frameIndex, Texture2D texture)> tempList;
    private Action<Texture2D, int> onFrameReady;
    private Action<List<Texture2D>> onSampleComplete;
    private bool saveFrames;
    private VideoPlayer player;

    public void Sample(VideoClip clip = null, bool saveFrames = true, List<Texture2D> targetList = null, Action<Texture2D, int> onFrameReady = null, Action<List<Texture2D>> onSampleComplete = null) {
        var videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.clip = clip != null ? clip : Clip;
        videoPlayer.Stop();
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.prepareCompleted += Prepared;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.frameReady += FrameReady;
        
        player = videoPlayer;
        Frames ??= targetList;
        this.onFrameReady = onFrameReady;
        this.onSampleComplete = onSampleComplete;
        this.saveFrames = saveFrames;

        videoPlayer.loopPointReached += source => {
            OnSampleComplete();
        };

        tempList = new List<(int frameIndex, Texture2D texture)>();
        Sampling = true;
        videoPlayer.Prepare();
    }

    public void QueueDestroy() {
        DestroyImmediate(gameObject);
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
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.ARGB32, request => OnCompleteReadback(request, source, frameidx));
    }

    private void OnCompleteReadback(AsyncGPUReadbackRequest request, VideoPlayer source, long frameIndex) {
        if (request.hasError) {
            Debug.Log("GPU readback error detected.");
            return;
        }

        var texture = new Texture2D((int) source.width, (int) source.height, TextureFormat.ARGB32, false);
        texture.LoadRawTextureData(request.GetData<uint>());
        texture.Apply();
        if(saveFrames) 
            tempList.Add(((int) frameIndex, texture));
        onFrameReady?.Invoke(texture, (int) frameIndex);
        source.frame = frameIndex + 1;
        if (frameIndex + 1 >= (long) source.frameCount) {
            StartCoroutine(FinishedSampling(10, 2.5f));
        }
    }

    private void OnSampleComplete() {
        if (saveFrames) {
            tempList.Sort((tupleA, tupleB) => tupleA.frameIndex.CompareTo(tupleB.frameIndex));
            tempList.ForEach(tuple => Frames.Add(tuple.texture));
        }

        onSampleComplete?.Invoke(Frames);
        Sampling = false;
        DestroyImmediate(player);
    }

    private IEnumerator FinishedSampling(int timeoutTries, float timeout) {
        for (var i = 0; i < timeoutTries; i++) {
            if(Sampling) yield return new WaitForSecondsRealtime(timeout);
            else yield break;
        }
        
        yield return null;
        if (Sampling) {
            OnSampleComplete();
        }
    }
}