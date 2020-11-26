using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[Serializable]
public class LightShowBehaviour : PlayableBehaviour {
    [FormerlySerializedAs("Color"), ColorUsage(true, true)]
    public Color EmissionColor = Color.white * 5;
    public Color BaseColor = Color.white;
    [Tooltip("Whether or not transparency should be calculated using Screen Blending. This makes semi-transparent elements possible.")]
    public bool UseScreenBlending = false;
    [Range(0f, 1f)] public float GlobalTransparency = 1f;
    [Min(0.001f)] public float FrameRate = 30f;
    [Tooltip("Time in seconds to offset the current frame by.")]
    public float TimeOffset;
    [Tooltip("Specifies whether or not this clip should use the total movie time or the clip movie time for calculating the current frame.\nIn other words, if this is set to false, then the clip would always start with the first frame of the video.")]
    public bool UseGlobalTime;
    [Range(0f, 1f)] public float BorderSize = 1f;
    [Tooltip("Tiling of the projected video. 1,1 is the original; 2,2 is tiled twice.")]
    public Vector2 ProjectionTiling = Vector2.one;
    [Tooltip("Offset of the projected video. 0,0 is the original; 1,0 is shifted once on the x axis (should look the same)")]
    public Vector2 ProjectionOffset;
    [Tooltip("Location of the projection")]
    public ProjectionType ProjectionLocation;

    private LightShowController controller;
    private bool initializedPlayerData;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        var lightShowVideo = playerData as LightShowVideo;

        if (controller == null || lightShowVideo == null || lightShowVideo.Sampling)
            return;

        if (!lightShowVideo.Data.SampledFrames) {
            lightShowVideo.SampleFrames();
            return;
        }

        if (!lightShowVideo.Data.LoadedFrames) {
            lightShowVideo.LoadFrames();
            return;
        }

        var currentTime = UseGlobalTime ? playable.GetGraph().GetRootPlayable(0).GetTime() : playable.GetTime();
        var currentFrame = Mathf.FloorToInt((float) ((currentTime + TimeOffset) * FrameRate)) % lightShowVideo.Data.Frames.Count;
        while (currentFrame < 0) currentFrame += lightShowVideo.Data.Frames.Count;
        if (!initializedPlayerData) {
            controller.SetChromaKeyColor(lightShowVideo.Data.BackgroundRemovalColor);
            controller.SetUseScreenBlending(UseScreenBlending);
            controller.SetBorderSize(BorderSize);
            initializedPlayerData = true;
        }

        controller.SetEmissionColor(EmissionColor);
        controller.SetBaseColor(BaseColor);
        controller.SetTiling(ProjectionTiling);
        controller.SetOffset(ProjectionOffset);
        controller.SetTransparency(GlobalTransparency);
        controller.SetFrame(lightShowVideo.Data.Frames[currentFrame]);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info) {
        if (controller != null)
            LightShowMaster.Instance.ReleasePooled(controller, this);

        controller = LightShowMaster.Instance.GetPooled(this);
        initializedPlayerData = false;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info) {
        if (controller != null)
            LightShowMaster.Instance.ReleasePooled(controller, this);
        controller = null;
    }
}