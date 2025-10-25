using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BatEnemy : EnemyBase
{
    Vector3 startPos;
    [SerializeField] Animator animator;
    Vector3 TargetPos;
    Vector3 TargetDir;
    float speed;
    bool waitingForNewPos;
    bool canAttack;
    float startingHealth;

    enum AttackingStates { Roaming, Spotted, Charging, Cooldown }
    AttackingStates enemyState;
    bool charging;
    bool runningEnumerator;

    [Header("Particles")]
    public ParticleSystem ChargingParticles;
    public ParticleSystem DizzyParticles;

    [Header("Visuals")]
    public Transform VisualsTransform;
    public float rotationSpeed = 0.1f;


    [Header("Random Move")]
    public float MaxSpeed = 10;
    public float randomMoveDistance;
    public float randomMoveVariance;
    public LayerMask groundIgnore;

    [Header("Audio")]
    public AudioCollection[] PlayerSpottedAudioCollection;
    public AudioCollection[] BatChargeAudioCollection;

    [Header("Attacks")]
    public float chargeAttackCooldown = 1.5f;

    void Awake()
    {
        startingHealth = Health;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        

        OnPlayerFound += PlayerFound;
        OnPlayerLost += PlayerLost;

    }

    void OnEnable()
    {
        PlayerTransform = null;
        Health = startingHealth;
        enemyState = AttackingStates.Roaming;
        StartCoroutine(CheckForPlayerRoutine());
        StartCoroutine(getNewTargetPos(0));
    }


    void PlayerFound()
    {
        if (enemyState == AttackingStates.Roaming)
        {
            ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, PlayerSpottedAudioCollection);
            enemyState = AttackingStates.Spotted;
            StartCoroutine(SpottedToChargeDelay(2));
        }
    }

    void PlayerLost()
    {
        if (enemyState == AttackingStates.Charging || enemyState == AttackingStates.Cooldown)
        {
            print("resseting bat stuff");
            charging = false;
            canAttack = false;

            ChargingParticles.Stop();
            animator.Play("CharacterArmature|Flying_Idle");
            DizzyParticles.Stop(false,ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        enemyState = AttackingStates.Roaming;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerTransform == null && enemyState != AttackingStates.Roaming)
        {
            PlayerLost();
        }
        switch (enemyState)
        {
            case AttackingStates.Roaming:
                //Rotate visuals
                if(TargetDir.sqrMagnitude > 0.0001f)
                 {
                    Quaternion lookRotation = Quaternion.LookRotation(TargetDir, Vector3.up);
                    VisualsTransform.rotation = Quaternion.Slerp(VisualsTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                }
                
                
                transform.position += TargetDir * speed * Time.fixedDeltaTime;
                if (Vector3.Distance(transform.position, TargetPos) < 2f)
                {
                    if (!waitingForNewPos)
                        StartCoroutine(getNewTargetPos(UnityEngine.Random.Range(0.5f, 1.5f)));

                    speed = Mathf.Lerp(speed, 0, 0.1f);
                }
                else if (!waitingForNewPos)
                {
                    speed = Mathf.Lerp(speed, MaxSpeed, 0.1f);
                }
                break;

            case AttackingStates.Spotted:
                if (PlayerTransform != null)
                {
                    Quaternion PlayerLook = Quaternion.LookRotation((PlayerTransform.position - transform.position).normalized, Vector3.up);
                    VisualsTransform.rotation = Quaternion.Slerp(VisualsTransform.rotation, PlayerLook, rotationSpeed * Time.deltaTime);
                }

                break;

            case AttackingStates.Charging:
                
                if (!charging)
                {
                    charging = true;
                    TargetPos = PlayerTransform.position;
                    TargetDir = (PlayerTransform.position - transform.position).normalized;
                    speed = 15;
                    animator.Play("CharacterArmature|Fast_Flying");
                    canAttack = true;
                    ChargingParticles.Play();
                    ScriptRefrenceSingleton.instance.soundFXManager.PlayRandomSoundCollection(transform, BatChargeAudioCollection);
                }
                if (canAttack && Physics.Raycast(transform.position, TargetDir, out RaycastHit rayhit, 1.5f, playerLayerMask))
                {
                    print("hit something");
                    if (rayhit.collider.transform.parent.TryGetComponent(out PlayerHealth playerHealth))
                    {
                        print("hit player");
                        if (transform.position.y > rayhit.collider.bounds.min.y)
                        {
                            AttackType[] attackTypes = { AttackType.Simple };
                            playerHealth.TakeDamage(1, new AttackType[] { AttackType.Simple }, gameObject);
                            canAttack = false;
                            ChargingParticles.Stop();
                        }
                    } 
                 }
                
                
                Quaternion ChargeLookRotation = Quaternion.LookRotation(TargetDir, Vector3.up);
                VisualsTransform.rotation = Quaternion.Slerp(VisualsTransform.rotation, ChargeLookRotation, rotationSpeed * Time.deltaTime);

                transform.position += TargetDir * speed * Time.fixedDeltaTime;
                if (Vector3.Distance(transform.position, TargetPos) < 2f && !runningEnumerator)
                {
                    canAttack = false;
                    animator.Play("CharacterArmature|Flying_Idle");
                    DizzyParticles.Play();
                    StartCoroutine(ChargeToCooldownDelay(chargeAttackCooldown));
                    speed = 0;
                    ChargingParticles.Stop();
                }
                break;

            case AttackingStates.Cooldown:
                transform.position += TargetDir * speed * Time.fixedDeltaTime;
                Quaternion CooldownLookRotation = Quaternion.LookRotation(TargetDir, Vector3.up);
                VisualsTransform.rotation = Quaternion.Slerp(VisualsTransform.rotation, CooldownLookRotation, rotationSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, TargetPos) < 2f && !runningEnumerator)
                {
                    if (PlayerTransform != null)
                    {
                        TargetDir = (PlayerTransform.position - transform.position).normalized;
                        DOVirtual.Float(speed, 0, 0.75f, (context) =>
                        {
                            speed = context;
                        });
                        StartCoroutine(CooldownToChargeDelay(1));
                    }
                }
                break;
         }
        
            
       
    }

    IEnumerator SpottedToChargeDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PlayerTransform != null)
            enemyState = AttackingStates.Charging;
     }

    IEnumerator ChargeToCooldownDelay(float delay)
    {
        runningEnumerator = true;
        yield return new WaitForSeconds(delay);

        DizzyParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        TargetPos = getRandomPositionNearStart();
        TargetDir = (TargetPos - transform.position).normalized;
        charging = false;
        enemyState = AttackingStates.Cooldown;
        runningEnumerator = false;
        speed = 10;
    }

    IEnumerator CooldownToChargeDelay(float delay)
    {
        runningEnumerator = true;
        yield return new WaitForSeconds(delay);
        enemyState = AttackingStates.Charging;
        runningEnumerator = false;
    }

    IEnumerator getNewTargetPos(float delay)
    {
        waitingForNewPos = true;
        yield return new WaitForSeconds(delay);
        TargetPos = getRandomPositionNearStart();
        TargetDir = (TargetPos - transform.position).normalized;
        waitingForNewPos = false;
        speed = 0;
    }

    Vector3 getRandomPositionNearStart()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 dir = UnityEngine.Random.insideUnitSphere;
            Vector3 distance = dir.normalized * UnityEngine.Random.Range(randomMoveDistance - randomMoveVariance, randomMoveDistance + randomMoveVariance);

            distance.y = UnityEngine.Random.Range(-2f, 2f);

            Vector3 returnPos = startPos + distance;
            if (Physics.CheckSphere(returnPos, 0.5f, ~groundIgnore))
            {
                continue;
            }
            return returnPos;

        }
        return startPos;
    }
}
