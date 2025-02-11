using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 8f;
    
    private Rigidbody rb;
    private float xRotation;
    private float yRotation;
    private Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        
        // Initialize rotations
        xRotation = transform.eulerAngles.x;
        yRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovementInput();
        HandleJump();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical camera rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply camera rotation
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Horizontal player rotation
        yRotation += mouseX;
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        moveDirection = orientation.forward * vertical + orientation.right * horizontal;
        moveDirection = moveDirection.normalized;
    }

    private void MovePlayer()
    {
        if (moveDirection != Vector3.zero)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(orientation.position, orientation.forward * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(orientation.position, orientation.right * 2);
    }
}