using UnityEngine;

public class MoveObjectToPosition : MonoBehaviour
{
    public Transform objectToMove;
    public Vector3 posToMoveTo;

    public void MoveObject()
    {
        objectToMove.position = posToMoveTo;
    }
}
