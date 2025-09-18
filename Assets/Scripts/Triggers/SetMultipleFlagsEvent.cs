using UnityEngine;

public class SetMultipleFlagsEvent : MonoBehaviour
{
    [SerializeField] public string[] flag_id;
    [SerializeField] public bool is_true;

    public void SetFlagsTrue()
    {
        is_true = true;

        for(int i = 0; i < flag_id.Length; i++)
        {
            FlagManager.Set_Flag(flag_id[i], is_true);
        } 
    }
}