using UnityEngine;
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
    // public float radius = 5f;
    public Material mat;
    public ProjectionCamera projectionCamera;

    public bool updateCamInUpdate = false;

    void Start()
    {
        GenerateCameras();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            RenderAndSaveImages();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadTextureFromClosestCamera();
        }
        if (updateCamInUpdate)
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
            }
        }
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
        Camera closestCamera = null;
        float minDistance = float.MaxValue;

        foreach (Camera cam in FindObjectsOfType<Camera>())
        {
            if (cam == mainCamera) continue;
            float distance = (cam.transform.position - mainCamera.transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                closestCamera = cam;
                minDistance = distance;
            }
        }

        if (closestCamera != null)
        {
            projectionCamera.projectorCamera = closestCamera;
            projectionCamera.UpdateCam();
            string path = closestCamera.name;// $"camera_{closestCamera.transform.position.y:F1}_{Vector3.Angle(Vector3.forward, closestCamera.transform.forward):F1}.png";
            Texture2D texture = Resources.Load<Texture2D>(path);
            // Assuming there's a renderer to apply this texture to
            // Renderer renderer = GetComponent<Renderer>();
            mat.mainTexture = texture;
        }
    }
}
