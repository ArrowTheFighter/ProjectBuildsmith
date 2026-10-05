using UnityEngine;

public class DisableSavingArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ScriptRefrenceSingleton.instance.saveLoadManager.canSave = false;
            Debug.Log("Saving is now disabled");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ScriptRefrenceSingleton.instance.saveLoadManager.canSave = true;
            Debug.Log("Saving is enabled again");
        }
    }
}
