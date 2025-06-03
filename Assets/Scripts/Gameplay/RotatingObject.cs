using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [SerializeField] Vector3 Rotation_Vector;
    [SerializeField] float speed_multiplier;
    public bool Running;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Running) return;
        transform.Rotate(Rotation_Vector * speed_multiplier * Time.deltaTime);
    }

    public void SetRunning(bool is_running)
    {
        Running = is_running;
     }
}
