using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class CreateLightShowVideo : EndNameEditAction {
    [MenuItem("Assets/Create/Light-Show Video", false, 0)]
    public static void CreateFile() {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateLightShowVideo>(),
            $"New Light-Show Video.{VideoImporter.Extension}", Resources.Load<Texture2D>("projector"), null);
    }

    public override void Action(int instanceId, string pathName, string resourceFile) {
        var data = new LightShowVideoData();
        var lightShowVideo = CreateInstance<LightShowVideo>();
        lightShowVideo.Initialize(data);
        FileUtility.CreateLightShowVideo(instanceId, pathName, lightShowVideo);
    }
}