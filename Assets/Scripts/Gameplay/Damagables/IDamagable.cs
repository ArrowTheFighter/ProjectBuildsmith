using UnityEngine;

public interface IDamagable
{
    public void TakeDamage(int amount, AttackType[] attackTypes, GameObject source);
    public void TakeDamage(int amount, AttackType[] attackTypes, GameObject source, out float ExtraForce);
    public void TakeDamage(int amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1);
    bool PlayerCanStomp { get; set; }
}
