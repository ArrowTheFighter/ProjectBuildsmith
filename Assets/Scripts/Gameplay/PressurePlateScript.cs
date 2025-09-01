using UnityEngine;
using UnityEngine.Events;

public class PressurePlateScript : MonoBehaviour
{
    public UnityEvent ActivatedEvent;
    public UnityEvent DeactivatedEvent;
    bool isActive;
    Collider col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider>();
    }

    void Activated()
    {
        ActivatedEvent?.Invoke();
    }

    void Deactivated()
    {
        DeactivatedEvent?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            print("Player landed on pressure plate");
            if (!isActive)
            {
                Activated();
                isActive = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("Player left the pressure plate");
            if (isActive)
            {
                Deactivated();
                isActive = false;
            }
        }
    }
}
