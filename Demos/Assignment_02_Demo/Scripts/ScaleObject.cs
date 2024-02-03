using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    public float speed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        float scale = (Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f * (maxScale - minScale) + minScale;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
