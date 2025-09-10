using UnityEngine;

public class TriggerFlagArea : MonoBehaviour
{
    [SerializeField] public string flag_id;
    [SerializeField] public bool is_true;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            is_true = true;
            FlagManager.Set_Flag(flag_id, is_true);
        }
    }
}