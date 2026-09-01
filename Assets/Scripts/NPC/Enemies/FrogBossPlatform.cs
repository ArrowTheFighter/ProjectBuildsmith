using UnityEngine;

public class FrogBossPlatform : MonoBehaviour
{
    public float topOffset;

    public Vector3 getTopPos()
    {
        return transform.position + Vector3.up * topOffset;
    }
    
}
