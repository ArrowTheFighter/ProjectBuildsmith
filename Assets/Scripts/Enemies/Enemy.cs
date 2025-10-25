using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public GameObject projectile;
    public GameObject spawnLocation;

    [SerializeField] private bool isActive;

    private float delayTimer;
    private float delay;
    private float distanceToCheck = 50f;

    private void Awake()
    {
        InvokeRepeating("FireProjectile", 1f, 5f);
    }

    private void Update()
    {
        delayTimer = Time.time + delay;

        if(Time.time > delay)
        {
            if (Vector3.Distance(ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.position, gameObject.transform.position) < distanceToCheck)
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
        }
    }

    void FireProjectile()
    {
        if (isActive)
        {
            var firedProjectile = Instantiate(projectile, spawnLocation.transform.position, gameObject.transform.rotation);
        }
    }
}
