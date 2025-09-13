using UnityEngine;
using UnityEngine.Events;

public class ActiveIfFlagIsSet : MonoBehaviour
{
    public string flag_id;
    public bool CheckForFlagBeingTrue = true;
    public UnityEvent unityEvent;
    
    public void Activate()
    {
        if (FlagManager.Get_Flag_Value(flag_id) == CheckForFlagBeingTrue)
        {
            unityEvent?.Invoke();
         }
    }
}
