using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class MacBoss : MonoBehaviour, IDamagable
{
    [SerializeField] Transform TargetTransform;
    [SerializeField] Transform dodgePositionTransform;
    [SerializeField] Transform[] phase1Positions;
    [SerializeField] float dodgeDistance;
    [SerializeField] float distanceToPosition;
    [SerializeField] bool isActive;
    [SerializeField] bool phase1Attacking;
    bool atPhase1Position;
    [SerializeField] bool phase2Attacking;
    [SerializeField] float speed;
    [SerializeField] float delayBetweenHammerAttacks = 3;
    [SerializeField] float offsetTowardsBossForHammerSpawns = 3;
    [SerializeField] float distanceToStopAttacking = 6;
    float velocity;
    float timer;
    bool phase1AttackInProgress;
    float phase1Health = 3;
    float phase2Health = 1;
    Transform playerTransform;

    [Header("Prefabs")]
    [SerializeField] GameObject HammerAttackPrefab;
    [SerializeField] ParticleSystem TakeDamageParticle;

    public bool canTakeDamge;

    public bool PlayerCanStomp { get => canTakeDamge; set => canTakeDamge = value; }

    [SerializeField] UnityEvent phase1FinishedUnityEvent;
    [SerializeField] UnityEvent phase1DamageTakenUnityEvent;
    [SerializeField] UnityEvent phase2FinishedUnityEvent;
    [SerializeField] UnityEvent onHammerSpawnedUnityEvent;

    void Awake()
    {
        playerTransform = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform;
    }


    // Update is called once per frame
    void Update()
    {
        
        //if(phase1AttackInProgress) return;
        if (!isActive) return;
        if(Vector3.Distance(transform.position, TargetTransform.position) > distanceToPosition)
        {
            velocity = Mathf.MoveTowards(velocity,1,0.05f);
        }
        else
        {
            velocity = Mathf.MoveTowards(velocity,0,0.1f);
            if(!atPhase1Position)
            {
                timer = 0;
                SpawnHammerAttack();
            }
            atPhase1Position = true;
        }
        
        RotateTowardsTarget();

        if(phase1Attacking && atPhase1Position)
        {
            timer += Time.deltaTime;

            if (timer > delayBetweenHammerAttacks)
            {
                SpawnHammerAttack();
                timer = 0;
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToTarget = (TargetTransform.position - transform.position).normalized;

        transform.position = transform.position + directionToTarget * (speed * Time.deltaTime * velocity);

        Vector3 newLookDir = (playerTransform.position - transform.position).normalized;
        newLookDir.y = 0;
        
        if (newLookDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(newLookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f * Time.deltaTime);
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 directionToTarget = (TargetTransform.position - transform.position).normalized;

        transform.position = transform.position + directionToTarget * (speed * Time.deltaTime * velocity);

        Vector3 newLookDir = directionToTarget;
        newLookDir.y = 0;
        if (atPhase1Position || phase2Attacking)
        {
            newLookDir = (playerTransform.position - transform.position).normalized;
            newLookDir.y = 0;
        }
        if (newLookDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(newLookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f * Time.deltaTime);
        }
    }

    void SpawnHammerAttack()
    {
        if(Vector3.Distance(playerTransform.position,transform.position) < distanceToStopAttacking) return;
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:

                phase1AttackRoutineType1();
                break;
            case 1:

                StartCoroutine(phase1AttackRoutineType2());
                break;
            case 2:

                phase1AttackRoutineType3();
                break;
        }
        onHammerSpawnedUnityEvent?.Invoke();
    }

    void phase1AttackRoutineType1()
    {
        Vector3 DirToBoss = transform.position - playerTransform.position;
        Vector3 spawnPosition = playerTransform.position + DirToBoss.normalized * offsetTowardsBossForHammerSpawns;
        Instantiate(HammerAttackPrefab, spawnPosition, transform.rotation);
    }

    IEnumerator phase1AttackRoutineType2()
    {
        Vector3 DirToBoss = transform.position - playerTransform.position;
        Vector3 crossDirection = Vector3.Cross(DirToBoss.normalized,Vector3.up);
        Vector3 spawnPosition = playerTransform.position + DirToBoss.normalized * offsetTowardsBossForHammerSpawns;
        Vector3 spawnPosition2 = spawnPosition + crossDirection * 7 - DirToBoss.normalized;
        Vector3 spawnPosition3 = spawnPosition + -crossDirection * 7 - DirToBoss.normalized;
        Instantiate(HammerAttackPrefab, spawnPosition, transform.rotation);
        yield return new WaitForSeconds(0.25f);
        Instantiate(HammerAttackPrefab, spawnPosition2, transform.rotation);
        Instantiate(HammerAttackPrefab, spawnPosition3, transform.rotation);
    }

    void phase1AttackRoutineType3()
    {
        Vector3 DirToBoss = transform.position - playerTransform.position;
        Vector3 crossDirection = Vector3.Cross(DirToBoss.normalized, Vector3.up);
        Vector3 spawnPosition = playerTransform.position + DirToBoss.normalized * offsetTowardsBossForHammerSpawns;
        Vector3 spawnPosition2 = playerTransform.position + crossDirection * (offsetTowardsBossForHammerSpawns * 0.5f);
        Vector3 spawnPosition3 = playerTransform.position + -crossDirection * (offsetTowardsBossForHammerSpawns * 0.5f);
        Instantiate(HammerAttackPrefab, spawnPosition, transform.rotation);
        Instantiate(HammerAttackPrefab, spawnPosition2, transform.rotation);
        Instantiate(HammerAttackPrefab, spawnPosition3, transform.rotation);
    }

    public void StartPhase1()
    {
        isActive = true;
        phase1Attacking = true;
        canTakeDamge = true;
        TargetTransform = getFurthestPoint(phase1Positions);
    }

    public void StartPhase2()
    {
        phase2Attacking = true;
        canTakeDamge = false;
        Quaternion lookDirection = Quaternion.Euler(0,-158,0);
        transform.DORotate(lookDirection.eulerAngles,2).SetEase(Ease.InOutSine);
    }

    IEnumerator resetCanTakeDamageRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        canTakeDamge = true;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source)
    {
        if (source == gameObject) return;
        canTakeDamge = false;
        StartCoroutine(resetCanTakeDamageRoutine(2));
        if(phase1Attacking)
        {
            phase1Health -= amount;
            atPhase1Position = false;
            TargetTransform = getFurthestPoint(phase1Positions);
            TakeDamageParticle?.Play();
            if (phase1Health <= 0)
            {
                Debug.Log("Phase 1 finished");
                phase1Attacking = false;
                isActive = false;
                phase1FinishedUnityEvent?.Invoke();
            }else
            {
                phase1DamageTakenUnityEvent?.Invoke();
            }
            // else
            // {
            //     isActive = false;
            //     Vector3 dodgeDirection = (dodgePositionTransform.position - transform.position).normalized;
            //     dodgeDirection.y = 0;
            //     dodgeDirection = dodgeDirection.normalized;
            //     Vector3 dodgePosition = transform.position + dodgeDirection * dodgeDistance;
            //     transform.DOMove(dodgePosition,0.75f).SetEase(Ease.InOutQuad).OnComplete(() => {isActive = true;});
            // }
        }
        if(phase2Attacking)
        {
            phase2Health -= amount;
            if(phase2Health <= 0)
            {
                Debug.Log("Phase 2 finished");
                phase2Attacking = false;
                phase2FinishedUnityEvent?.Invoke();
            }
        }
    }

    public Transform getFurthestPoint(Transform[] transforms)
    {
        Transform FurthestPoint = transforms[0];
        foreach(var point in transforms)
        {
            if(point == FurthestPoint) continue;
            if (Vector3.Distance(point.position, playerTransform.position) > Vector3.Distance(FurthestPoint.position, playerTransform.position))
            {
                FurthestPoint = point;
            }
        }
        return FurthestPoint;
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, out float ExtraForce)
    {
        ExtraForce = 1;
        TakeDamage(amount,attackTypes,source);
    }

    public void TakeDamage(float amount, AttackType[] attackTypes, GameObject source, float knockbackStrength = 1)
    {
        TakeDamage(amount, attackTypes, source);
    }
}
