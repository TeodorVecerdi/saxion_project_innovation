using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CameraProjection : MonoBehaviour {
    public Camera Camera;
    public MeshFilter MeshFilterSource;
    public MeshFilter MeshFilterTarget;

    public bool Enabled;
    public List<MatrixMul> Multiplications;

    private void OnEnable() {
        EditorApplication.update += Generate;
    }

    private void OnDisable() {
        EditorApplication.update -= Generate;
    }

    public void Generate() {
        if (!Enabled) return;
        var mesh = Instantiate(MeshFilterSource.sharedMesh);
        var vertexList = new List<Vector3>();
        mesh.GetVertices(vertexList);

        var matrix = Matrix4x4.identity;
        Multiplications.ForEach(mul => matrix *= Convert(mul));
        for (var i = 0; i < vertexList.Count; i++) {
            vertexList[i] = ((matrix * vertexList[i].V3_V4(1))).V4_V3();
        }

        mesh.SetVertices(vertexList);
        mesh.RecalculateNormals();
        if (MeshFilterTarget.sharedMesh != null) DestroyImmediate(MeshFilterTarget.sharedMesh);
        MeshFilterTarget.sharedMesh = mesh;
    }

    private Matrix4x4 Convert(MatrixMul mul) {
        switch (mul) {
            case MatrixMul.Identity:
                return Matrix4x4.identity;
            case MatrixMul.Projection:
                return Camera.projectionMatrix;
            case MatrixMul.ProjectionInverse:
                return Camera.projectionMatrix.inverse;
            case MatrixMul.View:
                return Camera.previousViewProjectionMatrix * Camera.projectionMatrix.inverse;
            case MatrixMul.ViewInverse:
                return (Camera.previousViewProjectionMatrix * Camera.projectionMatrix.inverse).inverse;
            case MatrixMul.CameraLocalToWorld:
                return Camera.transform.localToWorldMatrix;
            case MatrixMul.SourceLocalToWorld:
                return MeshFilterSource.transform.localToWorldMatrix;
            case MatrixMul.CameraWorldToLocal:
                return Camera.transform.worldToLocalMatrix;
            case MatrixMul.SourceWorldToLocal:
                return MeshFilterSource.transform.worldToLocalMatrix;
            default:
                throw new ArgumentOutOfRangeException(nameof(mul), mul, null);
        }
    }
}

public enum MatrixMul {
    Identity,
    Projection,
    ProjectionInverse,
    View,
    ViewInverse,
    CameraLocalToWorld,
    SourceLocalToWorld,
    CameraWorldToLocal,
    SourceWorldToLocal,
}