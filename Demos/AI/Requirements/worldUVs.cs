using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class worldUVs : MonoBehaviour
{
    public Transform target;
    public string path = "Assets";
    public string name = "BakedUVMesh";
    Camera cam;
    public bool saveMeshes;
    public bool mergeMeshes = true; // Flag to control whether to merge meshes or not

    List<CombineInstance> combineInstances = new List<CombineInstance>();

    void Start()
    {
        cam = Camera.main;
        cam.aspect = 1;
        Recurse(target, 0);

        if (mergeMeshes)
        {
            CombineAllMeshes();
        }
    }

    void Recurse(Transform t, int overflow)
    {
        overflow++;
        if (overflow < 1e5)
        {
            MeshFilter meshFilter = t.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                ProjectUV(meshFilter);
                if (mergeMeshes)
                {
                    AddMeshToCombine(meshFilter);
                }
                else
                {
                    SaveIndividualMesh(meshFilter);
                }
            }
            for (int i = 0; i < t.childCount; i++)
            {
                Recurse(t.GetChild(i), overflow);
            }
        }
    }

    void ProjectUV(MeshFilter meshFilter)
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPoint = meshFilter.transform.TransformPoint(vertices[i]);
            Vector3 screenPos = cam.WorldToScreenPoint(worldPoint);
            uvs[i] = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        }
        mesh.uv = uvs;
    }

    void AddMeshToCombine(MeshFilter meshFilter)
    {
        CombineInstance combine = new CombineInstance();
        combine.mesh = meshFilter.mesh;
        combine.transform = meshFilter.transform.localToWorldMatrix;
        combineInstances.Add(combine);
    }

    void SaveIndividualMesh(MeshFilter meshFilter)
    {
        if (saveMeshes)
        {
            Mesh clone = new Mesh();
            clone.name = meshFilter.name;
            clone.vertices = meshFilter.mesh.vertices;
            clone.triangles = meshFilter.mesh.triangles;
            clone.normals = meshFilter.mesh.normals;
            clone.uv = meshFilter.mesh.uv;

            string individualPath = $"{path}/{clone.name}_{Time.time}.asset";
            #if UNITY_EDITOR
            AssetDatabase.CreateAsset(clone, individualPath);
            Debug.Log($"Saved individual mesh at: {individualPath}");
            AssetDatabase.SaveAssets();
            #endif
        }
    }

    void CombineAllMeshes()
    {
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        finalMesh.RecalculateBounds();

        #if UNITY_EDITOR
        if (saveMeshes)
        {
            string finalPath = $"{path}/{name}.asset";
            AssetDatabase.CreateAsset(finalMesh, finalPath);
            Debug.Log($"Saved combined mesh at: {finalPath}");
            AssetDatabase.SaveAssets();
        }
        #endif
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
            Debug.Log("target is " + screenPos.x / Screen.width + " pixels from the left");
        }
    }
}
