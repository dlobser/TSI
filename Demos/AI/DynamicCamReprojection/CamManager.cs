using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CamManager : MonoBehaviour
{
    public int numRings;
    public int numAngles;
    public float minHeight;
    public float maxHeight;
    public float minRadius;
    public float maxRadius;
    public GameObject cameraPrefab;
    public Material mat;
    public ProjectionCamera projectionCamera;
    public bool updateCamInUpdate = false;

    private Dictionary<Vector3Int, List<Camera>> cameraGrid;
    public float gridCellSize = 10f; // Adjust based on your scene requirements

    void Start()
    {
        cameraGrid = new Dictionary<Vector3Int, List<Camera>>();
        GenerateCameras();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            RenderAndSaveImages();
        }
        if (Input.GetKeyDown(KeyCode.L) || updateCamInUpdate)
        {
            LoadTextureFromClosestCamera();
        }
    }

    void GenerateCameras()
    {
        float heightStep = (maxHeight - minHeight) / (numRings - 1);
        float radiusStep = (maxRadius - minRadius) / (numRings - 1);
        int name = 0;

        for (int ring = 0; ring < numRings; ring++)
        {
            float height = minHeight + ring * heightStep;
            float currentRadius = minRadius + ring * radiusStep;
            for (int angle = 0; angle < numAngles; angle++)
            {
                float theta = (angle / (float)numAngles) * 2 * Mathf.PI;
                Vector3 position = new Vector3(Mathf.Cos(theta) * currentRadius, height, Mathf.Sin(theta) * currentRadius);
                Quaternion rotation = Quaternion.LookRotation(-position.normalized, Vector3.up);
                GameObject camObj = Instantiate(cameraPrefab, position, rotation);
                camObj.name = "camera_" + name.ToString("0000");
                name++;
                AddCameraToGrid(camObj.GetComponent<Camera>());
            }
        }
    }

    void AddCameraToGrid(Camera camera)
    {
        Vector3Int gridKey = PositionToGridKey(camera.transform.position);
        if (!cameraGrid.ContainsKey(gridKey))
        {
            cameraGrid[gridKey] = new List<Camera>();
        }
        cameraGrid[gridKey].Add(camera);
    }

    Vector3Int PositionToGridKey(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / gridCellSize),
            Mathf.FloorToInt(position.y / gridCellSize),
            Mathf.FloorToInt(position.z / gridCellSize)
        );
    }

    public void RenderAndSaveImages()
    {
        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            RenderTexture rt = new RenderTexture(512, 512, 24);
            cam.targetTexture = rt;
            RenderTexture.active = rt;
            cam.Render();

            Texture2D image = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            image.Apply();

            byte[] bytes = image.EncodeToPNG();
            string filename = $"{cam.name}.png";
            File.WriteAllBytes(Path.Combine(Application.dataPath, "Resources", filename), bytes);

            Destroy(image);
            cam.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
        }
    }

    public void LoadTextureFromClosestCamera()
    {
        Camera mainCamera = Camera.main;
        Vector3Int gridKey = PositionToGridKey(mainCamera.transform.position);

        Camera closestCamera = FindClosestCameraInGrid(gridKey, mainCamera.transform.position);
        if (closestCamera != null)
        {
            closestCamera.aspect = 1f;
            projectionCamera.projectorCamera = closestCamera;
            projectionCamera.UpdateCam();
            string path = closestCamera.name; // Assuming texture names are based on camera names
            Texture2D texture = Resources.Load<Texture2D>(path);
            mat.mainTexture = texture;
        }
        else
        {
            Debug.Log("No close camera found in the grid cell.");
        }
    }

    Camera FindClosestCameraInGrid(Vector3Int gridKey, Vector3 position)
    {
        Camera closestCamera = null;
        float minDistance = float.MaxValue;

        if (cameraGrid.TryGetValue(gridKey, out List<Camera> cameras))
        {
            foreach (Camera cam in cameras)
            {
                float distance = (cam.transform.position - position).sqrMagnitude;
                if (distance < minDistance)
                {
                    closestCamera = cam;
                    minDistance = distance;
                }
            }
        }

        return closestCamera;
    }
}
