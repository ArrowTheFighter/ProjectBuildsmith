using UnityEngine;
using UnityEngine.Events;

public class BouncyPlatform : MonoBehaviour,IDamagable
{
    public float BounceHeight;
    public UnityEvent onBounce;

    public bool PlayerCanStomp { get => true; set => PlayerCanStomp = true; }

    [Header("Audio")]
    public AudioCollection[] bounceAudioCollection;

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        return;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = BounceHeight;

        ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, bounceAudioCollection);
        onBounce?.Invoke();
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        return;
    }

}
