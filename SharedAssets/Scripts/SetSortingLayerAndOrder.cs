using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways]
public class SetSortingLayerAndOrder : MonoBehaviour
{
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] public int sortingOrder;

    public void OnValidate()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = sortingOrder;
    }
}