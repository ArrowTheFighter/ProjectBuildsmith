using UnityEngine;

public interface IDamagable 
{
    public void TakeDamage(int amount,GameObject source);
    public void TakeDamage(int amount, GameObject source,out float ExtraForce);
}
