using UnityEngine;

public class BouncyPlatform : MonoBehaviour,IDamagable
{
    public float BounceHeight;

    public bool PlayerCanStomp { get => true; set => PlayerCanStomp = true; }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        return;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = BounceHeight;
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
