using System;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour, IMoveingPlatform
{
    public event Action OnBeforePlatformMove;

    public Transform ParentTransform;

    public Transform getInterfaceTransform()
    {
        return ParentTransform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ParentTransform == null)
        {
            if (transform.parent != null)
            {
                ParentTransform = transform.parent;
            }
        }
        foreach (var componenet in ParentTransform.gameObject.GetComponents<MonoBehaviour>())
        {
            if (componenet is IMoveingPlatform moveingPlatform)
            {
                moveingPlatform.OnBeforePlatformMove += SendBeforeMoveEvent;
            }
        }
        ScriptRefrenceSingleton.instance.gameplayUtils.OnStartMoveToMainMenu += UnsubscribeFromPlatformEvents;
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
        OnBeforePlatformMove?.Invoke();
    }

}
