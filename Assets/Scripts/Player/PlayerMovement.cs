using System;
using System.ComponentModel;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.UI;

[Obsolete]
public class PlayerMovement : MonoBehaviour
{
    //@Arrow
    //This entire script is copied from the Unity documentation and IS NOT what I want to use, it is only for testing!
    //Feel free to change all of it or completely remove it.

    private CharacterController controller;
    public bool can_control_player = true;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool _is_on_ground;
    [HideInInspector] public bool IsOnGround
    {
        get => _is_on_ground;
        set
        {
            if(_is_on_ground != value)
            {
                _is_on_ground = value;
                OnGroundChanged?.Invoke(_is_on_ground);
            }
        }
    }
    public event Action<bool> OnGroundChanged;
    private float playerSpeed = 10.0f;
    float speed_multiplier = 1;
    [SerializeField] float jumpHeight = 3.0f;
    [SerializeField] float gravityValue = -9.81f;
    [SerializeField] float no_clip_speed = 20;
    [SerializeField] bool can_dive;

    Vector3 current_move_direction;

    [SerializeField] LayerMask GroundCheckIgnoreLayers;
    [SerializeField] LayerMask WaterLayer;
    [SerializeField] float distance_to_ground;
    RaycastHit GroundCheckHitInfo;
    [SerializeField] float coyote_time;
    float coyote_time_stored;
    [SerializeField] Transform CameraTransform;
    [SerializeField] Transform Character_Model_Transform;
    [SerializeField] float move_acceleration;
    int max_double_jumps = 1;
    int double_jumps_remaining = 1;
    float tilt_amount;
    float current_tilt;
    bool no_clip;

    Vector3 hitNormal;
    Vector3 slope_slide_force;
    Vector3 last_safe_pos;
    Vector3 set_position;

    PlayerInput playerInput;

    IMoveingPlatform OnMovingPlatformScript;
    bool OnSlope;
    bool useGravity = true;
    //Zipline
    bool onZipline;
    bool zipline_is_up;
    float zipline_cooldown;
    SplineContainer rail_spline;
    float rail_float = 0;
    [SerializeField] float rail_kickoff_speed = 1;
    [SerializeField] float rail_speed = 1;
    [SerializeField] float rail_exit_force = 0.5f;
    [SerializeField] ParticleSystem rail_grind_particles;
    float rail_cooldown;
    Vector3 extra_vel;
    [SerializeField] GameObject Inventroy_Gameobject;

    //Audio
    PlayerAudio playerAudio;

    [Category("Debug")]
    [SerializeField] Transform move_direction_obj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Jump"].performed += Jump;
        playerInput.actions["NoClip"].performed += NoClip;
        playerInput.actions["Inventory"].performed += Toggle_Inventroy;
        playerInput.actions["Pause"].performed += Toggle_PauseMenu;
        playerInput.actions["Sprint"].performed += Dive;

        rail_grind_particles.Stop();
        playerAudio = GetComponent<PlayerAudio>();
        OnGroundChanged += Play_Landing_Sound;

        //WIPE THE FLAG LIST ON START
        FlagManager.wipe_flag_list();
    }

    // Update is called once per frame
    void Update()
    {
        // SPRINT STUFF
        //speed_multiplier = playerInput.actions["Sprint"].ReadValue<float>() > 0 ? sprint_speed_multiplier : 1;
        if (set_position != null && set_position != Vector3.zero)
        {
            transform.position = set_position;
            set_position = Vector3.zero;
            controller.enabled = true;
            return;
        }
        if (InWater())
        {
            print("im drowing!!!");
            transform.position = last_safe_pos;
            return;
        }
        // Stop the player from gaining downward velocity when grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = Mathf.Clamp(playerVelocity.y,-5f,5);
        }
        
        bool was_on_ground = false;
        Vector3 move = Vector3.zero;
        //If the player is on a zipline/grind
        
        if(can_control_player)
        {    
            // Get movement input
            move = new Vector3(
                playerInput.actions["Move"].ReadValue<Vector2>().x,
                0,
                playerInput.actions["Move"].ReadValue<Vector2>().y
                );

        }
            
        if(no_clip)
        {
            Vector3 cam_f = CameraTransform.forward.normalized * move.z;
            Vector3 cam_r = CameraTransform.right.normalized * move.x;

            if(playerInput.actions["Sprint"].ReadValue<float>() <= 0)
            {
                transform.position += (cam_f + cam_r) * no_clip_speed * Time.deltaTime;
                transform.position += new Vector3(0,playerInput.actions["Jump"].ReadValue<float>(),0) * no_clip_speed * Time.deltaTime;
            }else
            {
                transform.position += (cam_f + cam_r) * no_clip_speed * 10 * Time.deltaTime;
                transform.position += new Vector3(0,playerInput.actions["Jump"].ReadValue<float>(),0) * no_clip_speed * 10 * Time.deltaTime;
            }
            
            playerVelocity = Vector3.zero;
            return;
        }

        // Set the movement direction to the direction the camera is facing
        Vector3 camera_forward = CameraTransform.forward;
        camera_forward.y = 0;
        Vector3 forward_move = camera_forward.normalized * move.z;

        Vector3 camera_right = CameraTransform.right;
        camera_right.y = 0;
        Vector3 right_move = camera_right.normalized * move.x;

        //check if the player is on the ground before moving
        was_on_ground = OnGround();

        //Apply acceleration to movement
        Vector3 move_amount = forward_move + right_move;
        Rotate_Player(move_amount);

        current_move_direction = Vector3.Lerp(current_move_direction, move_amount * speed_multiplier,move_acceleration);
    
        //If we are on a slope then apply some force away from the slope so we slide
        
        //current_move_direction += slope_slide_force;

        if(extra_vel != Vector3.zero)
        {
            current_move_direction += extra_vel;
            if (OnGround() && !OnSlope)
            {
                extra_vel = Vector3.Lerp(extra_vel, Vector3.zero, 0.2f);
            }
            else
            {
                extra_vel = Vector3.Lerp(extra_vel, Vector3.zero, 0.05f);
            }
            
        }

        //Move the player
        if(!onZipline)
        {
            controller.Move( current_move_direction * Time.deltaTime * playerSpeed);
        }
        if(move_direction_obj != null)
        {
            move_direction_obj.position = transform.position + current_move_direction.normalized;
        }
        //rotate player

        //check if we are on a slope
        bool _onGround = OnGround();
        if (GroundCheckHitInfo.collider != null && GroundCheckHitInfo.collider.GetComponent<IMoveingPlatform>() != null)
        {
            if (OnMovingPlatformScript == null)
            {
                OnMovingPlatformScript = GroundCheckHitInfo.collider.GetComponent<IMoveingPlatform>();
                OnMovingPlatformScript.OnPlatformMove += Moving_Platform_Update;
            }
            
        }
        else
        {
            if (OnMovingPlatformScript != null)
            {
                OnMovingPlatformScript.OnPlatformMove -= Moving_Platform_Update;
                OnMovingPlatformScript = null;
            }
        }
        OnSlope = Vector3.Angle(Vector3.up, GroundCheckHitInfo.normal) >= controller.slopeLimit;
        //reset slop variable
        //hitNormal = Vector3.up;
        slope_slide_force = Vector3.zero;
        //if you walk off a platform, reset your downward velocity
        if( was_on_ground 
            && 
            !Physics.SphereCast(transform.position + Vector3.down * 0.5f,controller.radius,Vector3.down,out GroundCheckHitInfo, distance_to_ground,~GroundCheckIgnoreLayers)
            && playerVelocity.y < 0.1f 
            && playerVelocity.y > -3f
        ){
            playerVelocity.y = 0;
        }

        if(_onGround && !OnSlope && playerVelocity.y < 0.1f)
        {
            IsOnGround = true;
            coyote_time_stored = Time.time + coyote_time;
            double_jumps_remaining = max_double_jumps;
            if(!Physics.Raycast(transform.position + Vector3.up * 2.25f,Vector3.down,3.5f,WaterLayer))
            {
                last_safe_pos = transform.position;
            }
        }else
        {
            IsOnGround = false;
        }
        //Apply Gravity
        if(useGravity)
        {
            Vector3 up = Vector3.up;
            if (OnSlope && playerVelocity.y < 0.1f) 
            {
                //up = new Vector3(-hitNormal.x, hitNormal.y, -hitNormal.z);
                up = Vector3.ProjectOnPlane(Vector3.up,GroundCheckHitInfo.normal);
                //up = Vector3.Cross(Vector3.Cross(hitNormal,Vector3.down),hitNormal).normalized;
                Debug.DrawRay(transform.position,up.normalized * 4f,Color.red);
                //slope_slide_force.x = (1f - hitNormal.y) * 0.5f * hitNormal.x * 0.5f * (1f - slideFriction);
                //slope_slide_force.z = (1f - hitNormal.y) * 0.5f * hitNormal.z * 0.5f * (1f - slideFriction);
            }else
            {
                playerVelocity = Vector3.Lerp(playerVelocity,new Vector3(0,playerVelocity.y,0),0.1f);
            }
            playerVelocity += up * gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
        GroundCheckHitInfo = new RaycastHit();
        
        //Set the player's animation values
            Animator Character_Model_Animator = Character_Model_Transform.GetComponent<Animator>();
            Character_Model_Animator.SetFloat("Speed_Blend",current_move_direction.magnitude);
            Character_Model_Animator.SetBool("OnGround",_onGround && !OnSlope && playerVelocity.y < 0.1f);
            //tilt the player visuals when rotating
            current_tilt = Mathf.Lerp(current_tilt,tilt_amount,0.1f);
            Character_Model_Transform.eulerAngles = new Vector3(Character_Model_Transform.eulerAngles.x,Character_Model_Transform.eulerAngles.y, current_tilt);

        if(onZipline)
        {
            int dir = 1;
            if(!zipline_is_up) dir = -1;

            float new_point;
            float distance = (rail_speed / rail_spline.Spline.GetLength() * Time.deltaTime) * dir;
            SplineUtility.GetPointAtLinearDistance(rail_spline.Spline,rail_float,distance,out new_point);
            Vector3 new_rail_pos = rail_spline.Spline.EvaluatePosition(new_point);
            Vector3 rail_world_pos = rail_spline.transform.TransformPoint(new_rail_pos);
            Vector3 local_tangent = SplineUtility.EvaluateTangent(rail_spline.Spline,rail_float);
            Vector3 world_tangent = rail_spline.transform.TransformDirection(local_tangent);
            
            Vector3 required_to_point = (rail_world_pos + Vector3.up * 1.25f) - transform.position;

            controller.Move(required_to_point);
            //transform.position = rail_world_pos + Vector3.up * 1.25f;
            
            
            Vector3 rotated_tangent = Quaternion.Euler(0,90,0) * world_tangent * dir;
            Quaternion rotation = Quaternion.LookRotation(rotated_tangent);
            Character_Model_Transform.rotation = rotation;
            rail_float += rail_speed / rail_spline.Spline.GetLength() * Time.deltaTime * dir;
            rail_float = Mathf.Clamp01(rail_float);
            if(rail_float >= 0.999f || rail_float <= 0.001f)
            {
                StopZipline();
            }
        }
    }

    void Toggle_Inventroy(InputAction.CallbackContext context)
    {
        CanvasGroup inventory_canvasGroup = Inventroy_Gameobject.GetComponent<CanvasGroup>();
        if(inventory_canvasGroup.alpha <= 0)
        {
            if (ScriptRefrenceSingleton.instance.gameplayUtils.GetOpenMenu()) return;
            ScriptRefrenceSingleton.instance.gameplayUtils.OpenMenu();
            inventory_canvasGroup.alpha = 1;
            inventory_canvasGroup.blocksRaycasts = true;
            inventory_canvasGroup.interactable = true;
        }
        else
        {
            inventory_canvasGroup.alpha = 0;
            inventory_canvasGroup.blocksRaycasts = false;
            inventory_canvasGroup.interactable = false;
            ScriptRefrenceSingleton.instance.gameplayUtils.CloseMenu();
        }
    }

    void Toggle_PauseMenu(InputAction.CallbackContext context)
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.Toggle_Pause_Menu();
    }

    void Dive(InputAction.CallbackContext action)
    {
        if (!can_dive) return;
        if (!OnGround() || !can_control_player) return;
        Freeze_Movement();
        Invoke("Unfreeze_Movement", 0.5f);
        extra_vel = transform.forward * 0.5f;
        playerVelocity.y = Mathf.Sqrt(1 * -2.0f * gravityValue);
        Animator Character_Model_Animator = Character_Model_Transform.GetComponent<Animator>();
        Character_Model_Animator.SetTrigger("DoubleJump");
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Animator Character_Model_Animator = Character_Model_Transform.GetComponent<Animator>();
        if (onZipline)
        {
            playerVelocity.y = 0f;
            double_jumps_remaining--;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            Character_Model_Animator.SetTrigger("DoubleJump");
            StopZipline(false);
            playerAudio.PlayClip(0);
            return;
        }
        if (!can_control_player) return;
        if (no_clip) return;


        //If the player is grounded then do a normal_jump
        if (coyote_time_stored > Time.time && !OnSlope)
        {
            coyote_time_stored = 0;
            playerVelocity.y = 0f;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            Character_Model_Animator.SetBool("Jumped", true);
            Invoke("reset_jump", 0.05f);
            playerAudio.PlayClip(0);
        }
        else if (double_jumps_remaining > 0)
        {
            playerVelocity.y = 0f;
            double_jumps_remaining--;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            Character_Model_Animator.SetTrigger("DoubleJump");
            playerAudio.PlayClip(0);
        }
    }

    void reset_jump()
    {
        Animator Character_Model_Animator = Character_Model_Transform.GetComponent<Animator>();
        Character_Model_Animator.SetBool("Jumped",false);
    }

    void Play_Landing_Sound(bool should_play)
    {
        if(should_play)
        {
            playerAudio.PlayClip(1,1.2f,0.7f);
        }
    }

    void NoClip(InputAction.CallbackContext context)
    {
        if (!no_clip && playerInput.actions["No_Clip_Modifer"].ReadValue<float>() > 0)
        {
            no_clip = true;
        }
        else
        {
            no_clip = false;
         }
    }

    //Check if you are on the ground
    bool OnGround()
    {
        if(Physics.SphereCast(transform.position + Vector3.down * 0.5f,controller.radius,Vector3.down,out GroundCheckHitInfo, distance_to_ground,~GroundCheckIgnoreLayers)) return true;
        //if(Physics.Raycast(transform.position,Vector3.down,distance_to_ground,~GroundCheckIgnoreLayers)) return true;
        //if(Physics.Raycast(transform.position - transform.forward,Vector3.down,distance_to_ground,~GroundCheckIgnoreLayers)) return true;
        return false;
    }

    bool InWater()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(transform.position + Vector3.up,0.5f,WaterLayer);
        if(colliders.Length > 0)
        {
            if(colliders[0].tag == "Water")
            {
                return true;
            }
        }
        return false;
    }
    void Rotate_Player(Vector3 direction)
    {
        Quaternion old_rotation = transform.rotation;
        Vector3 old_forward = transform.forward;
        Vector3 horizontalVelocity = new Vector3(direction.x, 0, direction.z);
        if(horizontalVelocity == Vector3.zero)
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
        transform.rotation = Quaternion.Slerp(transform.rotation, forwardTarget,0.1f);
        int rot_direction = 1;
        if(Vector3.Cross(old_forward,transform.forward).y > 0)
        {   
            rot_direction = -1;
        }
        tilt_amount = Quaternion.Angle(old_rotation,transform.rotation) * 3 * rot_direction;
       
    }

    void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        hitNormal = hit.normal;
    }

    public void Set_Player_Position(Vector3 pos)
    {
        controller.enabled = false;
        set_position = pos;
    }

    void Moving_Platform_Update(Vector3 delta_move)
    {
        controller.Move(delta_move);
    }

    void Freeze_Movement()
    {
        can_control_player = false;
     }

    void Unfreeze_Movement()
    {
        can_control_player = true;
     }

    public void AddDoubleJump(int amount = 1)
    {
        print("Increasing double jumps by " + amount);
        max_double_jumps += amount;
    }

    [ContextMenu("Add Double Jump")]
    public void AddSingleDoubleJump()
    {
        AddDoubleJump();
     }
    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.tag == "Zipline")
    //     {
    //         print("zipline");
    //         zipline_direction = other.transform.up;
    //         StartZipline();
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if(other.tag == "Zipline")
    //     {
    //         if(onZipline)
    //         {
    //             StopZipline();
    //         }
    //     }
    // }

    public void StartZipline(Vector3 zipline_forward, SplineContainer spline)
    {
        if (rail_cooldown > Time.time) return;
        if (onZipline) return;
        if (playerVelocity.y > 0.5f) return;

        if (!FlagManager.Get_Flag_Value("Can_RailGrind"))
        {
            print("Rail grinding not unlocked");
            rail_spline = spline;


            float3 v3_kickoff;
            SplineUtility.GetNearestPoint(rail_spline.Spline, transform.position, out v3_kickoff, out rail_float, 10, 10);

            Vector3 rail_pos_kickoff = rail_spline.Spline.EvaluatePosition(rail_float);
            Vector3 world_pos_kickoff = rail_spline.transform.TransformPoint(rail_pos_kickoff);

            Vector3 dir = transform.position - world_pos_kickoff;
            dir = new Vector3(dir.x, .1f, dir.z);
            if (can_control_player)
            {
                 extra_vel = dir.normalized * rail_kickoff_speed;
                Freeze_Movement();
                Invoke("Unfreeze_Movement", 0.5f);
             }
           
            return;
        }
        Animator Character_Model_Animator = Character_Model_Transform.GetComponent<Animator>();
        Character_Model_Animator.SetTrigger("StartRailGrind");

        onZipline = true;
        useGravity = false;
        can_control_player = false;
        //Character_Model_Transform.forward = Vector3.Cross(Vector3.up,zipline_direction);
        rail_spline = spline;

        Vector3 local_tangent = SplineUtility.EvaluateTangent(rail_spline.Spline, rail_float);
        Vector3 world_tangent = rail_spline.transform.TransformDirection(local_tangent);

        zipline_is_up = true;
        if (Vector3.Angle(world_tangent, transform.forward) > 90)
        {
            zipline_is_up = false;
        }

        float3 v3;
        SplineUtility.GetNearestPoint(rail_spline.Spline, transform.position, out v3, out rail_float, 10, 10);

        Vector3 rail_pos = rail_spline.Spline.EvaluatePosition(rail_float);
        Vector3 world_pos = rail_spline.transform.TransformPoint(rail_pos);
        //transform.position = world_pos + Vector3.up * 1.25f;
        rail_grind_particles.Play();
    }

    public void StopZipline(bool jump = true)
    {
        if(!onZipline) return;
        if(jump) 
        {
            Jump(new InputAction.CallbackContext());
            return;
        }
        onZipline = false;
        useGravity = true;
        can_control_player = true;
        Character_Model_Transform.forward = transform.forward;
        Vector3 local_tangent = SplineUtility.EvaluateTangent(rail_spline.Spline,rail_float);
        Vector3 world_tangent = rail_spline.transform.TransformDirection(local_tangent);
        if(!zipline_is_up) world_tangent = -world_tangent;
        extra_vel = world_tangent * rail_exit_force;
        rail_cooldown = Time.time + 0.5f;
        world_tangent = new Vector3(world_tangent.x,0,world_tangent.z);
        Quaternion.LookRotation(world_tangent, Vector3.up);
        
        rail_grind_particles.Stop();
    }

}
