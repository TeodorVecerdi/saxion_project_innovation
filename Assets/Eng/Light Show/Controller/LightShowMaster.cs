using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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

    public Transform MainTransform;
    public Transform ChurchClockAllTransform;
    public Transform ChurchClockPositiveXTransform;
    public Transform ChurchClockNegativeXTransform;
    public Transform ChurchClockPositiveZTransform;
    public LightShowController MainTemplate;
    public LightShowController ChurchClockAllTemplate;
    public LightShowController ChurchClockPositiveXTemplate;
    public LightShowController ChurchClockNegativeXTemplate;
    public LightShowController ChurchClockPositiveZTemplate;

    private List<Transform> transforms;
    private List<LightShowController> templates;
    private List<List<LightShowController>> lightShowsInactive;
    private List<List<LightShowController>> lightShowsActive;
    private List<int> pooledCount;
    

    private void BuildLists() {
        transforms = new List<Transform> {
            MainTransform,
            ChurchClockAllTransform,
            ChurchClockPositiveXTransform,
            ChurchClockNegativeXTransform,
            ChurchClockPositiveZTransform
        };
        templates = new List<LightShowController> {
            MainTemplate,
            ChurchClockAllTemplate,
            ChurchClockPositiveXTemplate,
            ChurchClockNegativeXTemplate,
            ChurchClockPositiveZTemplate
        };
        lightShowsInactive = new List<List<LightShowController>> {
            new List<LightShowController>(),
            new List<LightShowController>(),
            new List<LightShowController>(),
            new List<LightShowController>(),
            new List<LightShowController>()
        };
        lightShowsActive = new List<List<LightShowController>> {
            new List<LightShowController>(),
            new List<LightShowController>(),
            new List<LightShowController>(),
            new List<LightShowController>(),
            new List<LightShowController>()
        };
        pooledCount = new List<int> {
            2,
            2,
            2,
            2,
            2
        };
    }

    public void ResetPool() {
        if (transforms == null || templates == null || lightShowsInactive == null || lightShowsActive == null) {
            BuildLists();
        }
        
        if (lightShowsActive.SelectMany(pair => pair).Count() != 0) {
            Debug.LogError("Cannot reset Light Show pool while objects are active");
            return;
        }

        foreach (var lightShowsInactiveType in lightShowsInactive) {
            lightShowsInactiveType.ForEach(controller => {
                if (controller != null) DestroyImmediate(controller.gameObject);
            });
            lightShowsInactiveType.Clear();
        }

        for (var i = 0; i < pooledCount.Count; i++) {
            pooledCount[i] = 2;
            for (var j = 0; j < pooledCount[i]; j++) {
                lightShowsInactive[i].Add(CreateObject((ProjectionType)i));
            }
        }
    }

    public LightShowController GetPooled(LightShowBehaviour requester) {
        var type = (int)requester.ProjectionLocation;
        if (pooledCount[type] == 0) ResetPool();
        if (lightShowsInactive[type].Count == 0) {
            var numToCreate = pooledCount[type];
            pooledCount[type] *= 2;
            for (var i = 0; i < numToCreate; i++) {
                lightShowsInactive[type].Add(CreateObject((ProjectionType)type));
            }
        }

        var controller = lightShowsInactive[type][0];
        lightShowsInactive[type].RemoveAt(0);
        lightShowsActive[type].Add(controller);
        OnActivate(controller, requester);
        return controller;
    }

    public void ReleasePooled(LightShowController controller, LightShowBehaviour requester) {
        var type = (int)requester.ProjectionLocation;
        if (!lightShowsActive[type].Contains(controller)) {
            Debug.LogError("Trying to release LightShowController when it is not active.");
        }

        lightShowsActive[type].Remove(controller);
        lightShowsInactive[type].Add(controller);
        OnDeactivate(controller, requester);
    }

    public LightShowController CreateObject(ProjectionType projectionType) {
        var instance = Instantiate(templates[(int)projectionType], transforms[(int)projectionType].position, transforms[(int)projectionType].rotation, transform);
        instance.ProjectionType = projectionType;
        instance.transform.localScale = transforms[(int)projectionType].localScale;
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