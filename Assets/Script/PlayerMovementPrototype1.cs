using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CompletePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float groundAcceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float friction;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private float airControl;

    [Header("Swinging Settings")]
    [SerializeField] private float maxSwingDistance = 25f;
    [SerializeField] private float minSwingDistance = 3f;  // Minimum distance to consider
    [SerializeField] private float sphereCastRadius = 1.5f; // Radius for sphere casting
    [SerializeField] private float springForce = 4.5f;
    [SerializeField] private float damperForce = 7f;
    [SerializeField] private float horizontalThrustForce = 200f;
    [SerializeField] private float forwardThrustForce = 200f;
    [SerializeField] private LayerMask swingMask;


    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LineRenderer swingLine;
    private float mouseSensitivity;

    private Rigidbody rb;
    private SpringJoint swingJoint;
    private Vector3 swingPoint;
    private bool isGrounded;
    private bool isSwinging;
    private Vector3 moveDirection;
    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        swingLine.positionCount = 0;
        mouseSensitivity = gameObject.GetComponentInChildren<CameraController>().mouseSensitivity;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleJump();
        GroundCheck();
        HandleSwingInput();

    }

    private void FixedUpdate()
    {
        if (isSwinging)
        {
            ApplySwingForces();
        }
        else
        {
            HandleMovement();
            ApplyGravity();
        }
    }

    private void LateUpdate()
    {
        print(isGrounded);
        DrawSwingRope();
    }

    #region Core Movement
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 100f;
        orientation.rotation = Quaternion.Euler(0f, yRotation += mouseX * mouseSensitivity, 0f);
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
    #endregion

    #region Swing Mechanics
    private void HandleSwingInput()
    {
        if (Input.GetMouseButtonDown(0)) StartSwing();
        if (Input.GetMouseButtonUp(0)) StopSwing();
    }

    private void StartSwing()
    {
        RaycastHit hit;
        bool validSwingPointFound = false;
        Vector3 cameraPos = playerCamera.position;
        Vector3 cameraForward = playerCamera.forward;

        // First try raycast
        if (Physics.Raycast(cameraPos, cameraForward, out hit, maxSwingDistance, swingMask))
        {
            // Check if the hit point is beyond minimum distance
            if (Vector3.Distance(transform.position, hit.point) > minSwingDistance)
            {
                validSwingPointFound = true;
            }
        }

        // If raycast failed, try spherecast
        if (!validSwingPointFound)
        {
            if (Physics.SphereCast(cameraPos, sphereCastRadius, cameraForward, 
                out hit, maxSwingDistance, swingMask))
            {
                // Verify spherecast hit distance
                if (Vector3.Distance(transform.position, hit.point) > minSwingDistance)
                {
                    validSwingPointFound = true;
                }
            }
        }

        if (validSwingPointFound)
        {
            isSwinging = true;
            swingPoint = hit.point;
            swingJoint = gameObject.AddComponent<SpringJoint>();
            
            swingJoint.autoConfigureConnectedAnchor = false;
            swingJoint.connectedAnchor = swingPoint;
            
            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);
            swingJoint.maxDistance = distanceFromPoint * 0.8f;
            swingJoint.minDistance = distanceFromPoint * 0.25f;
            
            swingJoint.spring = springForce;
            swingJoint.damper = damperForce;
            swingJoint.massScale = 4.5f;

            swingLine.positionCount = 2;
        }
    }


    private void StopSwing()
    {
        isSwinging = false;
        swingLine.positionCount = 0;
        Destroy(swingJoint);
    }

    private void ApplySwingForces()
    {
        // Horizontal movement
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        // Forward movement
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);

        // Shorten cable
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(-directionToPoint.normalized * forwardThrustForce * Time.deltaTime);
            
            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);
            swingJoint.maxDistance = distanceFromPoint * 0.8f;
            swingJoint.minDistance = distanceFromPoint * 0.25f;
        }
    }

    private void DrawSwingRope()
    {
        if (!isSwinging) return;

        swingLine.SetPosition(0, gunTip.position);
        swingLine.SetPosition(1, swingPoint);
    }
    #endregion

    #region Movement Helpers
    private void GroundMove(Vector3 wishDir)
    {
        float currentSpeed = Vector3.Dot(rb.linearVelocity, wishDir);
        float addSpeed = Mathf.Clamp(walkSpeed - currentSpeed, 0, walkSpeed);
        rb.AddForce(wishDir * (groundAcceleration * addSpeed * Time.fixedDeltaTime), ForceMode.Acceleration);
    }

    private void AirMove(Vector3 wishDir)
    {
        float wishSpeed = airAcceleration * Time.fixedDeltaTime;
        float currentSpeed = Vector3.Dot(rb.linearVelocity, wishDir);
        float addSpeed = Mathf.Clamp(walkSpeed * 1.2f - currentSpeed, 0, wishSpeed);

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
        if (!isGrounded && !isSwinging)
        {
            rb.AddForce(Vector3.down * gravity * rb.mass, ForceMode.Force);
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.05f);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    #endregion
    private void OnDrawGizmos()
    {
        if (isSwinging)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(swingPoint, 1f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerCamera.position + playerCamera.forward * maxSwingDistance, 1f);
        }
    }
}