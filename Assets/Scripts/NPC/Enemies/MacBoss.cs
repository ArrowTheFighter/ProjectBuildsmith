using System.Collections;
using UnityEngine;

public class MacBoss : MonoBehaviour
{
    [SerializeField] Transform TargetTransform;
    [SerializeField] float distanceToFollow;
    [SerializeField] bool isActive;
    [SerializeField] bool phase1Attacking;
    [SerializeField] float speed;
    float velocity;
    float timer;
    bool phase1AttackInProgress;

    [Header("Prefabs")]
    [SerializeField] GameObject HammerAttackPrefab;



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
    }
}
