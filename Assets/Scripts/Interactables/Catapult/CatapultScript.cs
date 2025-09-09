using System.Collections;
using DG.Tweening;
using UnityEngine;
using System;
using System.Collections.Generic;

public class CatapultScript : MonoBehaviour, IInteractable
{
    public Transform CatapultArm;
    public Vector3 ArmFinalPos;
    public float LaunchForwardForce;
    public float LaunchUpForce;
    Vector3 ArmStartPos;
    public SphereCollider launchPoint;
    [Header("LaunchTween")]
    public float LaunchDuration  = 0.1f;
    public Ease LaunchEase = Ease.InQuad;

    Vector3 LastPos;
    List<Transform> trackedTransforms = new List<Transform>();

    public string PROMPT;
    public string INTERACTION_PROMPT => PROMPT;

    public item_requirement[] items_required;
    public item_requirement[] required_items => items_required;

    public bool CanUse;
    public bool CanInteract { get => CanUse; set => CanUse = value; }

    public bool Interact(Interactor interactor)
    {
        Vector3 launchPos = launchPoint.transform.TransformPoint(launchPoint.center);
        Collider[] CollidersToLaunch = Physics.OverlapSphere(launchPos, launchPoint.radius);
        foreach (Collider target in CollidersToLaunch)
        {
            if (target.transform.parent != null && target.transform.parent.GetComponent<Rigidbody>() != null)
            {
                if (!trackedTransforms.Contains(target.transform.parent))
                {
                    trackedTransforms.Add(target.transform.parent);
                }
            }
            else if (target.GetComponent<Rigidbody>() != null)
            {
                if(!trackedTransforms.Contains(target.transform))
                {
                    trackedTransforms.Add(target.transform);
                }
            }
        }


        LastPos = launchPos;
        DOVirtual.Float(0, 1, LaunchDuration, (context) =>
        {
            CatapultArm.localRotation = Quaternion.Lerp(Quaternion.Euler(ArmStartPos), Quaternion.Euler(ArmFinalPos), context);
            Vector3 MoveOffset = launchPoint.transform.TransformPoint(launchPoint.center) - LastPos;
            LastPos = launchPoint.transform.TransformPoint(launchPoint.center);
            print(trackedTransforms.Count);
            foreach (Transform t in trackedTransforms)
            {
                t.position += MoveOffset;
            }

        }).SetEase(LaunchEase).OnComplete(() =>
        {
            trackedTransforms = new List<Transform>();
            CatapultArm.DOLocalRotate(ArmStartPos, 1f);
            LaunchStuff();
        });
        return true;
    }

    void LaunchStuff()
    {
        Vector3 launchPos = launchPoint.transform.TransformPoint(launchPoint.center);
        Collider[] CollidersToLaunch = Physics.OverlapSphere(launchPos, launchPoint.radius);
        foreach (Collider target in CollidersToLaunch)
        {
            Vector3 launchForce = transform.forward * LaunchForwardForce + transform.up * LaunchUpForce;
            if (target.transform.parent != null && target.transform.parent.TryGetComponent(out CharacterMovement characterMovement))
            {
                characterMovement.AddAbility<CatapultLaunch>();
                foreach (PlayerAbility ability in characterMovement.playerAbilities)
                {
                    if (ability is CatapultLaunch)
                    {
                        CatapultLaunch catapultLaunch = (CatapultLaunch)ability;
                        StartCoroutine(SetInitalVelocity(catapultLaunch));
                     }
                 }
            }
            if (target.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(launchForce, ForceMode.Impulse);
            }
            else if (target.transform.parent != null && target.transform.parent.TryGetComponent(out Rigidbody parent_rb))
            {
                parent_rb.AddForce(launchForce, ForceMode.Impulse);
            }
        }
    }

    IEnumerator SetInitalVelocity(CatapultLaunch catapultLaunch)
    {
        yield return new WaitForFixedUpdate();
        catapultLaunch.SetInitalVelocity();
     }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ArmStartPos = CatapultArm.localRotation.eulerAngles;
    }

}
