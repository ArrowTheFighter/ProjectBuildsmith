using UnityEngine;
using DG.Tweening;

public class BoomerangThrowAbility : PlayerAbility
{

    bool IsThrowing;
    public float damageStrength = 1;
    public AttackType[] attackTypes;
    GameObject boomerang;
    GameObject TargetEffect;
    Transform closestHit;

    Vector3 targetPos;
    Vector3 flyDir;
    bool movingAway;
    public float speed = 22;
    float currentSpeed;
    public float maxDistance = 25;
    public float radiusCheck = 7;
    LayerMask targetableLayers;
    float spinSpeed = 500;

    public void Start()
    {
        targetableLayers = 1 << 15;
    }

    public override void UpdateAbility()
    {

        if (!IsThrowing)
        {
            Vector3 checkDir = Camera.main.transform.forward;
            RaycastHit[] TargetCheck = Physics.SphereCastAll(transform.position, radiusCheck, checkDir, maxDistance, targetableLayers);
            if (TargetCheck.Length > 0)
            {
                closestHit = TargetCheck[0].transform;
                foreach (RaycastHit hit in TargetCheck)
                {
                    if (Vector3.Distance(hit.transform.position, transform.position) < Vector3.Distance(closestHit.position, transform.position))
                    {
                        closestHit = hit.transform;
                    }
                }
            }
            else
            {
                closestHit = null;
            }
            if (TargetEffect == null && closestHit != null)
            {
                print("spawning target particle");
                TargetEffect = Instantiate(PlayerParticlesManager.instance.GetTargetParticlePrefab(), closestHit.position, Quaternion.identity);
            }
            else if (TargetEffect != null && closestHit == null)
            {
                print("destroying target particle");
                Destroy(TargetEffect);
            }
            if (TargetEffect != null && closestHit != null)
            {
                if (Physics.Raycast(Camera.main.transform.position, (closestHit.position - Camera.main.transform.position).normalized, out RaycastHit hitInfo, maxDistance * 2, targetableLayers))
                {
                    Vector3 hitPos = hitInfo.point;
                    TargetEffect.transform.position = hitPos - (closestHit.position - Camera.main.transform.position).normalized * 0.25f;
                }
                else
                {
                    TargetEffect.transform.position = closestHit.position;
                 }
            }
            
        }

        if (GameplayInput.instance.playerInput.actions["BasicAttack"].ReadValue<float>() > 0)
        {
            if (!IsThrowing)
            {
                if (!characterMovement.MovementControlledByAbility)
                {
                    IsThrowing = true;
                    if (characterMovement.grounded)
                    {
                        characterMovement.OnBasicAttack?.Invoke();
                        Invoke("Throw", 0.2f);
                    }
                    else
                    {
                        Throw();
                     }
                }
            }

        }

        //if (Time.time > finishThrowDelay) IsThrowing = false;

        if (boomerang != null)
        {
            boomerang.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
            boomerang.transform.position += flyDir.normalized * currentSpeed * Time.deltaTime;

            if (movingAway)
            {

                if (Vector3.Distance(boomerang.transform.position, targetPos) < 1 || (Vector3.Distance(boomerang.transform.position, transform.position) > maxDistance * 1.5f))
                {
                    DOVirtual.Float(speed, 0, 0.5f, (value) =>
                    {
                        currentSpeed = value;
                    }).OnComplete(() =>
                    {
                        movingAway = false;
                        DOVirtual.Float(0, speed, 0.5f, (value) => { currentSpeed = value; });
                    });
                }

                if (Physics.Raycast(boomerang.transform.position, flyDir, out RaycastHit hitInfo, 0.75f))
                {
                    //Boomerang hit something
                    if (hitInfo.collider.TryGetComponent(out IDamagable component))
                    {
                        if (hitInfo.collider.transform.tag != "Player")
                        {
                            component.TakeDamage(1, new AttackType[] { AttackType.Simple }, boomerang);

                        }
                    }
                    else if (hitInfo.collider.transform.parent != null && hitInfo.collider.transform.parent.TryGetComponent(out IDamagable parent_component))
                    {
                        if (hitInfo.collider.transform.parent.tag != "Player")
                        {
                            parent_component.TakeDamage(1, new AttackType[] { AttackType.Simple }, boomerang);
                        }
                    }
                    AudioCollection audioCollection = PlayerAudioManager.instance.GetAudioClipByID("BoomerangHit");
                    SoundFXManager.instance.PlayRandomSoundCollection(boomerang.transform, audioCollection);
                    Instantiate(PlayerParticlesManager.instance.GetParticlePredabByID("BoomerangHit"), boomerang.transform.position, Quaternion.identity);
                    movingAway = false;
                }
            }
            else
            {
                targetPos = transform.position;
                flyDir = targetPos - boomerang.transform.position;
                if (Vector3.Distance(boomerang.transform.position, targetPos) < 1)
                {
                    Destroy(boomerang);
                    IsThrowing = false;
                    HotbarManager.instance.ShowActiveItem();
                }
            }

        }

    }

    public void Throw()
    {
        HotbarManager.instance.HideActiveItem();
        if (TargetEffect != null)
        {
            Destroy(TargetEffect);
        }
        IsThrowing = true;
        currentSpeed = speed;

        Vector3 dir = Camera.main.transform.forward;
        //dir.y = 0;
        targetPos = transform.position + dir.normalized * maxDistance + Vector3.up * 2.5f;
        if (closestHit != null)
        {
            targetPos = closestHit.position;
        }

        flyDir = targetPos - transform.position;
        boomerang = Instantiate(PlayerParticlesManager.instance.GetParticlePredabByID("Boomerang"), transform.position + dir.normalized, Quaternion.identity);
        movingAway = true;

        AudioCollection boomerangThrowCollection1 = PlayerAudioManager.instance.GetAudioClipByID("BoomerangThrow1");
        AudioCollection boomerangThrowCollection2 = PlayerAudioManager.instance.GetAudioClipByID("BoomerangThrow2");

        SoundFXManager.instance.PlayRandomSoundCollection(transform, boomerangThrowCollection1, boomerangThrowCollection2);

    }

    public override void FixedUpdateAbility()
    {
        return;
    }

    public override void ResetAbility()
    {
        if (TargetEffect != null)
        {
            DestroyImmediate(TargetEffect);
        }
        if (boomerang != null)
        {
            Destroy(boomerang);
         }
    }

   
}
