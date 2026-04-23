using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;

public class Zone1BossScript : MonoBehaviour, IDamagable
{
    public Animator Animator;

    public int Health = 3;
    public int currentZone;
    public Transform[] ZoneTransforms;
    public Transform playerTransform;
    Vector3 velocity;

    public float speed;
    public float rotationSpeed;

    public UnityEvent OnReachedZoneUnityEvent;

    enum BossStates 
    {
        InitalState   = 0,
        MoveingToZone = 1,
        WaitingAtZone = 2
    }

    BossStates bossState;
    bool canBeHit = false;

    bool playerCanStomp = true;
    public bool PlayerCanStomp { get => playerCanStomp; set { playerCanStomp = value;} }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossState = BossStates.InitalState;
        playerTransform = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform;

    }

    public void SetBossState(int state)
    {
        bossState = (BossStates)state;
    }

    // Update is called once per frame
    void Update()
    {
        switch(bossState)
        {
            case BossStates.MoveingToZone:

                if (Vector3.Distance(transform.position,ZoneTransforms[currentZone].position) < 1.5f)
                {
                    ArrivedAtZone();
                    return;
                }
                MoveTowardsTarget();
                
            break;

            case BossStates.WaitingAtZone:
                RotateTowardsPlayer();
            break;
            default:

            break;
        }
    }

    void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position,ZoneTransforms[currentZone].position,speed * Time.deltaTime);
        SmoothRotateTowardsTarget(ZoneTransforms[currentZone].position);
    }

    void RotateTowardsPlayer()
    {
        SmoothRotateTowardsTarget(playerTransform.position);
    }

    void SmoothRotateTowardsTarget(Vector3 targetPos)
    {
        Vector3 directionToTarget = targetPos - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed);
    }

    void ArrivedAtZone()
    {
        canBeHit = true;
        bossState = BossStates.WaitingAtZone;
        OnReachedZoneUnityEvent?.Invoke();
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        if (!canBeHit)
        {
            return;
        }

        Health--;
        if(Health <= 0)
        {
            print("We should die now");
            return;
        }
        currentZone++;
        canBeHit = false;
        bossState = BossStates.MoveingToZone;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        TakeDamage(amount,attackTypes,source);
        ExtraForce = 1;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        TakeDamage(amount,attackTypes,source);
    }
}
