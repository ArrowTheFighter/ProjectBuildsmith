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
            print("call child beforePlatformMove");
            passenger.BeforePlatformMove(this);
        }
        OnBeforePlatformMove?.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        print("moving platform child Collided with object");
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            print("adding object to moving platform child passengers");
            passengers.Add(passenger.GetPassengerScript());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        print("moving platform child Left collision with object");
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            print("removing object from moving platform child passengers");

            passengers.Remove(passenger.GetPassengerScript());
        }
    }

}
