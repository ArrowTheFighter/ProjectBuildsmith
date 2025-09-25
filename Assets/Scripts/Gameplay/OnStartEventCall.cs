using UnityEngine;
using UnityEngine.Events;

public class OnStartEventCall : MonoBehaviour
{
    public UnityEvent EventCall;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventCall?.Invoke();
    }

}
