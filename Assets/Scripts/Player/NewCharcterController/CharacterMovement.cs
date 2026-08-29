using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour, IPlatformPassenger
{
    [Header("Movement")]
    public float maxSpeed;
    float currentMaxSpeed;
    public float moveSpeed;

    public float groundDrag;
    public float airDrag;
    float horizontalAirDrag = 5f;

    public float coyoteTime = 0.2f;
    float coyoteCheck;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [HideInInspector] public bool readyToJump;
    [HideInInspector] public bool MovementControlledByAbility;
    [HideInInspector] public bool OverrideGravity;
    [HideInInspector] public float OverrideAirDragAmount;
    Vector3 simulatedVelocity;
    Vector3 lastAppliedSimulatedVelocity;
    Coroutine simulatedVelocityCoroutine;
    bool isSimulatingVelocity;
    bool jumpOveride;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 0.1f;
    [SerializeField] float maxTiltAngle = 25f;
    [HideInInspector] public float tilt_amount;
    float current_tilt;


    [Header("Gravity")]
    [SerializeField] float GravityForce;
    public Vector3 GravityDir;

    [SerializeField] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Slopes")]
    [SerializeField] public float maxSlopeAngle;
    RaycastHit groundHit;
    Vector3 groundNormal;
    RaycastHit steepSlopHit;
    RaycastHit slopeHit;
    public bool exitingSlope;
    [HideInInspector]
    public bool forceSteepSlope = false;

    [Header("Input")]
    [HideInInspector] public ICharacterInput characterInput;
    [HideInInspector] public PlayerInput playerInput;

    [Header("Ground Check")]
    public float playerHeight;
    public float playerRadius;
    public float groundCheckDistance = 0.5f;
    public float secondaryGroundCheckDistance = 0.6f;
    public float groundSnapRayDistance;
    public LayerMask IgnoreGroundLayerMask;
    [HideInInspector] public bool grounded;

    [Header("Collider")]
    [SerializeField] public CapsuleCollider capsuleCollider;

    public Transform orientation;

    Vector3 ControllerInput;

    Vector3 moveDirection;

    [HideInInspector] public Rigidbody rb;

    [HideInInspector] public List<PlayerAbility> playerAbilities = new List<PlayerAbility>();

    [HideInInspector] public PlayerAnimationController playerAnimationController;


    [Header("Ability Actions")]
    public Action onDoubleJump;
    public Action OnJump;
    public Action OnDash;
    public Action OnDashStop;
    public Action OnBasicAttack;

    public Action<Vector3> OnFinishedTurning;

    IMoveingPlatform moveingPlatform;
    public Vector3 platformDelta;

    Vector3 lastPlatformLocalPos;
    Vector3 lastPlatformGlobalPos;
    Vector3 platformCurrentFrameDelta;
    Vector3 lastLocalRotation;

    //Checking last platform
    IMoveingPlatform lastMovingPlatform;

    Vector3 lastPlatformLocalPosCheck;
    Vector3 lastPlatformGlobalPosCheck;


    private static readonly Collider[] _trigger_check_results = new Collider[8];

    [Header("Debug")]
    public bool printStrings;

    

    private void Start()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
        characterInput = GetComponent<ICharacterInput>();
        characterInput.OnJump += Jump;
        GravityDir = Vector3.down;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentMaxSpeed = maxSpeed;

        readyToJump = true;
        Cursor.lockState = CursorLockMode.Locked;
        AddAbility<DoubleJumpAbility>();
        AddAbility<DashAbility>();
        AddAbility<PlayerStompAbility>();
        AddAbility<RailGrindAbility>();
        if (characterInput is PlayerInputClass)
        {
            AddAbility<NoClip>();
        }
    }

    private void Update()
    {
        if (characterInput is NPCFollowTargetInput)
        {
            NPCFollowTargetInput nPCFollow = (NPCFollowTargetInput)characterInput;
            if (nPCFollow.IsWalking) currentMaxSpeed = maxSpeed * 0.5f;
            else currentMaxSpeed = maxSpeed;
        }


        // Ground check
        GroundCheck();
        if (printStrings)
        {
            //print(grounded);
        }
        // Handle Input
        MyInput();

        if (!MovementControlledByAbility)
        {
            ApplyGravity();
            // handle drag
            if (grounded)
            {

                rb.linearDamping = groundDrag;
            }
            else
            {
                ApplyAirDrag(horizontalAirDrag);
            }
        }
        else if (OverrideGravity)
        {
            if (moveingPlatform == null)
            {
                ApplyGravity();
            }
        }

        if (OverrideAirDragAmount > 0)
        {
            ApplyAirDrag(OverrideAirDragAmount);
        }

        foreach (PlayerAbility ability in playerAbilities.ToList())
        {
            ability.UpdateAbility();
        }
        if (coyoteCheck > 0) coyoteCheck -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        ApplyPlatformDelta();
        ResolveUpwardPlatformPenetration();
        if (!MovementControlledByAbility)
        {
            MovePlayer();
            RotateOrientation();
            SnapToGround();
            // Set max speed
            SpeedControl();
        }
        ApplySimulatedVelocity();
        if (moveingPlatform != null)
        {
            //capsuleCollider.excludeLayers = 1 << 14;
            //moveSpeed = walkSpeed * 2;
        }
        else
        {
            //capsuleCollider.excludeLayers &= ~(1 << 14);
            //moveSpeed = walkSpeed;
        }
        //ApplyPlatformDelta();
        UpdateTilt();
        foreach (PlayerAbility ability in playerAbilities.ToList())
        {
            ability.FixedUpdateAbility();
        }
    }

    void ApplyPlatformDelta()
    {
        Vector3 currentGlobalPos = Vector3.zero;

        if (lastPlatformGlobalPos == Vector3.zero) return;
        
        if (moveingPlatform == null)
        {
            if (lastMovingPlatform != null)
            {
                // print("doing last platform check");
                // Vector3 currentGlobalPos = lastMovingPlatform.getInterfaceTransform().TransformPoint(lastPlatformLocalPosCheck);
                // Vector3 checkDelta = currentGlobalPos - lastPlatformGlobalPosCheck;

                // float distance = groundCheckDistance;
                // if (rb.linearVelocity.y < -4) distance += 0.3f;
                // Physics.SphereCast(transform.position + checkDelta + Vector3.down * playerHeight * 0.25f, playerRadius, Vector3.down, out RaycastHit hitInfo, distance, ~IgnoreGroundLayerMask);

                // if(hitInfo.collider.transform == lastMovingPlatform.getInterfaceTransform())
                // {
                //     transform.position += checkDelta;
                // }
                currentGlobalPos = lastMovingPlatform.getInterfaceTransform().TransformPoint(lastPlatformLocalPos);
            }
        }else
        {
            currentGlobalPos = moveingPlatform.getInterfaceTransform().TransformPoint(lastPlatformLocalPos);
        }
            //currentGlobalPos = moveingPlatform.getInterfaceTransform().TransformPoint(lastPlatformLocalPos);
        platformCurrentFrameDelta = currentGlobalPos - lastPlatformGlobalPos;

        lastPlatformGlobalPos = Vector3.zero;
        lastPlatformLocalPos = Vector3.zero;
        if(platformCurrentFrameDelta.magnitude > 15)
        {
            print($"platform delta was huge {platformCurrentFrameDelta}!!!");
            Debug.Break();
        }
        if (platformCurrentFrameDelta != Vector3.zero)
            transform.position += platformCurrentFrameDelta;
        
        if(lastLocalRotation != Vector3.zero)
        {
            if(moveingPlatform != null)
            {
                Vector3 lastGlobalRotation = moveingPlatform.getInterfaceTransform().TransformDirection(lastLocalRotation);
                float angleDelta = Vector3.SignedAngle(transform.forward, lastGlobalRotation,Vector3.up);
                if (angleDelta != 0)
                {
                    transform.Rotate(Vector3.up, angleDelta);
                }
            }
            
        }
           
        if(grounded)
            platformDelta = platformCurrentFrameDelta / Time.fixedDeltaTime;
        platformCurrentFrameDelta = Vector3.zero;
    }

    void ResolveUpwardPlatformPenetration()
    {
        if (moveingPlatform == null)
        {
            Vector3 center = capsuleCollider.transform.TransformPoint(capsuleCollider.center);
            float originalRadius = capsuleCollider.radius * Mathf.Max(
                capsuleCollider.transform.lossyScale.x,
                capsuleCollider.transform.lossyScale.z
            );

            float originalHeight = Mathf.Max(capsuleCollider.height * capsuleCollider.transform.lossyScale.y, originalRadius * 2f);

            Vector3 dir;
            switch (capsuleCollider.direction)
            {
                case 0: dir = capsuleCollider.transform.right; break;   // X-axis
                case 1: dir = capsuleCollider.transform.up; break;      // Y-axis
                case 2: dir = capsuleCollider.transform.forward; break; // Z-axis
                default: dir = Vector3.up; break;
            }

            float pointOffset = (originalHeight / 2f) - originalRadius;

            Vector3 point1 = center + dir * pointOffset;
            Vector3 point2 = center - dir * pointOffset;

            Collider[] results = new Collider[10];
            float shrinkAmount = 0.05f; // world units
            float radius = Mathf.Max(0f, originalRadius - shrinkAmount);
            float height = Mathf.Max(radius * 2f, originalHeight - shrinkAmount * 2f);

            int hitCount = Physics.OverlapCapsuleNonAlloc(
                point1,
                point2,
                radius,
                results,
                ~IgnoreGroundLayerMask,
                QueryTriggerInteraction.Ignore
            );

            if (hitCount > 0)
            {

                // Loop through all hits and resolve penetration
                for (int i = 0; i < hitCount; i++)
                {
                    Collider hitCollider = results[i];

                    if (hitCollider == null || hitCollider == capsuleCollider)
                        continue;

                    if (Physics.ComputePenetration(
                        capsuleCollider,
                        transform.position,
                        transform.rotation,
                        hitCollider,
                        hitCollider.transform.position,
                        hitCollider.transform.rotation,
                        out Vector3 direction,
                        out float distance))
                    {
                        // Only resolve if the collision is pushing us UP
                        if (Vector3.Dot(direction, Vector3.up) > 0.5f && distance > 0.01f)
                        {
                            // Move the object out of penetration
                            transform.position += direction * distance;

                            // Kill downward velocity so we don't re-penetrate
                            if (rb != null && rb.linearVelocity.y < 0f)
                            {
                                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                            }

                            //if (printStrings) Debug.Log($"Resolved penetration with {hitCollider.name}: {direction * distance}");
                        }
                    }
                }
            }
        }

    }

    public void TurnAround(float duration = 0.5f)
    {
        Quaternion forwardTarget = Quaternion.LookRotation(-transform.forward, Vector3.up);
        transform.DORotate(forwardTarget.eulerAngles, duration, RotateMode.Fast);
     }

    public void ManualTurn(Vector3 dir,float duration = 0.25f)
    {
        Quaternion forwardTarget = Quaternion.LookRotation(dir, Vector3.up);
        transform.DORotate(forwardTarget.eulerAngles, duration, RotateMode.Fast);
    }

    public void AddAbility<T>() where T : PlayerAbility
    {

        var newAbility = gameObject.AddComponent<T>();
        newAbility.Initialize(this);
        playerAbilities.Add(newAbility);
    }

    public void RemoveAbility<T>() where T : PlayerAbility
    {
        // List<PlayerAbility> abilitiesToRemove = new List<PlayerAbility>();
        // foreach (PlayerAbility playerAbility in playerAbilities)
        // {
        //     if (TryGetComponent(out T component))
        //     {
        //         abilitiesToRemove.Add(playerAbility);
        //     }
        // }

        // foreach (PlayerAbility abilityToRemove in abilitiesToRemove)
        // {
        //     playerAbilities.RemoveAll(ability => ability.GetType() == T);
        //     DestroyImmediate(abilityToRemove);
        // }

        
        Type type = typeof(T);
        Component component = ScriptRefrenceSingleton.instance.gameplayUtils.PlayerTransform.GetComponent(type);

        if (component != null)
        {
            if (component is PlayerAbility)
            {
                PlayerAbility playerAbility = (PlayerAbility)component;
                playerAbility.ResetAbility();
            }
            DestroyImmediate(component);
        }
        playerAbilities.RemoveAll(ability => ability.GetType() == type);
        
    }

    void trackPlatformDelta(Vector3 delta)
    {
        //platformDelta = delta / Time.fixedDeltaTime;
    }

    void IPlatformPassenger.BeforePlatformMove(IMoveingPlatform moveingPlatform)
    {
        BeforePlatformMove();
    }

    void BeforePlatformMove()
    {
        if (moveingPlatform == null) return;
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitinfo, 1.25f))
        {
            lastPlatformLocalPos = moveingPlatform.getInterfaceTransform().InverseTransformPoint(hitinfo.point);
            lastPlatformGlobalPos = hitinfo.point;

            lastLocalRotation = moveingPlatform.getInterfaceTransform().InverseTransformDirection(transform.forward);
        }

    }
    
    void BeforeLastPlatformMove()
    {
        if (lastMovingPlatform == null) return;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitinfo, 1.25f))
        {
            lastPlatformLocalPosCheck = lastMovingPlatform.getInterfaceTransform().InverseTransformPoint(hitinfo.point);
            lastPlatformGlobalPosCheck = hitinfo.point;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * playerHeight * 0.23f, playerRadius);
        Gizmos.color = Color.red;
        Vector3 tempGravityDir = GravityDir;
        if (tempGravityDir == Vector3.zero) tempGravityDir = Vector3.down;
        Gizmos.DrawWireSphere((transform.position + Vector3.down * playerHeight * 0.23f) + tempGravityDir * groundSnapRayDistance, playerRadius);
        
        // Gizmos.DrawWireSphere(transform.position + Vector3.down * playerHeight * 0.25f, playerRadius);
        // Gizmos.DrawLine(transform.position + Vector3.down * playerHeight * 0.25f + Vector3.down * playerRadius, transform.position + Vector3.down * playerHeight * 0.25f + Vector3.down * playerRadius + Vector3.down * groundCheckDistance);
    }


    void GroundCheck()
    {
        if (moveingPlatform == null && lastMovingPlatform != null)
        {
            //lastMovingPlatform.OnBeforePlatformMove -= BeforeLastPlatformMove;
        }
        lastMovingPlatform = moveingPlatform;

        bool wasGrounded = grounded;
        float distance = groundCheckDistance;
        if (rb.linearVelocity.y < -4) distance += 0.3f;
        grounded = Physics.SphereCast(transform.position + Vector3.down * playerHeight * 0.25f + Vector3.up * 0.5f, playerRadius, Vector3.down, out groundHit, distance + 0.5f, ~IgnoreGroundLayerMask,QueryTriggerInteraction.Ignore);
        if (groundHit.transform != null && groundHit.transform.tag == "CantWalk")
        {
            forceSteepSlope = true;
            grounded = false;
        }
        else
        {
            forceSteepSlope = false;
            LayerMask mask = 1 << 2;
            if (groundHit.transform != null && CapsuleInsideTriggerWithTag(capsuleCollider, "CantWalk", mask))
            {
                forceSteepSlope = true;
                grounded = false;
            }
            else
            {
                forceSteepSlope = false;
            }
        }
        

        if (grounded && groundHit.collider.TryGetComponent(out IMoveingPlatform platform))
        {
            if (moveingPlatform != platform)
            {

                if(moveingPlatform == null)
                {

                    platformDelta = Vector3.zero;
                    //lastPlatformLocalPos = Vector3.zero;
                    lastPlatformGlobalPos = Vector3.zero;
                }
                
                moveingPlatform = platform;
                if ((Component)moveingPlatform != null && ((Component)moveingPlatform).gameObject != null)
                {
                    // Collider collider = ((Component)moveingPlatform).gameObject.GetComponent<Collider>();
                    // if (collider != null && collider.sharedMaterial != null)
                    // {
                    //     collider.sharedMaterial.dynamicFriction = 5;
                    //     collider.sharedMaterial.staticFriction = 5;
                    // }
                }
                //print($"adding moving platform for ${((Component)moveingPlatform).gameObject.name}");
                //platform.OnBeforePlatformMove += BeforePlatformMove;

                //BeforePlatformMove();
            }

        }
        else if (moveingPlatform != null)
        {
            platformDelta = Vector3.zero;
            //lastPlatformLocalPos = Vector3.zero;
            lastPlatformGlobalPos = Vector3.zero;
            //print($"removing moving platform from ${((Component)moveingPlatform).gameObject.name}");
            //moveingPlatform.OnBeforePlatformMove -= BeforePlatformMove;

            //platformDelta = Vector3.zero;
            if ((Component)moveingPlatform != null && ((Component)moveingPlatform).gameObject != null)
            {
                // Collider collider = ((Component)moveingPlatform).gameObject.GetComponent<Collider>();
                // if (collider != null && collider.sharedMaterial != null)
                // {
                //     collider.sharedMaterial.dynamicFriction = 0;
                //     collider.sharedMaterial.staticFriction = 0;
                // }
            }


            moveingPlatform = null;
            if(lastMovingPlatform != null)
            {
                //print($"adding moving platform from last moving platform for ${((Component)lastMovingPlatform).gameObject.name}");
                //lastMovingPlatform.OnBeforePlatformMove += BeforeLastPlatformMove;
            }
        }
        if (moveingPlatform == null && grounded)
        {
            platformDelta = Vector3.zero;
            //lastPlatformLocalPos = Vector3.zero;
            lastPlatformGlobalPos = Vector3.zero;
        }

        if (grounded && OnSteepSlope())
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position + Vector3.down * playerHeight * 0.25f, Vector3.down, out raycastHit, secondaryGroundCheckDistance, ~IgnoreGroundLayerMask))
            {
                if (raycastHit.normal != null)
                {
                    float angle = Vector3.Angle(Vector3.up, groundHit.normal);
                    if (angle < maxSlopeAngle + 30 && angle != 0)
                    {
                        grounded = true;
                        GravityDir = Vector3.down;
                        jumpOveride = true;
                        return;
                    }
                }
            }
            grounded = false;
            GravityDir = Vector3.down;
            return;
        }
        if (grounded && !exitingSlope)
        {
            if (GravityDir == -groundHit.normal.normalized) return;
            Vector3 oldGravityDir = GravityDir;
            GravityDir = -groundHit.normal.normalized;
            Vector3 velocity = rb.linearVelocity;

            Vector3 VelocityAlongOldGravity = Vector3.Project(velocity, oldGravityDir);
            Vector3 VelocityWithoutOldGravity = velocity - VelocityAlongOldGravity;

            float gravityMag = Vector3.Dot(velocity, oldGravityDir);
            Vector3 VelocityAlongNewGravity = GravityDir * gravityMag;

            Vector3 newVelocity = VelocityWithoutOldGravity + VelocityAlongNewGravity;

            rb.linearVelocity = newVelocity;
        }
        else
        {
            GravityDir = Vector3.down;
            if(moveingPlatform != null)
            {
                //print($"SECOND: removing moving platform from ${((Component)moveingPlatform).gameObject.name}");
                //moveingPlatform.OnBeforePlatformMove -= BeforePlatformMove;
                moveingPlatform = null;

                if (lastMovingPlatform != null)
                {
                    //print("What about this one??");
                    //lastMovingPlatform.OnBeforePlatformMove += BeforeLastPlatformMove;
                }
            }
        }
        if (wasGrounded && !grounded && readyToJump)
        {
            coyoteCheck = coyoteTime;    
        }
        jumpOveride = false;

        
    }

    Vector3 GetSurfaceNormal(Vector3 origin, Vector3 direction, float radius, float distance, LayerMask layerMask)
    {
        RaycastHit tempHit;
        if (Physics.Raycast(origin, direction, out tempHit, distance + radius, layerMask)) // Add radius to distance for safety
        {
            return tempHit.normal;
        }
        return Vector3.up; // Return up as a default if raycast fails
    }

    void RotateOrientation()
    {
        Quaternion old_rotation = transform.rotation;
        Vector3 old_forward = transform.forward;
        Vector3 horizontalVelocity = new Vector3(characterInput.GetMovementInput().x, 0, characterInput.GetMovementInput().z);
        if (horizontalVelocity == Vector3.zero)
        {
            tilt_amount = 0;
            return;
        }
        Quaternion forwardTarget = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
        if (forwardTarget == Quaternion.identity && horizontalVelocity.magnitude < 0.1f)
        {
            tilt_amount = 0;
            return;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, forwardTarget, rotationSpeed);
        int rot_direction = 1;
        if (Vector3.Cross(old_forward, transform.forward).y > 0)
        {
            rot_direction = -1;
        }
        tilt_amount = Quaternion.Angle(old_rotation, transform.rotation) * 3 * rot_direction;


    }

    void UpdateTilt()
    {
        current_tilt = Mathf.Lerp(current_tilt, Mathf.Clamp(tilt_amount, -maxTiltAngle, maxTiltAngle), 0.1f);
        //print("updating player tilt");
        //orientation.eulerAngles = new Vector3(orientation.eulerAngles.x, orientation.eulerAngles.y, current_tilt);
    }

    public void ApplyGravity(float multipler = 1f)
    {
        rb.AddForce(GravityDir * (GravityForce * multipler) * Time.deltaTime);
    }

    public void ApplyAirDrag(float amount)
    {
        rb.linearDamping = 0;
        Vector3 dragVelocity = rb.linearVelocity;

        dragVelocity.x /= (1 + amount * Time.deltaTime);
        dragVelocity.z /= (1 + amount * Time.deltaTime);

        rb.linearVelocity = new Vector3(dragVelocity.x, rb.linearVelocity.y, dragVelocity.z);
    }

    public void SimulateVelocity(Vector3 direction,float duration)
    {
        if(isSimulatingVelocity)
        {
            if(simulatedVelocityCoroutine != null)
                StopCoroutine(simulatedVelocityCoroutine);
            simulatedVelocityCoroutine = null;
        }
        simulatedVelocity = direction;
        simulatedVelocityCoroutine = StartCoroutine(ApplySimulatedVelocityCoroutine(duration));
    }

    public bool StopSimulatingVelocity()
    {
        if(isSimulatingVelocity)
        {
            if(simulatedVelocityCoroutine != null)
                StopCoroutine(simulatedVelocityCoroutine);
            simulatedVelocityCoroutine = null;
            simulatedVelocity = Vector3.zero;
            isSimulatingVelocity = false;
            return true;
        }
        return false;
    }

    void ApplySimulatedVelocity()
    {
        //rb.linearVelocity -= lastAppliedSimulatedVelocity;

        lastAppliedSimulatedVelocity = simulatedVelocity;
        if(lastAppliedSimulatedVelocity == Vector3.zero) return;
        //print($"applying simulated velocity : {lastAppliedSimulatedVelocity}");

        rb.MovePosition(transform.position + (lastAppliedSimulatedVelocity * Time.fixedDeltaTime));
    }

    IEnumerator ApplySimulatedVelocityCoroutine(float duration)
    {
        print("starting simulated velocity coroutine");
        isSimulatingVelocity = true;
        float currentDuration = 0;
        bool NoDuration = false;
        if(duration < 0)
        {
            NoDuration = true;
            duration = 1f;
        }
        while(currentDuration < duration)
        {
            if(!NoDuration)
                currentDuration += Time.fixedDeltaTime;
            if(grounded)
            {
                print("Player was grounded during simulated velocity");
                break;
            } 
            yield return new WaitForFixedUpdate();
        }
        simulatedVelocity = Vector3.zero;
        isSimulatingVelocity = false;
        print("stopping simulated velocity coroutine");
    }

    private void MyInput()
    {
        ControllerInput = characterInput.GetMovementInput();
        //ControllerInput = playerInput.actions["Move"].ReadValue<Vector2>();

    }

    private void MovePlayer()
    {
        // calculate movement direction
        Debug.DrawRay(transform.position, orientation.forward * 2, Color.red);
        moveDirection = ControllerInput;

        // on a walkable slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed, ForceMode.Force);
        }
        // on a steep slope
        else if (OnSteepSlope() && !exitingSlope)
        {
            //print("on steep slope");
            Vector3 wallNormal = new Vector3(groundHit.normal.x, 0, groundHit.normal.z);

            Vector3 moveInput = moveDirection.normalized;
            if (Vector3.Dot(moveInput, wallNormal) < 0f)
            {
                moveInput -= Vector3.Project(moveInput, wallNormal);
            }
            rb.AddForce(moveInput * moveSpeed, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {

        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > currentMaxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * currentMaxSpeed;
            }

           
        }
        else
        {

            
            //platformDelta = platformCurrentFrameDelta / Time.fixedDeltaTime;
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            Vector3 platformFlatVel = new Vector3(platformDelta.x, 0, platformDelta.z);

            float movingWithPlatformDot = Vector3.Dot(flatVel.normalized, platformFlatVel.normalized);

            float adjustedMaxSpeed = currentMaxSpeed + platformFlatVel.magnitude * movingWithPlatformDot;

            if (moveingPlatform == null && movingWithPlatformDot < -0.2f)
            {
                adjustedMaxSpeed = currentMaxSpeed;
            }

            if (moveingPlatform != null && flatVel.magnitude > currentMaxSpeed)
            {
                Vector3 limitedVel = rb.linearVelocity.normalized * currentMaxSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);

            }
            else if (flatVel.magnitude > currentMaxSpeed)
            {
                Vector3 limitedVel = rb.linearVelocity.normalized * adjustedMaxSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
            // Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            // Vector3 platformFlatVel = new Vector3(platformDelta.x, 0, platformDelta.z);
            // Vector3 relativeFlatVel = flatVel - platformFlatVel;

            // Vector3 platformDir = platformFlatVel.normalized;
            // float directionFactor = Vector3.Dot(relativeFlatVel.normalized, platformDir);
            // float relativeSpeed = Vector3.Dot(relativeFlatVel, platformDir);

            // // if (relativeFlatVel.sqrMagnitude > 0.001f && platformFlatVel.sqrMagnitude > 0.001f)
            // // {
            // //     directionFactor = Vector3.Dot(relativeFlatVel.normalized, platformDir);
            // // }

            // if (platformFlatVel.magnitude < 0.01f || flatVel.magnitude < 0.01f)
            //     directionFactor = 0f;

            // float directionBias = directionFactor * platformFlatVel.magnitude;
            // float adjustedMaxSpeed = currentMaxSpeed + directionBias;


            // // limit velocity if needed
            // if (Mathf.Abs(relativeSpeed) > Mathf.Abs(adjustedMaxSpeed) && Mathf.Sign(relativeSpeed) == Mathf.Sign(adjustedMaxSpeed))
            // {
            //     Vector3 limitedRelativeVel = platformDir * adjustedMaxSpeed;
            //     Vector3 newFlatVel = limitedRelativeVel + platformFlatVel;

            //     Vector3 newMax = new Vector3(newFlatVel.x, rb.linearVelocity.y, newFlatVel.z);
            //     rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, newMax, 1f); 
            //     //print(rb.linearVelocity.magnitude);
            // }

            // if (flatVel.magnitude > maxSpeed)
            // {
            //     Vector3 limitedVel = rb.linearVelocity.normalized * maxSpeed;
            //     //Vector3 newFlatVel = limitedRelativeVel + platformFlatVel;
            //     rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            // }
        }

    }

    private void Jump()
    {
        if (coyoteCheck <= 0)
        {
            if (!jumpOveride)
            {
                if (!grounded) return;
                if (OnSteepSlope()) return;
            }
        }
        if (!readyToJump) return;
        if (MovementControlledByAbility) return;
        OnJump?.Invoke();
        coyoteCheck = 0;
        readyToJump = false;
        GravityDir = Vector3.down;
        exitingSlope = true;
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float platformUpDelta = platformDelta.y;
        if(printStrings) print($"Platform up delta was {platformUpDelta}");
        rb.AddForce(transform.up * jumpForce + Vector3.up * platformUpDelta, ForceMode.Impulse);
        if (gameObject.tag == "Player") // So only the player makes jump sounds, and to prevent Rose from making manly grunts
        {
            AudioCollection audioCollectionJumpGrunt = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("Jump");
            AudioCollection audioCollectionJumpSound = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("JumpSound");
            ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, audioCollectionJumpGrunt, audioCollectionJumpSound);
        }
        if(moveingPlatform != null)
        {
            rb.AddForce(platformCurrentFrameDelta / Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    private void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }

    bool OnSlope()
    {
        if(forceSteepSlope) return false;
        if (groundHit.normal == null) return false;
        if (groundHit.transform != null && groundHit.transform.tag == "CantWalk") return false;
        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        return angle < maxSlopeAngle && angle != 0;
    }

    public bool OnSteepSlope()
    {
        if(forceSteepSlope) return true;
        if (groundHit.normal == null) return false;
        if (groundHit.transform != null && groundHit.transform.tag == "CantWalk") return true;
        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        return angle > maxSlopeAngle && angle + 1f != 0;

    }

    void SnapToGround()
    {

        Vector3 horVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (OnSlope() && !exitingSlope && horVel.magnitude > 0.5f)
        {
            RaycastHit raycastHit;
            if (Physics.SphereCast(transform.position + Vector3.down * playerHeight * 0.23f, playerRadius, GravityDir, out raycastHit, groundSnapRayDistance, ~IgnoreGroundLayerMask))
            {
                rb.MovePosition(transform.position + GravityDir * raycastHit.distance);
            }

        }
    }
   

    public Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public static bool CapsuleInsideTriggerWithTag(
        CapsuleCollider capsule,
        string requiredTag,
        LayerMask layerMask)
    {
        Transform t = capsule.transform;

        Vector3 center = t.TransformPoint(capsule.center);
        float radius = capsule.radius * Mathf.Max(t.lossyScale.x, t.lossyScale.z);

        float height = Mathf.Max(capsule.height * t.lossyScale.y, radius * 2f);
        float halfHeight = height * 0.5f - radius;

        Vector3 up = t.up;
        Vector3 point1 = center + up * halfHeight;
        Vector3 point2 = center - up * halfHeight;

        int count = Physics.OverlapCapsuleNonAlloc(
            point1,
            point2,
            radius,
            _trigger_check_results,
            layerMask,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < count; i++)
        {
            Collider c = _trigger_check_results[i];
            if (c != null && c.isTrigger && c.CompareTag(requiredTag))
                return true;
        }

        return false;
    }

    
    public IPlatformPassenger GetPassengerScript()
    {
        return this;
    }
}
