using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EnterAreaTrigger : MonoBehaviour
{
    public LayerMask playerLayerMask;
    [SerializeField] bool use_once;
    bool used;
    [SerializeField] UnityEvent OnEnterCall;
    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if ((playerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            OnEnterCall?.Invoke();
            if(use_once) used = true;
        }    
    }
}
