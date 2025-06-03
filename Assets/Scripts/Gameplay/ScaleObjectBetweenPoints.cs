using UnityEngine;

public class ScaleObjectBetweenPoints : MonoBehaviour
{
    [SerializeField] Transform start_pos;
    [SerializeField] Transform end_pos;
    [SerializeField] Transform object_to_scale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = start_pos.position - end_pos.position;
        object_to_scale.forward = dir;
        float distance = Vector3.Distance(start_pos.position, end_pos.position);
        object_to_scale.localScale = new Vector3(1, 1, 1 * distance);
        object_to_scale.position = Vector3.Lerp(start_pos.position, end_pos.position, 0.5f);
    }
}
