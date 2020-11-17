using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[ExecuteInEditMode]
public class UVMapper : MonoBehaviour {
    public Camera Camera;
    public MeshFilter Source;
    public MeshFilter Target;
    public Vector3 ScaleLeft;
    public Vector3 ScaleRight;
    public bool Auto;

    private Mesh backupMesh;

    public void BackupMesh() {
        backupMesh = Instantiate(Target.sharedMesh);
    }

    public void RestoreBackupMesh() {
        Target.sharedMesh = Instantiate(backupMesh);
    }

    public void CopyMesh() {
        Target.sharedMesh = Instantiate(Source.sharedMesh);
    }

    public void RandomUV() {
        var targetMesh = Target.sharedMesh;
        var uvs = new List<Vector2>();
        targetMesh.GetUVs(0, uvs);
        for (int i = 0; i < targetMesh.uv.Length; i++) {
            uvs[i] = new Vector2(Random.value, Random.value);
        }
        targetMesh.SetUVs(0, uvs);
        Target.sharedMesh = targetMesh;
    }
    
    public void Generate() {
        if (Camera == null || Source == null || Target == null) return;
        var sourceMesh = Source.sharedMesh;
        var sourceVertices = sourceMesh.vertices.ToList();
        var viewportVertices = sourceVertices.Select(vertex => Camera.WorldToViewportPoint((Matrix4x4.Scale(ScaleLeft) * Source.transform.localToWorldMatrix * Matrix4x4.Scale(ScaleRight) * vertex.V3_V4(1)).V4_V3())).ToList();
        var min = float.PositiveInfinity;
        var max = float.NegativeInfinity;
        viewportVertices.ForEach(vertex => {
            if (vertex.z > max) max = vertex.z;
            if (vertex.z < min) min = vertex.z;
        });
        
        var mappedVertices = viewportVertices.Select(vertex => new Vector3(vertex.x, vertex.y, vertex.z.Map(min, max, 1, 0))).ToList();
        // var projectedVertices 
        var targetMesh = Target.sharedMesh;
        var uvs = new List<Vector2>();
        targetMesh.GetUVs(0, uvs);
        for (int i = 0; i < uvs.Count; i++) {
            uvs[i] = new Vector2(viewportVertices[i].x, viewportVertices[i].y);
        }
        targetMesh.SetUVs(0, uvs);
    }

    private void Update() {
        if(Auto) Generate();
    }

    private void OnEnable() {
        EditorApplication.update += Update;
    }

    private void OnDisable() {
        EditorApplication.update -= Update;
    }
}