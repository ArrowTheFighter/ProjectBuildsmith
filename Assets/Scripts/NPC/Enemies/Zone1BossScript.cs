using UnityEngine;
using UnityEngine.Events;

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


    public UnityEvent[] OnReachedZoneUnityEvent;
    public UnityEvent OnDeathUnityEvent;

    [Header("Item To Drop")]
    [SerializeField] GameObject ItemToDropPrefab;
    [SerializeField] Vector3 itemDropOffset;
    [SerializeField] float itemDropForce;

    [Header("Particles")]
    public ParticleSystem TakeDamageParticle;
    public GameObject DeathParticlePrefab;

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
        targetPos.y = transform.position.y;
        Vector3 directionToTarget = targetPos - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed);
    }

    void ArrivedAtZone()
    {
        canBeHit = true;
        bossState = BossStates.WaitingAtZone;
        OnReachedZoneUnityEvent[currentZone]?.Invoke();
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
            DropItem();
            Instantiate(DeathParticlePrefab,transform.position,Quaternion.identity);
            OnDeathUnityEvent?.Invoke();
            Destroy(gameObject);
            return;
        }
        TakeDamageParticle.Play();
        currentZone++;
        canBeHit = false;
        bossState = BossStates.MoveingToZone;
    }

    void DropItem()
    {
        GameObject itemDropped = Instantiate(ItemToDropPrefab, transform.position + Vector3.up + itemDropOffset, Quaternion.identity);
        ItemPickup itemPickup = itemDropped.GetComponent<ItemPickup>();

        itemPickup.amount = 1;
        itemPickup.respawn_time = -1;
        Rigidbody rigidbody = itemDropped.GetComponent<Rigidbody>();
        rigidbody.useGravity = true;
        Vector3 horDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        rigidbody.AddForce((horDir + Vector3.up) * itemDropForce * UnityEngine.Random.Range(1f, 1.25f), ForceMode.Impulse);
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

    public void PlayGrabAnimation()
    {
        Animator.Play("BossGrab");
    }
}
