using UnityEngine;
using System;

public class MovingPlatformRotator : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Vector3 rotationAmount;
    public bool IsActive;

    public event Action OnBeforePlatformMove;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    


    void FixedUpdate()
    {
        if (!IsActive) return;
        
            OnBeforePlatformMove?.Invoke();

            transform.Rotate(rotationAmount);
            //rb.MovePosition(newPos);

    }

    public void SetActive(bool active)
    {
        IsActive = active;
    }

    public Transform getInterfaceTransform()
    {
        return transform;
    }
}
