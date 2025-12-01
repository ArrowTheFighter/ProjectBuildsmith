using UnityEngine;
using UnityEngine.Events;

public class ObjectPositionCheck : MonoBehaviour
{
    [Header("Object to check")]
    public Transform ObjectToCheck;

    [Header("X Position")]
    public bool Check_X;
    public float X_Pos;

    [Header("Y Position")]
    public bool Check_Y;
    public float Y_Pos;

    [Header("Z Position")]
    public bool Check_Z;
    public float Z_Pos;

    [Header("Events")]
    public UnityEvent IsPastPositionEvent;
    public UnityEvent IsUnderPositionEvent;


    public void Activate()
    {
        if (Check_X)
            if (ObjectToCheck.position.x < X_Pos)
            {
                RunIsUnderPositionEvent();
                return;
            } 
            

        if (Check_Y) 
            if (ObjectToCheck.position.y < Y_Pos)
            {
                RunIsUnderPositionEvent();
                return;
            }

        if (Check_Z) 
            if (ObjectToCheck.position.z < Z_Pos)
            {
                RunIsUnderPositionEvent();
                return;
            }

        RunIsPastPositionEvent();
    }

    void RunIsUnderPositionEvent()
    {
        IsUnderPositionEvent?.Invoke();  
    }

    void RunIsPastPositionEvent()
    {
        IsPastPositionEvent?.Invoke();
    }
}
