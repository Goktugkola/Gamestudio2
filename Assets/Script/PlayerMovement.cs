using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Assingables")]
    public Transform playerCam;
    public Transform orientation;
    private Rigidbody rb;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4500f;
    [SerializeField] public float maxSpeed = 20f;
    [SerializeField] private float jumpForce = 550f;
    [SerializeField] private float coyoteTimeDuration = 0.15f; // Time window to jump after leaving ground
    [SerializeField] private float aircontrol = 1f;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    [Header("Sliding Settings")]
    [SerializeField] private float slideForce = 400;
    [SerializeField] private float slideCounterMovement = 0.2f;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1, 1);
    public int gravityMultiplier = 25;
    [Header("Movement State")] 
    public MovementState State;
        [SerializeField]public enum MovementState
    {
        Running, // Changed to PascalCase
        Jumping,
        Dashing,
        Falling, // Changed to PascalCase
        Grappling,
        WallRunning,
        Crouching,
        Swinging,
        Mantling,
        FirstSection // Changed to PascalCase
    }
    [Header("Swinging Settings")]
    public float swingControl = 10f;
    [SerializeField] private float extraMomentum = 1.5f;

    [Header("For Ground Check")]
    public LayerMask whatIsGround;
    public enum SurfaceType
    {
        Grass,
        Stone,
        Wood,
    }
    public SurfaceType currentSurface;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float groundCheckDistance = 0.4f; // Moved closer to other ground check variables
    public bool grounded; // Moved from Movement Settings
    private bool wasGrounded; // To track if player was grounded in the previous FixedUpdate

    [Header("State Flags")] // Added Header for clarity
    private bool jumping = false; // Initialize explicitly
    private bool dashing = false; // Initialize explicitly
    private bool crouching = false;
    private bool readyToJump = true;
    private bool shiftTogglePressed = false; // Moved closer to related variable
    public bool canShiftToggle = false;
    private float coyoteTimeTimer; // Timer for coyote time
    private bool coyoteTimeActive; // Flag if coyote time is currently active

    [Header("Input")] // Added Header for clarity
    private float HorizontalInput;
    private float VerticalInput;

    [Header("Cooldowns")] // Added Header for clarity
    private float jumpCooldown = 0.25f;


    [Header("Physics Related")] // Added Header for clarity
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector; // Consider initializing if it has a default state

    // Public state variable
    // Consider making setter private if only changed internally

    // Enum defining player movement states



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Validate required components are assigned
        if (playerCam == null) Debug.LogError("Player Camera not assigned!", this);
        if (orientation == null) Debug.LogError("Orientation Transform not assigned!", this);
        if (groundCheck == null) Debug.LogError("Ground Check Transform not assigned!", this);
    }

    void Start()
    {
        State = MovementState.FirstSection; // Use PascalCase enum member
        playerScale = transform.localScale; // Initialize playerScale in Start
    }
    private void FixedUpdate()
    {
        wasGrounded = grounded; // Store the grounded state from the *previous* FixedUpdate cycle
        UpdateGrounding();      // Update the current grounded state

        // If currently mantling, let LedgeJump handle the state exclusively
        if (State == MovementState.Mantling)
        {

            return; 
        }

        // Coyote Time Logic
        if (wasGrounded && !grounded && State != MovementState.Jumping && State != MovementState.Dashing)
        {
            // Player just left the ground, not by jumping or dashing
            coyoteTimeActive = true;
            coyoteTimeTimer = coyoteTimeDuration;
            Debug.Log("Coyote Time Activated. Duration: " + coyoteTimeDuration);
        }

        if (coyoteTimeActive)
        {
            coyoteTimeTimer -= Time.fixedDeltaTime;
            // Debug.Log("Coyote Time Active. Timer: " + coyoteTimeTimer); // Optional: Log every frame coyote time is active
            if (coyoteTimeTimer <= 0f)
            {
                coyoteTimeActive = false; // Coyote time expired
                Debug.Log("Coyote Time Expired.");
            }
        }

        if (grounded)
        {
            if (coyoteTimeActive) // Log only if it was active and now grounded
            {
                Debug.Log("Coyote Time Deactivated due to grounding.");
            }
            coyoteTimeActive = false; // Reset coyote time upon landing
        }

        HandleMovement();
        if (grounded && !jumping && !dashing && !crouching && State != MovementState.FirstSection)
        {
            State = MovementState.Running;
        }
        // Ensure this condition doesn't conflict if Mantling state is handled above
        if (!grounded && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !dashing && State != MovementState.WallRunning && State != MovementState.Mantling) 
        {
            State = MovementState.Falling;
        }
    }
    private void Update()
    {
        HandleInput();
    }
    private void HandleInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        if (State == MovementState.FirstSection)
        {
            HorizontalInput = Mathf.Clamp(HorizontalInput, 0, 0);
            VerticalInput = Mathf.Clamp(VerticalInput, 0, 0.5f);
            jumping = false;
        }
        else
        {
            jumping = Input.GetButton("Jump");
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !shiftTogglePressed && canShiftToggle)
        {
            shiftTogglePressed = true;
            maxSpeed = (maxSpeed == 15f) ? 2f : 15f;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            shiftTogglePressed = false;
        }

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl) && State != MovementState.FirstSection)
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl) && State != MovementState.FirstSection)
            StopCrouch();
    }

    private void StartCrouch()
    {
        transform.localScale = crouchScale;
        if (grounded)
        {
            rb.AddForce(orientation.transform.forward * slideForce);
        }
    }

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
    private void HandleMovement()
    {
        // Cache Time.deltaTime
        float dt = Time.deltaTime;

        // Apply extra gravity
        if (!grounded && State != MovementState.WallRunning)
        {
            ApplyGravity(); // ApplyGravity already uses Time.deltaTime
        }

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;


        //Counteract sliding and sloppy movement
        CounterMovement(HorizontalInput, VerticalInput, mag); // CounterMovement uses Time.deltaTime

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        // Consider using sqrMagnitude here if profiling shows it's beneficial,
        // but FindVelRelativeToLook already calculates magnitude.
        if (HorizontalInput > 0 && xMag > maxSpeed) HorizontalInput = 0;
        if (HorizontalInput < 0 && xMag < -maxSpeed) HorizontalInput = 0;
        if (VerticalInput > 0 && yMag > maxSpeed) VerticalInput = 0;
        if (VerticalInput < 0 && yMag < -maxSpeed) VerticalInput = 0;

        // Cache orientation vectors
        Transform orientationTransform = orientation.transform;
        Vector3 forward = orientationTransform.forward;
        Vector3 right = orientationTransform.right;

        if (grounded)
        {
            if (!crouching)
            {
                rb.AddForce(forward * VerticalInput * moveSpeed * dt);
                rb.AddForce(right * HorizontalInput * moveSpeed * dt);
            }

        }
        else if (State == MovementState.Falling)
        {
            // Precompute multiplier if beneficial
            float airForceMultiplier = aircontrol * moveSpeed * dt / 10f;
            rb.AddForce(forward * VerticalInput * airForceMultiplier);
            rb.AddForce(right * HorizontalInput * airForceMultiplier);
        }
        else if (State == MovementState.Swinging)
        {
            // Precompute multiplier if beneficial
            float swingForceMultiplier = moveSpeed * swingControl * dt / 100f;
            rb.AddForce(forward * VerticalInput * swingForceMultiplier * extraMomentum); // Apply extraMomentum here
            rb.AddForce(right * HorizontalInput * swingForceMultiplier);
        }

    }

    private void ApplyGravity()
    {
        // Cache gravity calculation if gravityMultiplier is constant during gameplay
        rb.AddForce(Vector3.down * Time.deltaTime * gravityMultiplier * 10);
    }


    private void Jump()
    {
        // State = MovementState.Jumping; // Moved to be set only if jump is successful
        if (!readyToJump) return; // Player must be ready (jump cooldown)

        if (grounded || coyoteTimeActive) // Allow jump if grounded OR coyote time is active
        {
            State = MovementState.Jumping; // Set state to Jumping as a jump is happening
            readyToJump = false;

            if (coyoteTimeActive)
            {
                coyoteTimeActive = false; // Consume coyote time if it was used for this jump
                Debug.Log("Coyote Time Consumed by Jump.");
            }
            else if (grounded)
            {
                Debug.Log("Standard Ground Jump.");
            }

            // Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            // If jumping while falling, reset y velocity
            Vector3 vel = rb.linearVelocity;
            if (vel.y < 0.5f)
            {
                rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
            }
            else if (vel.y > 0)
            {
                rb.linearVelocity = new Vector3(vel.x, vel.y / 2, vel.z);
            }

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;
    }



    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.linearVelocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        // Cache Time.deltaTime
        float dt = Time.deltaTime;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * dt * -rb.linearVelocity.normalized * slideCounterMovement);
            return;
        }

        // Cache orientation vectors
        Transform orientationTransform = orientation.transform;
        Vector3 right = orientationTransform.right;
        Vector3 forward = orientationTransform.forward;

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * right * dt * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * forward * dt * -mag.y * counterMovement);
        }

        //Limit diagonal running using sqrMagnitude
        float maxSpeedSquared = maxSpeed * maxSpeed;
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVelocity.sqrMagnitude > maxSpeedSquared)
        {
            float fallspeed = rb.linearVelocity.y;
            Vector3 n = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    private bool IsFloor(Vector3 normal) =>
    Vector3.Angle(Vector3.up, normal) < maxSlopeAngle;
    private void UpdateGrounding()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, whatIsGround,
            QueryTriggerInteraction.Ignore);

        if (grounded && Physics.Raycast(groundCheck.position, Vector3.down,
            out RaycastHit hit, groundCheckDistance))
        {

            grounded = IsFloor(hit.normal);
            normalVector = hit.normal;
        }
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit surfacehit, groundCheckDistance+2f))
        {
            print("Surface Hit: " + surfacehit.collider.tag);
            currentSurface = surfacehit.collider.tag switch
            {
                "Grass" => SurfaceType.Grass,
                "Stone" => SurfaceType.Stone,
                "Wood" => SurfaceType.Wood,
                _ => SurfaceType.Stone, // Default
            };
        }
    }
}
