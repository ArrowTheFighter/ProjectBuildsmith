using UnityEngine;

public class ScaleRopeToElevator : MonoBehaviour
{
    public Transform targetTransform;
   
    void Update()
    {
        float distance = transform.position.y - targetTransform.position.y;
        transform.localScale = new Vector3(transform.localScale.x, 1 * distance,transform.localScale.z);
    }
}
