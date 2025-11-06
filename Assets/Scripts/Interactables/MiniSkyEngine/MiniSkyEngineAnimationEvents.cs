using UnityEngine;

public class MiniSkyEngineAnimationEvents : MonoBehaviour
{
    [Header("repairScript")]
    public RepairStructure repairStructure;

    [Header("Jumping event")]
    public ParticleSystem JumpingParticles;
    public AudioCollection[] JumpingAudio;

    [Header("StarEvent")]
    public ParticleSystem StarParticle;
    public AudioCollection[] StarAudio;


    public void PlayJumpingParticles()
    {
        JumpingParticles.Play();
        ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, JumpingAudio);
    }

    public void PlayStarParticle()
    {
        StarParticle.Play();
        ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, StarAudio);
    }

    void OnEnable()
    {
        repairStructure.SpawnedStar += PlayScaleAnim;
    }

    void Osable()
    {
        repairStructure.SpawnedStar -= PlayScaleAnim;
    }

    public void PlayScaleAnim()
    {
        GetComponent<Animator>().Play("MiniSEItemGathered");
    }
}