using UnityEngine;

[ExecuteAlways]
public class PropBlocks_GradientGraph : MonoBehaviour
{
    public Renderer renderer;
    // Original values
    public float Multiply = 1.0f;
    public float Gamma = 1.0f;
    public float Add = 0.0f;
    public Color ColorInner = Color.white;
    public Color ColorOuter = Color.white;
    [Range(0.0f, 1.0f)]
    public float RingMix = 1.0f;
    [Range(0.0f, 1.0f)]
    public float RingsMix = 0.0f;
    [Range(0.0f, 1.0f)]
    public float RingThickness = 0.2f;
    public float RingBrightness = 1.0f;
    public float RingSize = 1.0f;
    public float RingEdgeFade = 0.5f;
    [Range(0.0f, 1.0f)]
    public float UVToPosition = 0.2f;
    public Vector3 positionOffset = Vector3.zero;

    [Range(0.0f, 1.0f)]
    public float CircleOrSquare = 0.0f;

    // Duplicate values for Graph control
    public float GraphMultiply = 0.0f;
    public float GraphGamma = 0.0f;
    public float GraphAdd = 0.0f;
    public Color GraphColorInner = Color.black;
    public Color GraphColorOuter = Color.black;
    [Range(0.0f, 1.0f)]
    public float GraphRingMix = 0.0f;
    [Range(0.0f, 1.0f)]
    public float GraphRingsMix = 0.0f;
    [Range(0.0f, 1.0f)]
    public float GraphRingThickness = 0.0f;
    public float GraphRingBrightness = 0.0f;
    public float GraphRingSize = 0.0f;
    public float GraphRingEdgeFade = 0.0f;
    [Range(0.0f, 1.0f)]
    public float GraphUVToPosition = 0.0f;
    public Vector3 GraphPositionOffset = Vector3.zero;

    [Range(0.0f, 1.0f)]
    public float GraphCircleOrSquare = 0.0f;
    

    private MaterialPropertyBlock propBlock;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
    }

    void OnEnable(){
        SetProperties();
    }

    void SetProperties(){
        if (renderer == null)
            return;
        if(propBlock == null)
            propBlock = new MaterialPropertyBlock();

        renderer.GetPropertyBlock(propBlock);

        // Combine the original and graph values
        propBlock.SetFloat("_Multiply", Multiply + GraphMultiply);
        propBlock.SetFloat("_Gamma", Gamma + GraphGamma);
        propBlock.SetFloat("_Add", Add + GraphAdd);
        propBlock.SetColor("_ColorInner", ColorInner + GraphColorInner);
        propBlock.SetColor("_ColorOuter", ColorOuter + GraphColorOuter);
        propBlock.SetFloat("_RingMix", RingMix + GraphRingMix);
        propBlock.SetFloat("_RingsMix", RingsMix + GraphRingsMix);
        propBlock.SetFloat("_RingThickness", 1/(RingThickness + GraphRingThickness));
        propBlock.SetFloat("_RingBrightness", RingBrightness + GraphRingBrightness);
        propBlock.SetFloat("_RingSize", RingSize + GraphRingSize);
        propBlock.SetFloat("_RingEdgeFade", RingEdgeFade + GraphRingEdgeFade);
        propBlock.SetFloat("_UVToPosition", UVToPosition + GraphUVToPosition);
        propBlock.SetVector("_PositionOffset", positionOffset + GraphPositionOffset);
        propBlock.SetFloat("_CircleOrSquare", CircleOrSquare + GraphCircleOrSquare);

        renderer.SetPropertyBlock(propBlock);
    }

    void Update()
    {
        SetProperties();
    }
}
