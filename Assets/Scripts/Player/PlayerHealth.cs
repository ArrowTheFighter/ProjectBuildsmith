using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{

    [SerializeField] int MaxHealth;
    [SerializeField] int Health;

    public void TakeDamage(int amount,GameObject source)
    {
        if (source == GameplayUtils.instance.PlayerTransform.gameObject) return;
        Health -= amount;
        if (Health <= 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        print("respawning");
        transform.position = gameObject.GetComponent<PlayerSafeZone>().safePos;
    }
}
