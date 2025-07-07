using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] PlayerAudio playerAudio;
    [SerializeField] CharacterMovement characterMovement;
    public void FootstepSound(AnimationEvent evt)
    {
        print("Called footstepSound");
        if (IsHeaviestAnimClip(evt.animatorClipInfo.clip))
        {
            playerAudio.PlayClip(1, 0.5f);
        }
    }

    public void ChopHit(AnimationEvent evt)
    {
        
        foreach (PlayerAbility ability in characterMovement.playerAbilities)
        {
            switch (ability)
            {
                case QuickChopAbility quickChopAbility:
                    quickChopAbility.AttackCheck();
                    break;
                }
            }
        
    }

    bool IsHeaviestAnimClip(AnimationClip currentClip)
    {
        var currentAnimatorClipInfo = GetComponent<Animator>().GetCurrentAnimatorClipInfo(1);
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


