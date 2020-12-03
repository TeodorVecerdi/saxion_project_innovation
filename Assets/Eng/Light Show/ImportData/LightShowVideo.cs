using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LightShowVideo : ScriptableObject {
    [HideInInspector] public string AssetGuid;
    [SerializeField] public LightShowVideoData Data;
    [NonSerialized] public bool Sampling;

    public void Initialize(LightShowVideoData data) {
        Data = data;
        AssetGuid = data.AssetGuid;
    }

    public void SetAssetGuid(string assetGuid) {
        AssetGuid = Data.AssetGuid = assetGuid;
    }

    public void SampleFrames() {
        if (!Directory.Exists($"{Application.dataPath}/Resources/Video Frames")) {
            Directory.CreateDirectory($"{Application.dataPath}/Resources/Video Frames");
        }

        if (!Directory.Exists($"{Application.dataPath}/Resources/Video Frames/{Data.VideoClipAssetGuid}")) {
            Directory.CreateDirectory($"{Application.dataPath}/Resources/Video Frames/{Data.VideoClipAssetGuid}");
        } else {
            Data.SampledFrames = true;
            LoadFrames();
            return;
        }
        Sampling = true;
        var gameObject = new GameObject("Temp_Sampler");
        var sampler = gameObject.AddComponent<VideoSampler>();
        sampler.Sample(Data.VideoClip, false, null, (frame, frameIndex) => {
            Debug.Log($"Saving frame {frameIndex}");
            File.WriteAllBytes($"{Application.dataPath}/Resources/Video Frames/{Data.VideoClipAssetGuid}/{frameIndex}.png", frame.EncodeToPNG());
        }, _ => {
            Debug.Log("Sampling complete [video]");
            Sampling = false;
            AssetDatabase.Refresh();
            Data.SampledFrames = true;
            sampler.QueueDestroy();
            LoadFrames();
        });
    }

    public void LoadFrames() {
        if (!Data.SampledFrames) {
            SampleFrames();
            return;
        }

        Data.Frames ??= new List<Texture2D>();

        if (Data.LoadedFrames || Data.Frames.Count > 0) {
            Data.Frames.Clear();
        }

        Data.Frames.AddRange(Resources.LoadAll<Texture2D>($"Video Frames/{Data.VideoClipAssetGuid}"));
        /*Data.Frames.Sort((first, second) => {
            var nameA = int.Parse(first.name);
            var nameB = int.Parse(second.name);
            return nameA.CompareTo(nameB);
        });*/
        // Data.Frames.AddRange(frames);
        Data.LoadedFrames = true;
    }
}