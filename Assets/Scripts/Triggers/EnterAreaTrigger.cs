using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnterAreaTrigger : MonoBehaviour
{
    public bool Enabled = true;
    public LayerMask playerLayerMask;
    [SerializeField] bool use_once;
    bool used;
    [SerializeField] UnityEvent OnEnterCall;

    void OnTriggerEnter(Collider other)
    {
        if(!Enabled) return;
        if (used) return;
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            OnEnterCall?.Invoke();
            if(use_once) used = true;
        }    
    }

    public void ResetTrigger()
    {
        used = false;
    }

    public void SetEnabled(bool _enabled)
    {
        Enabled = _enabled;
    }
}
