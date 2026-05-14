using UnityEngine;
using System;
using System.Collections.Generic;

public class FloatingObject : MonoBehaviour, IMoveingPlatform , IEnableDisable,ISaveable
{
    [SerializeField] Vector3 MoveTo;
    [SerializeField] Vector3 rotationAmount;
    [SerializeField] float duration;
    [SerializeField] float delay;
    [SerializeField] bool AutoAddChildComponenets = true;
    [SerializeField] AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool IsActive;
    bool disabled;

    Vector3 startPos;
    Vector3 endPos;
    float elapsed;
    public bool reverse;

    private readonly HashSet<IPlatformPassenger> passengers = new();

    public event Action OnBeforePlatformMove;

    public Transform obj_transform;
    public MeshFilter visualMesh;

    [Header("Debug")]
    public bool PrintDebug;

    [SerializeField] int unique_id;
    public int Get_Unique_ID { get => unique_id; set => unique_id = value; }

    public bool Get_Should_Save => IsActive;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elapsed = delay;
        if(AutoAddChildComponenets)
        {
            Collider[] childColliders = GetComponentsInChildren<Collider>();

            foreach (var collider in childColliders)
            {
                if (collider.transform == transform) continue; 
                if (collider.isTrigger) continue;
                GameObject childObj = collider.gameObject;
                MovingPlatformChild movingPlatformChild = childObj.AddComponent<MovingPlatformChild>();

                movingPlatformChild.ResetComponenet();
                movingPlatformChild.ParentTransform = transform;
                movingPlatformChild.SetupComponenet();
            }
        }

        if (obj_transform == null) obj_transform = transform;
        startPos = obj_transform.position;
        endPos = startPos + MoveTo;

        //transform.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        // if (rb != null)
        // {
        //     startPos = transform.position;
        //     rb.DOMove(transform.position + MoveTo, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetUpdate(UpdateType.Fixed);
        // }
    }

    void FixedUpdate()
    {
        if (disabled) return;
        if (!IsActive) return;
        if (elapsed < duration)
        {
            foreach (var passenger in passengers)
            {
                passenger.BeforePlatformMove(this);
            }
            OnBeforePlatformMove?.Invoke();
            elapsed += Time.fixedDeltaTime;
        

            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = easing.Evaluate(t);
            Vector3 newPos = Vector3.Lerp(reverse ? startPos : endPos, reverse ? endPos : startPos, easedT);
            obj_transform.position = newPos;
            transform.Rotate(rotationAmount);
            //rb.MovePosition(newPos);

        }
        else
        {
            reverse = !reverse;
            elapsed = 0f;
        }
        

    }

    public void SetActive(bool active)
    {
        IsActive = active;
     }

    public void SetPositionAsStart()
    {
        startPos = transform.position;
        endPos = startPos + MoveTo;
        elapsed = 0f;
        reverse = true;
    }

    void OnDrawGizmosSelected()
    {
        Transform checkTransform = obj_transform;
        if (visualMesh != null) checkTransform = visualMesh.transform;
        if (checkTransform == null) checkTransform = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(checkTransform.position, checkTransform.position + MoveTo);
        Gizmos.DrawSphere(checkTransform.position + MoveTo, 0.2f);
        if (checkTransform.TryGetComponent(out MeshFilter meshFilter))
        {
            if (meshFilter.sharedMesh != null)
            {
                Gizmos.DrawWireMesh(meshFilter.sharedMesh, checkTransform.position + MoveTo, checkTransform.rotation, checkTransform.lossyScale);
             }
         }
    }

    public Transform getInterfaceTransform()
    {
        return obj_transform;
    }

    void OnCollisionEnter(Collision collision)
    {
        print("moving platform Collided with object");
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            print("adding object to moving platform passengers");
            passengers.Add(passenger.GetPassengerScript());
        }
            
    }

    void OnCollisionExit(Collision collision)
    {
        print("moving platform Left collision with object");
        if (collision.collider.TryGetComponent(out IPlatformPassenger passenger))
        {
            print("removing object from moving platform passengers");
            passengers.Remove(passenger.GetPassengerScript());
            
        }
    }

    public void EnableInterface()
    {
        disabled = false;
    }

    public void DisableInterface()
    {
        disabled = true;
    }

    public void SaveLoaded(SaveFileStruct saveFileStruct)
    {
        SetActive(true);
    }
}
