using UnityEngine;

public class PlayerWaterCheck : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 3, Vector3.down, 2f, layerMask))
        {
            print("In water");
            GetComponent<PlayerHealth>().Respawn();
        }
    }
}
