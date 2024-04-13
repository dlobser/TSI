using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionCamera : MonoBehaviour
{
    public Camera projectorCamera;
    public Material projectionMaterial;
    public Matrix4x4 projectionMatrix;

    void Update()
    {
        projectionMatrix = GL.GetGPUProjectionMatrix(projectorCamera.projectionMatrix, false) * projectorCamera.worldToCameraMatrix;
        projectionMaterial.SetMatrix("_ProjMatrix", projectionMatrix);
    }
}
