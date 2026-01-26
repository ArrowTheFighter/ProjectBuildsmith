using NUnit.Framework.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Splines;

public class RailGrindAbility : PlayerAbility
{
    Collider[] collisionsAllocation = new Collider[10];
    bool onGrindrail;
    float ziplineCooldown;
    Spline currentSpline;
    SplineContainer currentSplineContainer;
    float currentSplinePos;
    public float splineSpeed = 1f;
    Vector3 storedVelocity;
    bool directionIsForward = true;

    Vector3 PlayerOffsetPosition
    {
        get => transform.position + Vector3.up * 1.5f;
        set {transform.position = value + Vector3.up * 1.5f;}
    }

    public override void Initialize(CharacterMovement player)
    {
        base.Initialize(player);
        characterMovement.characterInput.OnJump += PlayerJumped;
    }
    
    public override void UpdateAbility()
    {
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
                
                currentSpline = spline;
                currentSplineContainer = col.GetComponent<SplineContainer>();
                currentSplinePos = curvePos;

                float3 tangent = SplineUtility.EvaluateTangent(currentSpline, currentSplinePos);
                directionIsForward = Vector3.Dot(characterMovement.orientation.forward,tangent) > 0;

                PlayerOffsetPosition = nearestPoint;
                onGrindrail = true;
                characterMovement.MovementControlledByAbility = true;
                characterMovement.rb.linearVelocity = Vector3.zero;
                characterMovement.readyToJump = false;
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
            if(currentSpline != null)
            {
                characterMovement.rb.linearVelocity = Vector3.zero;
                Vector3 lastPos = characterMovement.transform.position;

                SplineUtility.Evaluate(currentSpline,currentSplinePos,out float3 position,out float3 tangent, out float3 upVector);
                //characterMovement.rb.linearVelocity += (Vector3)tangent * Time.deltaTime * splineSpeed;
                int dir = directionIsForward? 1 : -1;
                Vector3 directionToTest = ((Vector3)tangent).normalized * dir;
                directionToTest.y = 0;
                //print(directionToTest);
                print(directionToTest * (splineSpeed * Time.deltaTime ));
                Vector3 testPoint = characterMovement.transform.position + directionToTest * (splineSpeed * Time.deltaTime);

                Vector3 newGlobalPosition = GetNearestPointOnSpline(currentSplineContainer, testPoint, out Spline spline, out float curvePos);
                //float3 newPosition = SplineUtility.EvaluatePosition(currentSpline, curvePos);

                //Vector3 newGlobalPosition = currentSplineContainer.transform.TransformPoint(newPosition);
                PlayerOffsetPosition = newGlobalPosition;
                //characterMovement.transform.position = newGlobalPosition;

                //Vector3 nearestPoint = GetNearestPointOnSpline(currentSplineContainer, characterMovement.transform.position, out Spline _spline, out float _curvePos);
                currentSplinePos = curvePos;

                Vector3 dirThingy = testPoint - lastPos;
                storedVelocity = dirThingy.normalized * (splineSpeed * 0.25f) / Time.fixedDeltaTime;
                //print(storedVelocity);
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
        Gizmos.DrawSphere(testPoint,0.5f);
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

        characterMovement.SimulateVelocity(storedVelocity,3);

        characterMovement.rb.linearVelocity += new Vector3(0,20,2);
        storedVelocity = Vector3.zero;
        Invoke("ResetReadyToJump",0.1f);
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
        //we chill for now
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



}
