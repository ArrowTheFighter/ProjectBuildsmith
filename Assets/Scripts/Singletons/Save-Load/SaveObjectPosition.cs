using UnityEngine;

public class SaveObjectPosition : MonoBehaviour
{
    public int SaveObjectID;

    void Start()
    {
        SaveLoadManager.instance.saveObjectPositions.Add(this);
    }
}
