using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightShowMaster : MonoBehaviour {
    private static LightShowMaster instance;
    public static LightShowMaster Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<LightShowMaster>();
                instance.ResetPool();
            }

            return instance;
        }
        set => instance = value;
    }

    [SerializeField] private int pooledCount;
    public Transform TemplateTransform;
    public LightShowController LightShowTemplate;
    public List<LightShowController> LightShowInactive;
    public List<LightShowController> LightShowActive;

    public void ResetPool() {
        if (LightShowActive.Count != 0) {
            Debug.LogError("Cannot reset Light Show pool while objects are active");
            return;
        }
        LightShowInactive.ForEach(controller => DestroyImmediate(controller.gameObject));
        LightShowInactive.Clear();
        pooledCount = 2;
        for (var i = 0; i < pooledCount; i++) {
            LightShowInactive.Add(CreateObject());
        }
    }

    public LightShowController GetPooled(LightShowBehaviour requester) {
        if(pooledCount == 0) ResetPool();
        if (LightShowInactive.Count == 0) {
            var numToCreate = pooledCount;
            pooledCount *= 2;
            for (var i = 0; i < numToCreate; i++) {
                LightShowInactive.Add(CreateObject());
            }
        }

        var controller = LightShowInactive[0];
        LightShowInactive.RemoveAt(0);
        LightShowActive.Add(controller);
        OnActivate(controller, requester);
        return controller;
    }

    public void ReleasePooled(LightShowController controller, LightShowBehaviour requester) {
        if (!LightShowActive.Contains(controller)) {
            Debug.LogError("Trying to release LightShowController when it is not active.");
        }

        LightShowActive.Remove(controller);
        LightShowInactive.Add(controller);
        OnDeactivate(controller, requester);
    }
    
    public LightShowController CreateObject() {
        var instance = Instantiate(LightShowTemplate, TemplateTransform.position, TemplateTransform.rotation, transform);
        instance.transform.localScale = TemplateTransform.localScale;
        instance.transform.SetParent(transform, true);
        instance.CloneMaterial();
        instance.gameObject.SetActive(false);
        return instance;
    }

    public void OnActivate(LightShowController controller, LightShowBehaviour requester) {
        controller.gameObject.SetActive(true);
    }

    public void OnDeactivate(LightShowController controller, LightShowBehaviour requester) {
        controller.gameObject.SetActive(false);
    }
}