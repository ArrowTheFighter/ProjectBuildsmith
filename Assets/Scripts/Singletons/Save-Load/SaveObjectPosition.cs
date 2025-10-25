using UnityEngine;

public class SaveObjectPosition : MonoBehaviour
{
    public int SaveObjectID;

    void Start()
    {
        ScriptRefrenceSingleton.instance.saveLoadManager.saveObjectPositions.Add(this);
    }
}
