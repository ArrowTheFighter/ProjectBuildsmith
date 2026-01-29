using UnityEngine;
using UnityEngine.Events;

public class NPCHammerAnimationEvent : MonoBehaviour
{
    public UnityEvent OnHammerHit;

    [Header("DEBUG")]
    public bool StartPlayingHammerAnimation;

    void Start()
    {
        if(StartPlayingHammerAnimation)
        {
            if(TryGetComponent(out Animator animator))
            {
                animator.Play("HameringRobot");
            }
        }
    }

    public void HammerHitAnimEvent()
    {
        OnHammerHit?.Invoke();
    }


}
