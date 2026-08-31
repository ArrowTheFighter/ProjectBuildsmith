using DG.Tweening;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public Material MaterialToSwapTo;
    

    [ContextMenu("SwapMats")]
    public void SwapMaterials()
    {
        MeshRenderer.material = MaterialToSwapTo;
    }
}
