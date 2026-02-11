using UnityEngine;
using UnityEngine.Events;

public class PlayerDieInZone : MonoBehaviour
{
    Transform playerTransform;
    PlayerHealth playerHealth;

    public UnityEvent PlayerDiedInZoneUnityEvent;

    void PlayerTookDamageInZone(float health)
    {
        if(health <= 0)
        {
            Debug.Log("Player died in zone",gameObject);
            PlayerDiedInZoneUnityEvent?.Invoke();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.TryGetComponent(out IPlatformPassenger component))
        {
            playerTransform = ((Component)component.GetPassengerScript()).transform;
            playerHealth = playerTransform.GetComponent<PlayerHealth>();
            playerHealth.OnTakeDamage += PlayerTookDamageInZone;
        }
    }


    void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out IPlatformPassenger component))
        {
            if(((Component)component.GetPassengerScript()).transform == playerTransform)
            {
                playerHealth.OnTakeDamage -= PlayerTookDamageInZone;
                playerHealth = null;
                playerTransform = null;
            }
        }
    }
}
