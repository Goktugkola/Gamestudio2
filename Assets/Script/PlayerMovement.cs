using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AdvancedFPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    private float mouseSensitivity;
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float groundAcceleration = 15f;
    [SerializeField] private float airAcceleration = 5f;
    [SerializeField] private float friction = 8f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 24f;
    [SerializeField] private float airControl = 0.3f;
    [SerializeField] private float maxAirSpeedMultiplier = 1.2f;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerCamera;
    
    private Rigidbody rb;
    private float xRotation;
    private float yRotation;
    private bool isGrounded;
    private Vector3 moveDirection;

    private void Awake()
    {
        mouseSensitivity = gameObject.GetComponent<CameraController>().mouseSensitivity;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleJump();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyGravity();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation += mouseX, 0f);
    }

    private void HandleMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 wishDir = orientation.forward * input.y + orientation.right * input.x;
        wishDir = wishDir.normalized;

        if (isGrounded)
        {
            GroundMove(wishDir);
            ApplyFriction();
        }
        else
        {
            AirMove(wishDir);
        }
    }

    private void GroundMove(Vector3 wishDir)
    {
        float currentSpeed = Vector3.Dot(rb.linearVelocity, wishDir);
        float addSpeed = Mathf.Clamp(walkSpeed - currentSpeed, 0, walkSpeed);
        
        rb.AddForce(wishDir * (groundAcceleration * addSpeed * Time.fixedDeltaTime), ForceMode.Acceleration);
    }

    private void AirMove(Vector3 wishDir)
    {
        // Air strafing - preserve momentum while allowing directional control
        float wishSpeed = airAcceleration * Time.fixedDeltaTime;
        float currentSpeed = Vector3.Dot(rb.linearVelocity, wishDir);
        float addSpeed = Mathf.Clamp(walkSpeed * maxAirSpeedMultiplier - currentSpeed, 0, wishSpeed);

        if (addSpeed > 0)
        {
            Vector3 accelDir = Vector3.Lerp(rb.linearVelocity.normalized, wishDir, airControl).normalized;
            rb.AddForce(accelDir * (airAcceleration * addSpeed * Time.fixedDeltaTime), ForceMode.Acceleration);
        }
    }

    private void ApplyFriction()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float speed = flatVelocity.magnitude;

        if (speed < 0.1f) return;

        float drop = speed * friction * Time.fixedDeltaTime;
        rb.linearVelocity *= Mathf.Max(speed - drop, 0) / speed;
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravity * rb.mass, ForceMode.Force);
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
}