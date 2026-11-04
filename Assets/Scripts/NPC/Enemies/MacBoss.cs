using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MacBoss : MonoBehaviour, IDamagable
{
    [SerializeField] Transform TargetTransform;
    [SerializeField] float distanceToFollow;
    [SerializeField] bool isActive;
    [SerializeField] bool phase1Attacking;
    [SerializeField] bool phase2Attacking;
    [SerializeField] float speed;
    float velocity;
    float timer;
    bool phase1AttackInProgress;
    float phase1Health = 3;
    float phase2Health = 1;

    [Header("Prefabs")]
    [SerializeField] GameObject HammerAttackPrefab;

    public bool canTakeDamge;

    public bool PlayerCanStomp { get => canTakeDamge; set => canTakeDamge = value; }

    [SerializeField] UnityEvent phase1FinishedUnityEvent;
    [SerializeField] UnityEvent phase2FinishedUnityEvent;

    // Update is called once per frame
    void Update()
    {
        if(phase1AttackInProgress) return;
        if (!isActive) return;
        if(Vector3.Distance(transform.position, TargetTransform.position) > distanceToFollow)
        {
            velocity = Mathf.MoveTowards(velocity,1,0.05f);
        }
        else
        {
            velocity = Mathf.MoveTowards(velocity,0,0.1f);
        }
        Vector3 directionToTarget = (TargetTransform.position - transform.position).normalized;

        transform.position = transform.position + directionToTarget * (speed * Time.deltaTime * velocity);

        Vector3 newLookDir = directionToTarget;
        newLookDir.y = 0;
        if (newLookDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(newLookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f * Time.deltaTime);
        }

        if(phase1Attacking)
        {
            timer += Time.deltaTime;

            if (timer > 5)
            {
                StartCoroutine(phase1AttackRoutine());
                timer = 0;
            }
        }
    }

    IEnumerator phase1AttackRoutine()
    {
        phase1AttackInProgress = true;
        yield return new WaitForSeconds(0.5f);
        print("Attacking");
        Instantiate(HammerAttackPrefab,TargetTransform.position,transform.rotation);
        yield return new WaitForSeconds(2f);
        print("attack finished");
        phase1AttackInProgress = false; 
    }

    public void StartPhase1()
    {
        isActive = true;
        phase1Attacking = true;
        canTakeDamge = true;
    }

    public void StartPhase2()
    {
        phase2Attacking = true;
        canTakeDamge = true;
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
            if (phase1Health <= 0)
            {
                Debug.Log("Phase 1 finished");
                phase1Attacking = false;
                isActive = false;
                phase1FinishedUnityEvent?.Invoke();
            }
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
