using System;
using UnityEditorInternal;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Assingables")]
    public Transform playerCam;
    public Transform orientation;
    private Rigidbody rb;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4500;
    [SerializeField] public float maxSpeed = 20;
    [SerializeField] private float jumpForce = 550f;
    [SerializeField] private float aircontrol = 1f;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    public bool grounded;
    [Header("Sliding Settings")]
    [SerializeField] private float slideForce = 400;
    [SerializeField] private float slideCounterMovement = 0.2f;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    [SerializeField] private Vector3 playerScale;
    public int gravityMultiplier = 25;
    [Header("Swinging Settings")]
    public float swingControl = 10f;
    [SerializeField] private float extraMomentum = 1.5f;

    [Header("For Ground Check")]
    public LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool jumping, dashing, crouching = false;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    private float groundCheckDistance = 0.4f;
    private float HorizontalInput, VerticalInput;
    //Sliding
    // 
    //     public float slideCounterMovement = 0.2f;
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    public MovementState State;

    private bool shiftTogglePressed = false;

    public enum MovementState
    {
        running,
        Jumping,
        Dashing,
        falling,
        Grappling,
        WallRunning,
        Crouching,
        Swinging
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        if (grounded && !jumping && !dashing && !crouching)
        {
            State = MovementState.running;
        }
        if (!grounded && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !dashing && State != MovementState.WallRunning)
        {
            State = MovementState.falling;
        }
        UpdateGrounding();
    }
    private void Update()
    {
        HandleInput();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !shiftTogglePressed)
        {
            shiftTogglePressed = true;
            maxSpeed = (maxSpeed == 15f) ? 2f : 15f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftTogglePressed = false;
        }

    }

    private void HandleInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
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

        // Apply extra gravity
        if (!grounded && State != MovementState.WallRunning)
        {
            ApplyGravity();
        }

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;


        //Counteract sliding and sloppy movement
        CounterMovement(HorizontalInput, VerticalInput, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (HorizontalInput > 0 && xMag > maxSpeed) HorizontalInput = 0;
        if (HorizontalInput < 0 && xMag < -maxSpeed) HorizontalInput = 0;
        if (VerticalInput > 0 && yMag > maxSpeed) VerticalInput = 0;
        if (VerticalInput < 0 && yMag < -maxSpeed) VerticalInput = 0;
        if (grounded)
        {
            if (!crouching)
            {
                rb.AddForce(orientation.transform.forward * VerticalInput * moveSpeed * Time.deltaTime);
                rb.AddForce(orientation.transform.right * HorizontalInput * moveSpeed * Time.deltaTime);
            }

        }
        else if (State == MovementState.falling)
        {
            rb.AddForce(orientation.transform.forward * VerticalInput * aircontrol * moveSpeed * Time.deltaTime / 10);
            rb.AddForce(orientation.transform.right * HorizontalInput * aircontrol * moveSpeed * Time.deltaTime / 10);
        }
        else if (State == MovementState.Swinging)
        {
            rb.AddForce(orientation.transform.forward * VerticalInput * moveSpeed * swingControl * extraMomentum * Time.deltaTime / 100);
            rb.AddForce(orientation.transform.right * HorizontalInput * moveSpeed * swingControl * Time.deltaTime / 100);
        }

    }

    private void ApplyGravity()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * gravityMultiplier * 10);
    }


    private void Jump()
    {
        State = MovementState.Jumping;
        if (!grounded || !readyToJump) return;


        readyToJump = false;

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

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.linearVelocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.linearVelocity.x, 2) + Mathf.Pow(rb.linearVelocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.linearVelocity.y;
            Vector3 n = rb.linearVelocity.normalized * maxSpeed;
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
    }
}
