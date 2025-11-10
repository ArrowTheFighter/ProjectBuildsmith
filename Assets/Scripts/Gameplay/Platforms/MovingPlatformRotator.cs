using UnityEngine;
using System;

public class MovingPlatformRotator : MonoBehaviour, IMoveingPlatform
{
    [SerializeField] Vector3 rotationAmount;
    public bool IsActive;

    public event Action OnBeforePlatformMove;
    [SerializeField] bool AutoAddChildComponenets = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (AutoAddChildComponenets)
        {
            Collider[] childColliders = GetComponentsInChildren<Collider>();

            foreach (var collider in childColliders)
            {
                if (collider.transform == transform) continue;
                print($"adding movingplatformchild componenet to {collider.gameObject.name}");
                GameObject childObj = collider.gameObject;
                MovingPlatformChild movingPlatformChild = childObj.AddComponent<MovingPlatformChild>();

                //movingPlatformChild.ResetComponenet();
                movingPlatformChild.ParentTransform = transform;
                movingPlatformChild.SetupComponenet();
            }
        }
    }


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
