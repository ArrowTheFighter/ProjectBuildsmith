using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
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
        if (!MovementControlledByAbility)
        {
            MovePlayer();
            RotateOrientation();
            SnapToGround();
            // Set max speed
            SpeedControl();
        }
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
        if (moveingPlatform == null)
        {
            if(lastMovingPlatform != null)
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
        if (lastPlatformGlobalPos == Vector3.zero) return;
            //currentGlobalPos = moveingPlatform.getInterfaceTransform().TransformPoint(lastPlatformLocalPos);
        platformCurrentFrameDelta = currentGlobalPos - lastPlatformGlobalPos;

        lastPlatformGlobalPos = Vector3.zero;
        lastPlatformLocalPos = Vector3.zero;
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
        Gizmos.DrawWireSphere(transform.position + Vector3.down * playerHeight * 0.25f, playerRadius);
        Gizmos.DrawLine(transform.position + Vector3.down * playerHeight * 0.25f + Vector3.down * playerRadius, transform.position + Vector3.down * playerHeight * 0.25f + Vector3.down * playerRadius + Vector3.down * groundCheckDistance);
    }


    void GroundCheck()
    {
        if (moveingPlatform == null && lastMovingPlatform != null)
        {
            lastMovingPlatform.OnBeforePlatformMove -= BeforeLastPlatformMove;
        }
        lastMovingPlatform = moveingPlatform;

        bool wasGrounded = grounded;
        float distance = groundCheckDistance;
        if (rb.linearVelocity.y < -4) distance += 0.3f;
        grounded = Physics.SphereCast(transform.position + Vector3.down * playerHeight * 0.25f, playerRadius, Vector3.down, out groundHit, distance, ~IgnoreGroundLayerMask);
        if (groundHit.transform != null && groundHit.transform.tag == "CantWalk")
        {
            grounded = false;
        }

        if (grounded && groundHit.collider.TryGetComponent(out IMoveingPlatform platform))
        {
            if (moveingPlatform != platform)
            {
                //print("adding platform move");

                if(moveingPlatform == null)
                {
                    print("landed on moving platform");
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

                platform.OnBeforePlatformMove += BeforePlatformMove;

                BeforePlatformMove();
            }

        }
        else if (moveingPlatform != null)
        {
            moveingPlatform.OnBeforePlatformMove -= BeforePlatformMove;
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
                lastMovingPlatform.OnBeforePlatformMove += BeforeLastPlatformMove;
            }
        }
        if (moveingPlatform == null && grounded)
        {
            platformDelta = Vector3.zero;
            lastPlatformLocalPos = Vector3.zero;
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
                moveingPlatform.OnBeforePlatformMove -= BeforePlatformMove;
                moveingPlatform = null;

                if (lastMovingPlatform != null)
                {
                    lastMovingPlatform.OnBeforePlatformMove += BeforeLastPlatformMove;
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
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        AudioCollection audioCollectionJumpGrunt = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("Jump");
        AudioCollection audioCollectionJumpSound = ScriptRefrenceSingleton.instance.playerAudioManager.GetAudioClipByID("JumpSound");
        ScriptRefrenceSingleton.instance.soundFXManager.PlayAllSoundCollection(transform, audioCollectionJumpGrunt, audioCollectionJumpSound);
        
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
        if (groundHit.normal == null) return false;
        if (groundHit.transform != null && groundHit.transform.tag == "CantWalk") return false;
        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        return angle < maxSlopeAngle && angle != 0;
    }

    public bool OnSteepSlope()
    {
        if (groundHit.normal == null) return false;
        if (groundHit.transform != null && groundHit.transform.tag == "CantWalk") return true;
        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        return angle > maxSlopeAngle && angle + 1f != 0;

    }

    void SnapToGround()
    {
        if (OnSlope() && !exitingSlope)
        {
            RaycastHit raycastHit;
            if (Physics.SphereCast(transform.position + Vector3.down * playerHeight * 0.25f, playerRadius, GravityDir, out raycastHit, groundSnapRayDistance, ~IgnoreGroundLayerMask))
            {
                rb.MovePosition(transform.position + GravityDir * raycastHit.distance);
            }

        }
    }

    public Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
