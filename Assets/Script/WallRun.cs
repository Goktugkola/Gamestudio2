using System.Collections;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] float wallRunForce = 10f;
    [SerializeField] float wallcheckDistance = 1f;
    [SerializeField] float wallJumpForce = 10f;
    [SerializeField] private float coyoteTimeDuration = 0.2f; // Duration of coyote time
    private float coyoteTimeTimer = 0f; // Tracks remaining coyote time
    private Vector3 lastWallNormal; // Stores the last wall's normal for jumping
    private bool canUseCoyoteTime = false; // Indicates if coyote time is active
    float wallruntimer = 0.9f;
    private float defaultwallruntime;
    private Vector3 wallRunDirection;
    public GameObject playerObject;
    public Transform orientation;
    private GameObject wall;
    public bool isWallRight;
    public bool isWallLeft;
    
    [SerializeField] LayerMask wallLayer = 8;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private void Awake()
    {
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        rb = playerObject.GetComponent<Rigidbody>();
        if (playerObject == null)
        {
            Debug.LogError("Player object is not assigned in the inspector.");
        }
        if (orientation == null)
        {
            Debug.LogError("Orientation is not assigned in the inspector.");
        }
    }

    private void Start()
    {
        defaultwallruntime = wallruntimer;
    }
    void Update()
    {
        WallRunFunc();
    }

    private void WallRunFunc()
    {
        isWallRight = Physics.Raycast(playerObject.transform.position, orientation.right, out RaycastHit hitRight, wallcheckDistance, wallLayer);
        isWallLeft = Physics.Raycast(playerObject.transform.position, -orientation.right, out RaycastHit hitLeft, wallcheckDistance, wallLayer);
        Vector3 directionToWall = isWallRight ? hitRight.normal : isWallLeft ? hitLeft.normal : Vector3.zero;

        if (!isWallRight && !isWallLeft)
        {
            if (playerMovement.State == PlayerMovement.MovementState.WallRunning)
            {
                rb.useGravity = true;
                playerMovement.State = PlayerMovement.MovementState.Falling;
                canUseCoyoteTime = true; // Enable coyote time
                coyoteTimeTimer = coyoteTimeDuration; // Reset coyote time timer
            }
            wallruntimeReset();
        }

        if (playerMovement.State == PlayerMovement.MovementState.WallRunning)
        {
            if (wallruntimer > 0)
            {
                wallruntimer -= 0.005f;
            }
            if (wallruntimer <= 0.01f)
            {
                rb.useGravity = true;
                rb.AddForce(Vector3.down * 2, ForceMode.Acceleration);
            }
            if (rb.linearVelocity.magnitude <= 5f)
            {
                rb.useGravity = true;
            }
            if (wallruntimer > 0.5f)
            {
                if(rb.linearVelocity.y < 0)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                }
            }
            float distanceToWall = isWallRight ? Vector3.Distance(playerObject.transform.position, hitRight.point) : Vector3.Distance(playerObject.transform.position, hitLeft.point);
            wallRunDirection = Vector3.ProjectOnPlane(rb.linearVelocity, -directionToWall).normalized * rb.linearVelocity.magnitude;
            wallRunDirection = new Vector3(wallRunDirection.x, 0, wallRunDirection.z);
            
            //Wallrun
            wallruntimer = Mathf.Clamp(wallruntimer, 0.001f, defaultwallruntime);
            rb.AddForce(wallRunDirection * wallRunForce * wallruntimer / 6, ForceMode.Force);

            //Walljump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(directionToWall * wallJumpForce * 100);
                rb.AddForce(Vector3.up * wallJumpForce * 50);
                rb.useGravity = true;
                playerMovement.State = PlayerMovement.MovementState.Falling;
                canUseCoyoteTime = false; // Disable coyote time after jumping
            }
            else if (distanceToWall > 0.5f)
            {
                rb.AddForce(-directionToWall * wallRunForce);
            }
        }
        else if (canUseCoyoteTime && coyoteTimeTimer > 0)
        {
            print("Coyote time active: " + coyoteTimeTimer);
            // Allow wall jump during coyote time
            coyoteTimeTimer -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("Coyote jump!");
                rb.AddForce(lastWallNormal * wallJumpForce * 100);
                rb.AddForce(Vector3.up * wallJumpForce * 50);
                rb.useGravity = true;
                playerMovement.State = PlayerMovement.MovementState.Falling;
                canUseCoyoteTime = false; // Disable coyote time after jumping
            }
        }

        if (isWallLeft || isWallRight)
        {
            wall = isWallRight ? hitRight.collider.gameObject : hitLeft.collider.gameObject;
            lastWallNormal = directionToWall; // Store the last wall's normal
        }

        if (playerMovement.State == PlayerMovement.MovementState.Falling)
        {
            if (Vector3.Angle(orientation.forward, -directionToWall) > 50 && Vector3.Angle(orientation.forward, -directionToWall) < 95 && wallruntimer > 0)
            {
                playerMovement.State = PlayerMovement.MovementState.WallRunning;
                rb.useGravity = false;
            }
            if (Vector3.Angle(-orientation.forward, -directionToWall) > 50 && Vector3.Angle(-orientation.forward, -directionToWall) < 95 && wallruntimer > 0)
            {
                playerMovement.State = PlayerMovement.MovementState.WallRunning;
                rb.useGravity = false;
            }
        }
    }
    public void wallruntimeReset()
    {
        wallruntimer = defaultwallruntime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, orientation.right * wallcheckDistance);
        Gizmos.DrawRay(transform.position, -orientation.right * wallcheckDistance);
    }
}
