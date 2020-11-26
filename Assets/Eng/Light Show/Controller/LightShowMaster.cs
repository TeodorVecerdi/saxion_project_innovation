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

    [SerializeField,HideInInspector] private SerializedDictionary<ProjectionType, Transform> transforms;
    [SerializeField,HideInInspector] private SerializedDictionary<ProjectionType, LightShowController> templates;
    [SerializeField,HideInInspector] private SerializedDictionary<ProjectionType, List<LightShowController>> lightShowsInactive;
    [SerializeField,HideInInspector] private SerializedDictionary<ProjectionType, List<LightShowController>> lightShowsActive;
    [SerializeField, HideInInspector] private SerializedDictionary<ProjectionType, int> pooledCount;

    private void BuildDictionaries() {
        transforms = new SerializedDictionary<ProjectionType, Transform> {
            {ProjectionType.Main, MainTransform},
            {ProjectionType.ChurchClockAll, ChurchClockAllTransform},
            {ProjectionType.ChurchClockPositiveX, ChurchClockPositiveXTransform},
            {ProjectionType.ChurchClockNegativeX, ChurchClockNegativeXTransform},
            {ProjectionType.ChurchClockPositiveZ, ChurchClockPositiveZTransform}
        };
        templates = new SerializedDictionary<ProjectionType, LightShowController> {
            {ProjectionType.Main, MainTemplate},
            {ProjectionType.ChurchClockAll, ChurchClockAllTemplate},
            {ProjectionType.ChurchClockPositiveX, ChurchClockPositiveXTemplate},
            {ProjectionType.ChurchClockNegativeX, ChurchClockNegativeXTemplate},
            {ProjectionType.ChurchClockPositiveZ, ChurchClockPositiveZTemplate}
        };
        lightShowsInactive = new SerializedDictionary<ProjectionType, List<LightShowController>> {
            {ProjectionType.Main, new List<LightShowController>()},
            {ProjectionType.ChurchClockAll, new List<LightShowController>()},
            {ProjectionType.ChurchClockPositiveX, new List<LightShowController>()},
            {ProjectionType.ChurchClockNegativeX, new List<LightShowController>()},
            {ProjectionType.ChurchClockPositiveZ, new List<LightShowController>()}
        };
        lightShowsActive = new SerializedDictionary<ProjectionType, List<LightShowController>> {
            {ProjectionType.Main, new List<LightShowController>()},
            {ProjectionType.ChurchClockAll, new List<LightShowController>()},
            {ProjectionType.ChurchClockPositiveX, new List<LightShowController>()},
            {ProjectionType.ChurchClockNegativeX, new List<LightShowController>()},
            {ProjectionType.ChurchClockPositiveZ, new List<LightShowController>()}
        };
        pooledCount = new SerializedDictionary<ProjectionType, int> {
            {ProjectionType.Main, 2},
            {ProjectionType.ChurchClockAll, 2},
            {ProjectionType.ChurchClockPositiveX, 2},
            {ProjectionType.ChurchClockNegativeX, 2},
            {ProjectionType.ChurchClockPositiveZ, 2}
        };
    }

    public void FixReferences() {
        foreach (var projectionType in lightShowsInactive.Keys) {
            lightShowsInactive[projectionType].ForEach(controller => {
                if(controller != null)
                    DestroyImmediate(controller.gameObject);
            });
            lightShowsInactive[projectionType].Clear();
        }
        BuildDictionaries();
    }

    public void ResetPool() {
        if (transforms == null || templates == null || lightShowsInactive == null || lightShowsActive == null) {
            BuildDictionaries();
        }
        
        if (lightShowsActive.SelectMany(pair => pair.Value).Count() != 0) {
            Debug.LogError("Cannot reset Light Show pool while objects are active");
            return;
        }

        foreach (var lightShowController in lightShowsActive.SelectMany(pair => pair.Value)) {
            DestroyImmediate(lightShowController.gameObject);
        }

        foreach (var projectionType in lightShowsInactive.Keys) {
            lightShowsInactive[projectionType].ForEach(controller => DestroyImmediate(controller.gameObject));
            lightShowsInactive[projectionType].Clear();
        }

        pooledCount.Keys.ToList().ForEach(key => {
            pooledCount[key] = 2;
            for (var i = 0; i < pooledCount[key]; i++) {
                lightShowsInactive[key].Add(CreateObject(key));
            }
        });
    }

    public LightShowController GetPooled(LightShowBehaviour requester) {
        ProjectionType type = requester.ProjectionLocation;
        if (pooledCount[type] == 0) ResetPool();
        if (lightShowsInactive[type].Count == 0) {
            var numToCreate = pooledCount[type];
            pooledCount[type] *= 2;
            for (var i = 0; i < numToCreate; i++) {
                lightShowsInactive[type].Add(CreateObject(type));
            }
        }

        var controller = lightShowsInactive[type][0];
        lightShowsInactive[type].RemoveAt(0);
        lightShowsActive[type].Add(controller);
        OnActivate(controller, requester);
        return controller;
    }

    public void ReleasePooled(LightShowController controller, LightShowBehaviour requester) {
        ProjectionType type = requester.ProjectionLocation;
        if (!lightShowsActive[type].Contains(controller)) {
            Debug.LogError("Trying to release LightShowController when it is not active.");
        }

        lightShowsActive[type].Remove(controller);
        lightShowsInactive[type].Add(controller);
        OnDeactivate(controller, requester);
    }

    public LightShowController CreateObject(ProjectionType projectionType) {
        var instance = Instantiate(templates[projectionType], transforms[projectionType].position, transforms[projectionType].rotation, transform);
        instance.ProjectionType = projectionType;
        instance.transform.localScale = transforms[projectionType].localScale;
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