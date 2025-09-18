using UnityEngine;

public class BouncyPlatform : MonoBehaviour,IDamagable
{
    public float BounceHeight;

    public bool PlayerCanStomp { get => true; set => PlayerCanStomp = true; }

    [Header("Audio")]
    [SerializeField] AudioClip bounceSoundFX;
    [SerializeField] float bounceSoundFXVolume = 1f;
    [SerializeField] float bounceSoundFXPitch = 1f;

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        return;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = BounceHeight;

        if (bounceSoundFX != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(bounceSoundFX, transform, bounceSoundFXVolume, bounceSoundFXPitch);
        }
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        return;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
