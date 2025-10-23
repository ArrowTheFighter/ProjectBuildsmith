using UnityEngine;

public class SetFlagScript : MonoBehaviour
{
    public string defualt_flag;
    public void SetFlag(string flag_id)
    {
        FlagManager.Set_Flag(flag_id);
    }
     
     public void SetDefaultFlag()
    {
        FlagManager.Set_Flag(defualt_flag);
    }
    
        
    
}
