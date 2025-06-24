using UnityEngine;

public class PlayerOrientation : MonoBehaviour
{
    [SerializeField] Transform CameraTransform;
    [SerializeField] Transform PlayerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardDir = PlayerTransform.position - CameraTransform.position;
        forwardDir.y = 0;
        transform.forward = forwardDir;
    }
}
