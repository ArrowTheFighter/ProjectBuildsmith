using UnityEngine;

public class MoveObjectToPosition : MonoBehaviour
{
    public Transform objectToMove;
    public Vector3 posToMoveTo;
    public Transform targetTransform;

    public void MoveObject()
    {
        if(targetTransform == null)
            objectToMove.position = posToMoveTo;
        else    
            objectToMove.position = targetTransform.position;
    }
}
