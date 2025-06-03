using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] PlayerAudio playerAudio;
    public void FootstepSound(AnimationEvent evt)
    {
        if(IsHeaviestAnimClip(evt.animatorClipInfo.clip))
        {
            playerAudio.PlayClip(1,0.5f);
        }
    }

    bool IsHeaviestAnimClip(AnimationClip currentClip) {
    var currentAnimatorClipInfo = GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
    float highestWeight = 0f;
    AnimationClip highestWeightClip = null;
            
    // Find the clip with the highest weight
    foreach (var clipInfo in currentAnimatorClipInfo) {
        if (clipInfo.weight > highestWeight) {
            highestWeight = clipInfo.weight;
            highestWeightClip = clipInfo.clip;
        }
    }
            
    return highestWeightClip != null && currentClip == highestWeightClip;
}
}


