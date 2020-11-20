using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class LightShowVideoData {
    public Color BackgroundRemovalColor = Color.green;
    public string AssetGuid;
    [SerializeReference] public VideoClip VideoClip;
    public string VideoClipAssetGuid;
    [NonSerialized] public List<Texture2D> Frames;
    public bool SampledFrames;
    [NonSerialized] public bool LoadedFrames;
}