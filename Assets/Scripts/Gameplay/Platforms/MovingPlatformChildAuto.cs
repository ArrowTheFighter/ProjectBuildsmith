using System;
using UnityEngine;

public class MovingPlatformChildAuto : MonoBehaviour
{
   
    public Transform MainMovingPlatformTransform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MainMovingPlatformTransform == null)
        {
            MainMovingPlatformTransform = transform;
        }

        Collider[] childColliders = GetComponentsInChildren<Collider>();

        foreach(var collider in childColliders)
        {
            //if (collider.transform == transform) continue;
            GameObject childObj = collider.gameObject;
            MovingPlatformChild movingPlatformChild = childObj.AddComponent<MovingPlatformChild>();

            movingPlatformChild.ResetComponenet();
            movingPlatformChild.ParentTransform = MainMovingPlatformTransform;
            movingPlatformChild.SetupComponenet();
        }

    }

    


}
