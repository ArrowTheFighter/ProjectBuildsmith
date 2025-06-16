using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float groundDrag = 2f;
    [SerializeField] float maxSpeed = 10;

    [Header("Jumping")]
    [SerializeField] float jumpForce;
    float jumpCooldown;
    public Action OnJump;

    [Header("Extra Gravity")]
    [SerializeField] float extraGravity = 5;


    [Header("Ground Check")]
    bool onGround;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask groundCheckIgnoreLayers;
    RaycastHit groundCheckHit;

    //Rotation
    [Header("Rotation")]
    [SerializeField] float TiltSpeed;
    [SerializeField] float RotationSpeed = 0.2f;
    float tiltAmount;
    float currentTilt;

    [Header("Relations")]
    [SerializeField] Transform visuals;
    [SerializeField] Collider CharacterCollider;
    //Input
    ICharacterInput characterInput;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterInput = GetComponent<ICharacterInput>();

        if (characterInput == null)
        {
            Debug.LogError(gameObject.name + " Doesnt have a ICharacterInput component. Please add one.", gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveCharacter();

        RotateCharacter(rb.linearVelocity);
        TiltPlayer();

        onGround = OnGround();
        ApplyDrag();
        Jump();
        ExtraGravity();
        SpeedControl();
    }

    void ApplyDrag()
    {
        if (onGround)
        {
            Vector3 horVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            float speed = horVel.magnitude;
            float dragAmount = groundDrag * Time.deltaTime;

            if (speed > 0)
            {
                speed = Mathf.Max(0f, speed - dragAmount);
                horVel = horVel.normalized * speed;
            }
            rb.linearVelocity = new Vector3(horVel.x, rb.linearVelocity.y, horVel.z);
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    void MoveCharacter()
    {
        Vector3 moveInput = characterInput.GetMovementInput();
        rb.AddForce(moveInput * moveSpeed * 10 * Time.deltaTime, ForceMode.Force);
    }

    void RotateCharacter(Vector3 direction)
    {
        Quaternion old_rotation = transform.rotation;
        Vector3 old_forward = transform.forward;
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z);
        Quaternion newRotation = Quaternion.identity;
        if (horizontalVelocity.magnitude <= 0.1f)
        {
            tiltAmount = 0;
            return;
        }
        Quaternion forwardTarget = Quaternion.LookRotation(horizontalVelocity.normalized, Vector3.up);
        if (forwardTarget == Quaternion.identity && horizontalVelocity.magnitude < 0.1f)
        {
            tiltAmount = 0;
            return;
        }
        if (forwardTarget != Quaternion.identity && horizontalVelocity.magnitude > 0.1f)
        {
            print("rotating player");
            newRotation = Quaternion.Slerp(transform.rotation, forwardTarget, RotationSpeed);
            rb.MoveRotation(newRotation);
        }
        int rot_direction = 1;
        if (Vector3.Cross(old_forward, newRotation * Vector3.forward).y > 0)
        {
            rot_direction = -1;
        }
        tiltAmount = Mathf.Clamp(Quaternion.Angle(old_rotation, newRotation) * 3 * rot_direction, -25f, 25f);
    }

    void TiltPlayer()
    {
        currentTilt = Mathf.Lerp(currentTilt, tiltAmount, TiltSpeed);
        visuals.eulerAngles = new Vector3(visuals.eulerAngles.x, visuals.eulerAngles.y, currentTilt);
    }

    bool OnGround()
    {
        switch (CharacterCollider)
        {
            case CapsuleCollider capsuleCollider:
                if (Physics.Raycast(transform.position,
                        Vector3.down,
                        out groundCheckHit,
                        groundCheckDistance,
                        ~groundCheckIgnoreLayers)
                        ) return true;
                // if (Physics.SphereCast(transform.position,
                //     capsuleCollider.radius,
                //     Vector3.down,
                //     out groundCheckHit,
                //     groundCheckDistance,
                //     ~groundCheckIgnoreLayers)
                //     ) return true;
                break;
            default:
                Debug.LogError("Collider Type was not found! Please update CharacterMovement Script", gameObject);
                break;
        }
        return false;
    }

    void Jump()
    {

        if (characterInput.GetJumpInput())
        {
            print("Charater tryig to jump");
            if (Time.time < jumpCooldown) return;
            if (onGround)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpCooldown = Time.time + 0.1f;
                OnJump?.Invoke();
            }
        }
    }

    void ExtraGravity()
    {
        if (!onGround)
        {
            rb.AddForce(Vector3.down * extraGravity * Time.deltaTime, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 horVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (horVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVel = horVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public Vector3 GetVelocity()
    {
        return rb.linearVelocity;
    }

    public bool GetOnGround()
    {
        return onGround;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
     }
}
