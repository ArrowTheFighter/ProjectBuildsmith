using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour, IMoveingPlatform
{
    private readonly HashSet<IPlatformPassenger> passengers = new();
    public event Action OnBeforePlatformMove;

    public Transform ParentTransform;

    public Transform getInterfaceTransform()
    {
        return ParentTransform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetupComponenet()
    {
        if (ParentTransform == null)
        {
            if (transform.parent != null)
            {
                ParentTransform = transform.parent;
            }
        }
        AddEventConnection();
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnsubscribeFromPlatformEvents;
        
        //Set the platform to be on the NotSafe Layer
        if(gameObject.layer == 0)
            gameObject.layer = 13;
    }

    public void ResetComponenet()
    {
        if (ParentTransform == null) return;
        foreach (var componenet in ParentTransform.gameObject.GetComponents<MonoBehaviour>())
        {
            if (componenet is IMoveingPlatform moveingPlatform)
            {
                moveingPlatform.OnBeforePlatformMove -= SendBeforeMoveEvent;
            }
        }
    }

    public void AddEventConnection()
    {
        foreach (var componenet in ParentTransform.gameObject.GetComponents<MonoBehaviour>())
        {
            if (componenet is IMoveingPlatform moveingPlatform)
            {
                moveingPlatform.OnBeforePlatformMove += SendBeforeMoveEvent;
            }
        }
    }


    void UnsubscribeFromPlatformEvents()
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu -= UnsubscribeFromPlatformEvents;
        foreach(var componenet in ParentTransform.gameObject.GetComponents<MonoBehaviour>())
        {
            if(componenet is IMoveingPlatform moveingPlatform)
            {
                
                moveingPlatform.OnBeforePlatformMove -= SendBeforeMoveEvent;
            }
        }
        
    }

    void SendBeforeMoveEvent()
    {
        foreach (var passenger in passengers)
        {
            passenger.BeforePlatformMove(this);
        }
        OnBeforePlatformMove?.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            passengers.Add(passenger.GetPassengerScript());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            passengers.Remove(passenger.GetPassengerScript());
        }
    }

}
