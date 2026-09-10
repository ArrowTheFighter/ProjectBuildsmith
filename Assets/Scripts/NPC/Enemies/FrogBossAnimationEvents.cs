using UnityEngine;

public class FrogBossAnimationEvents : MonoBehaviour
{
    
    [SerializeField] FrogBoss frogBoss;

    public void SpawnAttackBullet()
    {
        frogBoss.SpawnBullet();
    }
}
