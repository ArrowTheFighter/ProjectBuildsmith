using UnityEngine;

public class TreeFallScript : MonoBehaviour,ISkippable
{

    public void PlayTreeFallAnimation()
    {
        Animation animation = GetComponent<Animation>();
        animation.Play();
     }

    public void Skip()
    {
        Animation animation = GetComponent<Animation>();
        animation.Play();
        AnimationState state = animation[animation.clip.name];
        state.normalizedTime = 1f; // Jump to the end (1 = 100%)
        animation.Sample();
    }
}
