using UnityEngine;

public class SetNearPlayerMatPos : MonoBehaviour
{
    public Material targetMaterial; // Drag your material here

    void Update()
    {
        // Passes this object's current 3D position to the shader every frame
        targetMaterial.SetVector("_TargetPosition", transform.position);
    }
}
