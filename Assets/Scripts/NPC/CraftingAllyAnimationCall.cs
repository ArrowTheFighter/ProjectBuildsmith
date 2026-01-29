using UnityEngine;

public class CraftingAllyAnimationCall : MonoBehaviour
{
    public ParticleSystem HammerHitParticle;
    public void PlayHammerHitAnimation()
    {
        if(TryGetComponent(out Animator animator))
        {
            animator.Play("LayingOnAnvilReact");
            HammerHitParticle.Play();
        }
    }
}
