using UnityEngine;

public class Swing : MonoBehaviour
{
    [Header("Assignables")]
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform player;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LineRenderer swingLine;
    private Rigidbody rb;
    [Header("Swinging Settings")]
    [SerializeField] private float maxSwingDistance = 25f;
    [SerializeField] private float minSwingDistance = 3f;  // Minimum distance to consider
    [SerializeField] private float sphereCastRadius = 1.5f; // Radius for sphere casting
    [SerializeField] private float smallsphereCastRadius = 0.5f; // Radius for sphere casting
    [SerializeField] private float springForce = 4.5f;
    [SerializeField] private float damperForce = 7f;
    [SerializeField] private float horizontalThrustForce = 200f;
    [SerializeField] private float forwardThrustForce = 200f;
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;

    [SerializeField] private LayerMask swingMask;
    [SerializeField] private float extendCableSpeed = 0.5f;
    [SerializeField] private float grappleMultiplier = 20f;
    private bool isSwinging;
    private SpringJoint swingJoint;
    private Vector3 swingPoint;
    public int currentWeaponid = 0;
    private void Awake()
    {
        swingLine.positionCount = 0;
        rb = player.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        HandleLength();
    }
    private void FixedUpdate()
    {

    }
    private void LateUpdate()
    {
        HandleSwingInput();
        if (swingJoint != null)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                DrawSwingRope();

            }
        }
    }

    private void HandleSwingInput()
    {
        if (Input.GetKeyDown(fireKey))
        {
            StartSwing();
            player.gameObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.Swinging;
            OdmGearMovement();
        }
        if (Input.GetKeyUp(fireKey))
        {
            StopSwing();
            player.gameObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.falling;
        }
        /*/if (Input.GetMouseButtonDown(1))
        {
            StartSwing();
            player.gameObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.Grappling;
        }
        if (Input.GetMouseButtonUp(1))
        {
            StopSwing();
            player.gameObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.falling;
        }/*/
    }
    private void StartSwing()
    {
        RaycastHit hit;
        bool validSwingPointFound = false;
        Vector3 cameraPos = playerCam.position;
        Vector3 cameraForward = playerCam.forward;

        // First try raycast
        if (Physics.SphereCast(cameraPos, smallsphereCastRadius, cameraForward, out hit, maxSwingDistance, swingMask))
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
            swingJoint = player.gameObject.AddComponent<SpringJoint>();

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
    public void HandleLength()
    {
        if (isSwinging == true && swingJoint != null)
        {
            Vector3 directionToPoint = swingPoint - player.transform.position;
            if (Vector3.Dot(rb.linearVelocity.normalized, directionToPoint.normalized) > 0.7f || player.position.y > swingJoint.transform.position.y) // Adjust the threshold as needed
            {
                print("Swing direction and player's velocity direction are the same");
                swingJoint.damper = 0f;

            }
            else if (player.gameObject.GetComponent<PlayerMovement>().State != PlayerMovement.MovementState.Grappling)
            {
                print("Swing direction and player's velocity direction are not the same");
                swingJoint.damper = damperForce;
            }
            if (player.gameObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.Grappling)
            {
                print("Grapple mode");
                swingJoint.damper = 0;
                swingJoint.spring = springForce * grappleMultiplier;
                swingJoint.minDistance = 0;
            }
        }
    }
    private void OdmGearMovement()
    {
        // right
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        // left
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        // forward
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            swingJoint.maxDistance = distanceFromPoint * 0.8f;
            swingJoint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            swingJoint.maxDistance = extendedDistanceFromPoint * 0.8f;
            swingJoint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }
    private void StopSwing()
    {
        isSwinging = false;
        player.gameObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.Swinging;

        swingLine.positionCount = 0;
        Destroy(swingJoint);
    }

    private void DrawSwingRope()
    {
        if (!isSwinging) return;

        swingLine.SetPosition(0, gunTip.position);
        swingLine.SetPosition(1, swingPoint);
    }
}
