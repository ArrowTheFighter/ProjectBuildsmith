using UnityEngine;

public class BlacksmithTrailerCutscene : MonoBehaviour
{
    public ParticleSystem sparksParticle;
    public void SpawnHammerSparks()
    {
        sparksParticle.Play();
    }

    public void SwitchToShocked()
    {
        GetComponent<Animator>().SetTrigger("Shocked");
    }
}
