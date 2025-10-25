using System;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] CharacterMovement characterMovement;
    public Action OnAxeSwingStart;
    public Action OnAxeSwingEnd;

    public AudioClip footstepsSoundFX;
    public float footstepsSoundFXVolume;
    public float footstepsSoundFXPitch;

    public ParticleSystem walkingParticle;

    public void FootstepSound(AnimationEvent evt)
    {
        if (IsHeaviestAnimClip(evt.animatorClipInfo.clip))
        {
            if (footstepsSoundFX != null)
            {
                ScriptRefrenceSingleton.instance.soundFXManager.PlaySoundFXClip(footstepsSoundFX, transform, footstepsSoundFXVolume, footstepsSoundFXPitch);
                
            }
            if (walkingParticle != null)
            {
                walkingParticle.Play();
            }
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

    public void AxeSwingStart()
    {
        OnAxeSwingStart?.Invoke();
    }

    public void AxeSwingEnd()
    {
        OnAxeSwingEnd?.Invoke();
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


