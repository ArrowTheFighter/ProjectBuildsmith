using UnityEngine;

public class PlayAnimationClip : MonoBehaviour
{
    public Animator animator;
    public string defaultAnimName;
    public float TransitionTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public void PlayClip(string clip_name)
    {
        if(TransitionTime < 0)
        {
            animator.Play(clip_name);
        }else
        {
            animator.CrossFade(clip_name,TransitionTime);
        }
    }

    public void PlayDefaultClip()
    {
        PlayClip(defaultAnimName);
    }

    public void SkipDefaultClip()
    {
        animator.Play(defaultAnimName,-1,1);
    }
}
