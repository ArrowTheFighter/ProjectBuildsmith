using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class TreeSpitter : EnemyBase
{
    public AttackType[] attackTypes;

    public Transform[] SpawnPoints;
    Transform currentSpawnPoint;
    Vector3 startPos;
    enum treeSpitterStates { idle, PlayerFound, shooting, dancing, moving, lostPlayer }
    treeSpitterStates enemyState = treeSpitterStates.idle;

    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    GameObject Projectile;
    Vector3 targetPos;
    float travelTime = 1;
    public float arcHeight = 10f;

    Vector3 projectileStartPos;
    Vector3 projectileControlPoint;
    float timeElapsed;
    bool hidden;

    [Header("Delays")]
    public float shootDelay = 1;
    public float DanceDuration = 1;

    Animator animator;

    [Header("Audio")]
    public AudioCollection[] PlayerSpottedAudioCollection;
    public AudioCollection[] ShootProjectileAudioCollection;
    public AudioCollection[] HidingAudioCollection;


    void Start()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position;
        StartCoroutine(CheckForPlayerRoutine());

        OnPlayerFound += PlayerFound;
        OnPlayerLost += PlayerLost;
        OnDeath += WeDied;
    }

    void WeDied()
    {
        //TODO Rework the projectile so it can live without the enemy
        if (Projectile != null)
        {
            Destroy(Projectile);
        }
     }

    void Update()
    {
        if (PlayerTransform != null)
        {
            Vector3 dirToPlayer = PlayerTransform.position - transform.position;
            dirToPlayer.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(dirToPlayer.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.1f);
        }
    }

    void PlayerFound()
    {
        if (enemyState == treeSpitterStates.idle)
        {
            SoundFXManager.instance.PlayRandomSoundCollection(transform, PlayerSpottedAudioCollection);
            animator.Play("Jump");
            enemyState = treeSpitterStates.PlayerFound;
            StartCoroutine(PlayerSpottedDelay());
        }

    }

    IEnumerator PlayerSpottedDelay()
    {
        yield return new WaitForSeconds(1);
        StartMove();
    }


    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        ShootProjectile();
    }

    void ShootProjectile()
    {
        if (enemyState == treeSpitterStates.PlayerFound)
        {
            animator.CrossFade("Bite_Front", 0.1f);
            enemyState = treeSpitterStates.shooting;
            //SpawnProjectile();
            StartCoroutine(StartDanceDelay());
        }
    }

    public void SpawnProjectile()
    {
        SoundFXManager.instance.PlayRandomSoundCollection(transform, ShootProjectileAudioCollection);
        print("player transform = " + PlayerTransform);
        if (PlayerTransform != null)
        {
            Projectile = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
            projectileStartPos = Projectile.transform.position;
            targetPos = PlayerTransform.position;
            Vector3 midPoint = Vector3.Lerp(projectileStartPos, targetPos, 0.5f);
            midPoint.y += arcHeight;
            projectileControlPoint = midPoint;
            timeElapsed = 0;
            StartCoroutine(ProjectileProcess());
        }

    }

    IEnumerator ProjectileProcess()
    {
        while (Projectile != null)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / travelTime);

            Vector3 m1 = Vector3.Lerp(projectileStartPos, projectileControlPoint, t);
            Vector3 m2 = Vector3.Lerp(projectileControlPoint, targetPos, t);

            Vector3 curPos = Vector3.Lerp(m1, m2, t);
            Projectile.transform.position = curPos;

            if (t > 0.15f && Physics.Raycast(Projectile.transform.position, (targetPos - curPos).normalized, out RaycastHit hitInfo, 0.75f))
            {
                if (hitInfo.collider.transform != transform && hitInfo.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(1, attackTypes, Projectile);
                }
                else if (hitInfo.collider.transform.parent != null && hitInfo.collider.transform.parent.TryGetComponent(out IDamagable parentDamagable))
                {
                    parentDamagable.TakeDamage(1, attackTypes, Projectile);
                }
                Destroy(Projectile);
            }

            if (t >= 1)
            {
                Destroy(Projectile);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartDanceDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        StartDance();
    }

    void StartDance()
    {
        if (enemyState == treeSpitterStates.shooting)
        {
            animator.CrossFade("Dance", 0.1f);
            enemyState = treeSpitterStates.dancing;
            StartCoroutine(StartMoveDelay());
        }
    }

    IEnumerator StartMoveDelay()
    {
        yield return new WaitForSeconds(DanceDuration);
        StartMove();
    }

    void StartMove()
    {
        animator.CrossFade("Idle", 0.1f);
        enemyState = treeSpitterStates.moving;
        Hide();
        StartCoroutine(EndMoveDelay());
    }

    IEnumerator EndMoveDelay()
    {
        yield return new WaitForSeconds(shootDelay);
        EndMove();
    }

    void EndMove()
    {
        enemyState = treeSpitterStates.PlayerFound;
        MoveToNewPosition();
        StartCoroutine(ShootDelay());
    }

    void MoveToNewPosition()
    {
        List<Transform> availableSpots = new List<Transform>();
        foreach (Transform t in SpawnPoints)
        {
            if (t != currentSpawnPoint) availableSpots.Add(t);
        }
        currentSpawnPoint = availableSpots[UnityEngine.Random.Range(0, availableSpots.Count)];
        transform.position = currentSpawnPoint.position + Vector3.down * 2.5f;
        transform.DOMove(transform.position + Vector3.up * 2.5f, 0.35f).SetEase(Ease.InOutQuad);
        CanTakeDamage = true;

        SoundFXManager.instance.PlayRandomSoundCollection(transform, HidingAudioCollection);
    }

    void Hide()
    {
        SoundFXManager.instance.PlayRandomSoundCollection(transform, HidingAudioCollection);
        transform.DOMove(transform.position + Vector3.down * 2.5f, 0.35f).SetEase(Ease.InOutQuad).OnComplete(() => { CanTakeDamage = false; });
    }

    void PlayerLost()
    {
        animator.CrossFade("No", 0.1f);
        enemyState = treeSpitterStates.idle;
        StartCoroutine(LostPlayerDelay());
    }

    IEnumerator LostPlayerDelay()
    {
        yield return new WaitForSeconds(2);
        if (PlayerTransform != null) yield break;

        Hide();
        yield return new WaitForSeconds(0.5f);
        if (PlayerTransform != null) yield break;
        currentSpawnPoint = null;
        transform.position = startPos + Vector3.down * 2.5f;
        transform.DOMove(transform.position + Vector3.up * 2.5f, 0.35f).SetEase(Ease.InOutQuad);

    }



}
