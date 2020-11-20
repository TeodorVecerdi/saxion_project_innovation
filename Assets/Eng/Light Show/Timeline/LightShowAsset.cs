using UnityEngine;
using UnityEngine.Playables;

public class LightShowAsset : PlayableAsset {
    public LightShowBehaviour Template;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
        var playable = ScriptPlayable<LightShowBehaviour>.Create(graph, Template);
        return playable;
    }
}