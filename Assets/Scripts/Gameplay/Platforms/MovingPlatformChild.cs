using System;
using UnityEngine;

public class MovingPlatformChild : MonoBehaviour, IMoveingPlatform
{
    public event Action<Vector3> OnPlatformMove;
    public event Action OnBeforePlatformMove;
    public event Action OnAfterPlatformMove;

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
                moveingPlatform.OnAfterPlatformMove += SendAfterMoveEvent;
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
                moveingPlatform.OnAfterPlatformMove -= SendAfterMoveEvent;
            }
        }
        
    }

    void SendBeforeMoveEvent()
    {
        OnBeforePlatformMove?.Invoke();
    }
    
    void SendAfterMoveEvent()
    {
        OnAfterPlatformMove?.Invoke();
    }

}
