using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailGrindAbility : PlayerAbility
{
    Collider[] collisionsAllocation = new Collider[10];
    bool onZipline;
    float ziplineCooldown;
    Spline currentSpline;
    SplineContainer currentSplineContainer;
    float currentSplinePos;
    public float splineSpeed = 0.5f;

    Vector3 PlayerOffsetPosition
    {
        set{transform.position = value + Vector3.up;}
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
                if(onZipline) break;
                if(ziplineCooldown > Time.time) break;
                print("hit zipline");
                Vector3 nearestPoint = GetNearestPointOnSpline(col.GetComponent<SplineContainer>(),characterMovement.transform.position, out Spline spline,out float curvePos);
                currentSpline = spline;
                currentSplineContainer = col.GetComponent<SplineContainer>();
                currentSplinePos = curvePos;
                PlayerOffsetPosition = nearestPoint;
                onZipline = true;
                characterMovement.MovementControlledByAbility = true;
                characterMovement.rb.linearVelocity = Vector3.zero;
                //characterMovement.OverrideGravity = false;
            }
        }
        if(!hitSomething)
        {
            ExitGrindRail();
        }

        if(onZipline)
        {
            if(currentSpline != null)
            {
                print("moving player on spline");
                characterMovement.rb.linearVelocity = Vector3.zero;
                SplineUtility.Evaluate(currentSpline,currentSplinePos,out float3 position,out float3 tangent, out float3 upVector);
                characterMovement.transform.position = characterMovement.transform.position + (Vector3)tangent * Time.deltaTime * splineSpeed;

                Vector3 nearestPoint = GetNearestPointOnSpline(currentSplineContainer, characterMovement.transform.position, out Spline spline, out float curvePos);
                currentSplinePos = curvePos;
            }
        }
    }

    public void PlayerJumped()
    {
        print("RailGrind - Player jumped");
        ExitGrindRail();

        //characterMovement.OverrideGravity = true;
    }

    void ExitGrindRail()
    {
        if(!onZipline) return;
        onZipline = false;
        characterMovement.MovementControlledByAbility = false;
        ziplineCooldown = Time.time + 1;
        currentSpline = null;
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
