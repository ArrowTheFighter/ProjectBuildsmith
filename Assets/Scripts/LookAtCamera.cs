using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform Camera_Transform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Camera_Transform == null)
        {
            Camera_Transform = FindFirstObjectByType<Camera>().transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward =  transform.position - Camera_Transform.position;
    }
}
