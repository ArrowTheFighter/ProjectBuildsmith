using UnityEngine;

public class WaterCheckScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        print("Entered collision");
        if(other.tag == "Water")
        {
            print("Hit water");
        }
    }
}
