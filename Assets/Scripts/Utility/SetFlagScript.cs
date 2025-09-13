using UnityEngine;

public class SetFlagScript : MonoBehaviour
{
    public void SetFlag(string flag_id)
    {
        FlagManager.Set_Flag(flag_id);
     }
}
