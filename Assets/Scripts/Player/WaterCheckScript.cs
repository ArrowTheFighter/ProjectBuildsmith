using UnityEngine;

public class WaterCheckScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Water")
        {
            print("Hit water");
        }
    }
}
