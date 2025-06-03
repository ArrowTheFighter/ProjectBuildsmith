using UnityEngine;

public class SawbladeHologramActivate : MonoBehaviour
{
    [SerializeField] LayerMask player_layer;

    public GameObject sawbladeHologram;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter(Collider other)
    {
        //Checks to see if the player enters a pickup's trigger
        //Checks to see what type of resource it is and increases the amount of it
        //Destroys the pickup
        if ((player_layer.value & (1 << other.gameObject.layer)) != 0)
        {
            sawbladeHologram.SetActive(true);
        }
    }
}
