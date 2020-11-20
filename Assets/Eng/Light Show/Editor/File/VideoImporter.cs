using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, Extension, 3)]
public class VideoImporter : ScriptedImporter {
    public const string Extension = "lightshow";
    [HideInInspector] public LightShowVideo Instance;

    public override void OnImportAsset(AssetImportContext ctx) {
        var lightShowVideo = FileUtility.LoadLightShowVideo(ctx.assetPath);
        var icon = Resources.Load<Texture2D>("projector");

        if (string.IsNullOrEmpty(lightShowVideo.AssetGuid) || lightShowVideo.AssetGuid != AssetDatabase.AssetPathToGUID(ctx.assetPath)) {
            var assetGuid = AssetDatabase.AssetPathToGUID(ctx.assetPath);
            lightShowVideo.SetAssetGuid(assetGuid);
        }
        
        Instance = lightShowVideo;
        ctx.AddObjectToAsset("MainAsset", lightShowVideo, icon);
        ctx.SetMainObject(lightShowVideo);
        Debug.Log("Imported");
    }
}
