using UnityEngine;
using UnityEngine.Events;

public class NPCHammerAnimationEvent : MonoBehaviour
{
    public UnityEvent OnHammerHit;

    Animator animator;

    [Header("DEBUG")]
    public bool StartPlayingHammerAnimation;

    void Start()
    {
        animator = GetComponent<Animator>();
        if(StartPlayingHammerAnimation)
        {
            
            animator.Play("HameringRobot");
        }
    }

    public void HammerHitAnimEvent()
    {
        if(animator.IsInTransition(0))
            return;
        OnHammerHit?.Invoke();
    }


}
