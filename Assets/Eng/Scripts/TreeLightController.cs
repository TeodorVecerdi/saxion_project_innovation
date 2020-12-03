using System.Collections.Generic;
using UnityEngine;

public class TreeLightController : MonoBehaviour {
    public List<Light> Lights;

    public void OnTweetReceived(float tweetHue) {
        var color = Color.HSVToRGB(tweetHue, 1, 1);
        Lights.ForEach(light => light.color = color);   
    }
}