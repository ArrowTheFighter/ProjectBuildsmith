using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;

public class RailGrindAbility : PlayerAbility
{
    Collider[] collisionsAllocation = new Collider[10];
    [HideInInspector]
    public bool onGrindrail;
    float ziplineCooldown;
    Spline currentSpline;
    SplineContainer currentSplineContainer;
    float currentSplinePos;
    public float splineSpeed = 20f;
    public float exitForceMultiplier = 0.8f;
    Vector3 storedVelocity;
    bool directionIsForward = true;
    SplineAnimateCustom splineAnimate;
    Vector3 lastPosition;
  

    public override void Initialize(CharacterMovement player)
    {
        base.Initialize(player);
        characterMovement.characterInput.OnJump += PlayerJumped;

        GameObject splineAnimateGameObject = new GameObject();
        splineAnimate = splineAnimateGameObject.AddComponent<SplineAnimateCustom>();
        splineAnimate.m_Method = SplineAnimateCustom.Method.Speed;
        splineAnimate.m_LoopMode = SplineAnimateCustom.LoopMode.Once;
        splineAnimate.PlayOnAwake = false;

    }
    
    public override void UpdateAbility()
    {
        if(ScriptRefrenceSingleton.instance.gameplayUtils.PauseMenuIsOpen) return;


        CapsuleCollider capsuleCollider = characterMovement.capsuleCollider;
        GetCapsulePoints(capsuleCollider,out Vector3 p1, out Vector3 p2);
        Physics.OverlapCapsuleNonAlloc(p1+ Vector3.down * 0.15f,p2,1.25f,collisionsAllocation);
        bool hitSomething = false;
        foreach(Collider col in collisionsAllocation)
        {
            if(col == null) continue;
            if(col.tag == "Zipline")
            {
                hitSomething = true;
                if(onGrindrail) break;
                if(ziplineCooldown > Time.time) break;

                print("hit zipline");

                Vector3 nearestPoint = GetNearestPointOnSpline(col.GetComponent<SplineContainer>(),characterMovement.transform.position, out Spline spline,out float curvePos);
                
                Vector3 directionToNearestPoint = nearestPoint - transform.position;
                if(Vector3.Dot(directionToNearestPoint.normalized, Vector3.down) < 0.2f) continue;

                ScriptRefrenceSingleton.instance.playerParticlesManager.GetParticleByID("RailGrind").Play();
                characterMovement.playerAnimationController.animator.SetTrigger("StartRailGrind");

                foreach (var ability in characterMovement.playerAbilities)
                {
                    if (ability is DashAbility dashAbility)
                    {
                        dashAbility.ResetAbility();
                        dashAbility.canDash = true;
                    }
                    if (ability is DoubleJumpAbility doubleJumpAbility)
                    {
                        doubleJumpAbility.ResetAbility();
                    }
                }

                currentSpline = spline;
                currentSplineContainer = col.GetComponent<SplineContainer>();
                currentSplinePos = curvePos;

                if (currentSpline.Closed)
                {
                    splineAnimate.m_LoopMode = SplineAnimateCustom.LoopMode.Loop;
                }else
                {
                    splineAnimate.m_LoopMode = SplineAnimateCustom.LoopMode.Once;
                }

                float3 tangent = SplineUtility.EvaluateTangent(currentSpline, currentSplinePos);
                directionIsForward = Vector3.Dot(characterMovement.orientation.forward,tangent) > 0;

                //transform.position = nearestPoint + Vector3.up * 1.5f;

                splineAnimate.Container = currentSplineContainer;
                splineAnimate.NormalizedTime = Mathf.Clamp01(currentSplinePos);
                splineAnimate.m_Reverse = !directionIsForward;
                splineAnimate.MaxSpeed = splineSpeed;
                splineAnimate.Play();
                //PlayerOffsetPosition = nearestPoint;
                transform.position = splineAnimate.transform.position + splineAnimate.transform.up * 1.15f;
                onGrindrail = true;
                characterMovement.MovementControlledByAbility = true;
                characterMovement.rb.linearVelocity = Vector3.zero;
                characterMovement.readyToJump = false;

                return;
                //characterMovement.OverrideGravity = false;
            }
        }
        if(!hitSomething && onGrindrail)
        {
            ExitGrindRail();
        }
        // Move the player while on a grindRail
        if(onGrindrail)
        {
            if(currentSpline != null && splineAnimate != null)
            {
                if(directionIsForward && !currentSpline.Closed && splineAnimate.NormalizedTime >= 1)
                {
                    ExitGrindRail();
                    return;
                }
                else if(!directionIsForward && !currentSpline.Closed && splineAnimate.NormalizedTime <= 0)
                {
                    ExitGrindRail();
                    return;
                }

                transform.position = splineAnimate.transform.position + splineAnimate.transform.up * 1.25f;

                Vector3 downDir = splineAnimate.transform.position - transform.position;

                Quaternion newLookDir = Quaternion.LookRotation(splineAnimate.transform.right * (directionIsForward ? 1 : -1),-downDir);

                characterMovement.transform.rotation = newLookDir;

                // Quaternion rotationAmount = directionIsForward ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, 270, 0);
                // characterMovement.orientation.rotation = rotationAmount * splineAnimate.transform.rotation;

                Vector3 moveDelta = characterMovement.transform.position - lastPosition;

                storedVelocity = moveDelta.normalized * splineSpeed;

                lastPosition = characterMovement.transform.position;

                Physics.SyncTransforms();

            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if(currentSpline == null) return;
        SplineUtility.Evaluate(currentSpline, currentSplinePos, out float3 position, out float3 tangent, out float3 upVector);
        //characterMovement.rb.linearVelocity += (Vector3)tangent * Time.deltaTime * splineSpeed;
        int dir = directionIsForward ? 1 : -1;
        Vector3 directionToTest = ((Vector3)tangent).normalized * dir;
        directionToTest.y = 0;
        //print(directionToTest);
        Vector3 testPoint = characterMovement.transform.position + (directionToTest * splineSpeed);
        Vector3 newGlobalPosition = GetNearestPointOnSpline(currentSplineContainer, testPoint, out Spline spline, out float curvePos);
        Gizmos.DrawSphere(newGlobalPosition, 0.5f);
    }

    public void PlayerJumped()
    {
        ExitGrindRail();

        //characterMovement.OverrideGravity = true;
    }

    void ExitGrindRail()
    {
        if(!onGrindrail) return;
        onGrindrail = false;
        characterMovement.MovementControlledByAbility = false;
        ziplineCooldown = Time.time + 0.25f;
        currentSpline = null;

        characterMovement.orientation.localEulerAngles = Vector3.zero;

        characterMovement.SimulateVelocity(storedVelocity * exitForceMultiplier,-1);

        ScriptRefrenceSingleton.instance.playerParticlesManager.GetParticleByID("RailGrind").Stop();

        characterMovement.playerAnimationController.animator.SetTrigger("StopRailGrind");

        characterMovement.rb.linearVelocity += new Vector3(0,20,2);
        storedVelocity = Vector3.zero;
        Invoke("ResetReadyToJump",0.1f);
        characterMovement.GetComponent<LongFallReset>().ResetTime();
    }

    void ResetReadyToJump()
    {
        characterMovement.readyToJump = true;
    }

    public override void FixedUpdateAbility()
    {
        // we do nothing
    }

    public override void ResetAbility()
    {
        print("reseting rail grind ability");
        characterMovement.StopSimulatingVelocity();
    }

    Vector3 GetNearestPointOnSpline(SplineContainer splineContainer,Vector3 position,out Spline _spline,out float curvePos)
    {
        Vector3 globalNearest = Vector3.zero;
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(position);
        foreach(Spline spline in splineContainer.Splines)
        {
            SplineUtility.GetNearestPoint(spline,localPos,out float3 nearest,out float normalizedCurvePos,SplineUtility.PickResolutionDefault,2);
            
            globalNearest = splineContainer.transform.TransformPoint((Vector3)nearest);
            _spline = spline;
            curvePos = normalizedCurvePos;
            return globalNearest;
        }
        _spline = null;
        curvePos = 0;
        return Vector3.zero;
    }

    void GetCapsulePoints(
    CapsuleCollider capsule,
    out Vector3 p1,
    out Vector3 p2)
    {
        Vector3 axis = capsule.direction switch
        {
            0 => Vector3.right,
            1 => Vector3.up,
            2 => Vector3.forward,
            _ => Vector3.up
        };

        float halfSegment = Mathf.Max(0, capsule.height / 2f - capsule.radius);

        Vector3 center = capsule.center;

        Vector3 p1Local = center + axis * halfSegment;
        Vector3 p2Local = center - axis * halfSegment;

        Transform t = capsule.transform;
        p1 = t.TransformPoint(p1Local);
        p2 = t.TransformPoint(p2Local);
    }

    public static bool GetDirectionToOtherCollider(
    Collider primary,
    Collider other,
    out Vector3 directionToOther,
    out float penetrationDepth
)
    {
        directionToOther = Vector3.zero;
        penetrationDepth = 0f;

        bool overlapped = Physics.ComputePenetration(
            primary,
            primary.transform.position,
            primary.transform.rotation,
            other,
            other.transform.position,
            other.transform.rotation,
            out Vector3 separationDirection,
            out float separationDistance
        );

        if (!overlapped)
            return false;

        // separationDirection points in the direction to move PRIMARY
        // to get out of OTHER, so invert it to get direction TO other
        directionToOther = -separationDirection.normalized;
        penetrationDepth = separationDistance;

        return true;
    }


}
