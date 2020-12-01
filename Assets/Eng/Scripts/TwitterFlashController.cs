using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TwitterFlashController : MonoBehaviour {
    private static readonly int GlobalTransparency = Shader.PropertyToID("_globalTransparency");
    private static readonly int BorderSize = Shader.PropertyToID("_borderSize");
    private static readonly int BaseColor = Shader.PropertyToID("_baseColor");

    public Vector2 HueRange;
    public Material MaterialMain;
    public Material MaterialTower;
    public MeshFilter Main;
    public MeshFilter Tower;
    [Min(0.01f)] public float AnimationTime = 1f;
    public EasingFunction.Ease EaseType = EasingFunction.Ease.Linear;

    private bool isAnimating;
    private float animationTimer;
    private int animationDirection;
    private Color targetColor;


    public void OnTweetReceived() {
        animationTimer = 0f;
        isAnimating = true;
        animationDirection = 1;
        targetColor = Color.HSVToRGB(Random.Range(HueRange.x, HueRange.y)/360f, .9f, .9f);
        MaterialMain.SetColor(BaseColor, targetColor);
        MaterialTower.SetColor(BaseColor, targetColor);

        var mainBounds = Main.mesh.bounds.size;
        var mainBoundsV4 = new Vector4(mainBounds.x < mainBounds.z ? mainBounds.z : mainBounds.x, mainBounds.y, 0, 0);
        var towerBounds = Tower.mesh.bounds.size;
        var towerBoundsV4 = new Vector4(towerBounds.x < towerBounds.z ? towerBounds.z : towerBounds.x, towerBounds.y, 0, 0);
        MaterialMain.SetVector("_meshSize", mainBoundsV4);
        MaterialTower.SetVector("_meshSize", towerBoundsV4);
    }

    private void Update() {
        if(!isAnimating) return;

        animationTimer += animationDirection * Time.deltaTime;
        if (animationTimer >= AnimationTime) {
            animationTimer = AnimationTime;
            animationDirection = -1;
        }

        if (animationTimer < 0) {
            animationTimer = 0;
            isAnimating = false;
        }

        var amount = EasingFunction.GetEasingFunction(EaseType).Invoke(0f, 1f, animationTimer/AnimationTime);
        MaterialMain.SetFloat(BorderSize, amount);
        MaterialMain.SetFloat(GlobalTransparency, amount);
        MaterialTower.SetFloat(BorderSize, amount);
        MaterialTower.SetFloat(GlobalTransparency, amount);
    }
}

