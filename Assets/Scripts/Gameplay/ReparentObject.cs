using UnityEngine;

public class ReparentObject : MonoBehaviour
{
    public Transform ObjectToReparent;
    public Transform NewParent;

    public void ReparentToNewParent()
    {
        ObjectToReparent.SetParent(NewParent);
    }

    public void ResetObjectParent()
    {
        ObjectToReparent.SetParent(null);
    }
}
