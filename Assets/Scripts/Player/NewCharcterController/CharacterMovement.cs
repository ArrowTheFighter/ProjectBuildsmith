using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed;
    public float moveSpeed;

    public float groundDrag;
    public float airDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    [HideInInspector] public bool readyToJump;
    [HideInInspector] public bool MovementControlledByAbility;
    bool jumpOveride;

    [Header("Rotation")]
    [SerializeField] float maxTiltAngle = 25f;
    [HideInInspector] public float tilt_amount;
    float current_tilt;


    [Header("Gravity")]
    [SerializeField] float GravityForce;
    public Vector3 GravityDir;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Slopes")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit groundHit;
    Vector3 groundNormal;
    RaycastHit steepSlopHit;
    RaycastHit slopeHit;
    bool exitingSlope;

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

    public Transform orientation;

    Vector3 ControllerInput;

    Vector3 moveDirection;

    [HideInInspector] public Rigidbody rb;

    List<PlayerAbility> playerAbilities = new List<PlayerAbility>();

    [Header("DEBUG")]
    [SerializeField] int TargetFPS = 60;

    [Header("Ability Actions")]
    public Action onDoubleJump;
    public Action OnJump;
    public Action OnDash;
    public Action OnDashStop;

    private void Start()
    {
        characterInput = GetComponent<ICharacterInput>();
        characterInput.OnJump += Jump;
        GravityDir = Vector3.down;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        Cursor.lockState = CursorLockMode.Locked;
        //playerInput = GetComponent<PlayerInput>();
        //playerInput.actions["Jump"].performed += Jump;
        AddAbility<DoubleJumpAbility>();
        AddAbility<DashAbility>();
    }

    private void Update()
    {
        //DEBUG
        Application.targetFrameRate = TargetFPS;


        // Ground check
        GroundCheck();
        // Handle Input
        MyInput();

        if (!MovementControlledByAbility)
        {
            // handle drag
            if (grounded)
                rb.linearDamping = groundDrag;
            else
                rb.linearDamping = 0;
        }

        foreach (PlayerAbility ability in playerAbilities)
        {
            ability.UpdateAbility();
        }
    }

    private void FixedUpdate()
    {
        if (!MovementControlledByAbility)
        {
            ApplyGravity();
            MovePlayer();
            RotateOrientation();
            SnapToGround();
            // Set max speed
            SpeedControl();
        }
        UpdateTilt();
        foreach (PlayerAbility ability in playerAbilities)
        {
            ability.FixedUpdateAbility();
        }
    }

    void AddAbility<T>() where T : PlayerAbility
    {

        var newAbility = gameObject.AddComponent<T>();
        newAbility.Initialize(this);
        playerAbilities.Add(newAbility);
    }

    void GroundCheck()
    {
        float distance = groundCheckDistance;
        if (rb.linearVelocity.y < -4) distance += 0.3f;
        grounded = Physics.SphereCast(transform.position + Vector3.down * playerHeight * 0.25f, playerRadius, Vector3.down, out groundHit, distance, ~IgnoreGroundLayerMask);

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
        transform.rotation = Quaternion.Slerp(transform.rotation, forwardTarget, 0.1f);
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
        orientation.eulerAngles = new Vector3(orientation.eulerAngles.x, orientation.eulerAngles.y, current_tilt);
    }

    public void ApplyGravity()
    {
        rb.AddForce(GravityDir * GravityForce);
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
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > maxSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

    }

    private void Jump()
    {
        if (!jumpOveride)
        {
            if (!grounded) return;
            if (OnSteepSlope()) return;
        }
        if (!readyToJump) return;
        OnJump?.Invoke();
        readyToJump = false;
        GravityDir = Vector3.down;
        exitingSlope = true;
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
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
        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        return angle < maxSlopeAngle && angle != 0;
    }

    public bool OnSteepSlope()
    {
        if (groundHit.normal == null) return false;
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

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
