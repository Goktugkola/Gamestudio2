using System.Collections;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] float wallRunForce = 10f;
    [SerializeField] float wallcheckDistance = 1f;
    [SerializeField] float wallJumpForce = 10f;
    [SerializeField] float wallruntime = 0.9f;
    private float defaultwallruntime;
    private Vector3 wallRunDirection;
    public GameObject playerObject;
    public Transform orientation;
    private GameObject wall;
    private bool isWallRight;
    private bool isWallLeft;
    [SerializeField] LayerMask wallLayer = 8;

    private void Start()
    {
        defaultwallruntime = wallruntime;
    }
    void Update()
    {
        WallRunFunc();
    }

    private void WallRunFunc()
    {
        isWallRight = Physics.Raycast(playerObject.transform.position, orientation.right, out RaycastHit hitRight, wallcheckDistance, wallLayer);
        isWallLeft = Physics.Raycast(playerObject.transform.position, -orientation.right, out RaycastHit hitLeft, wallcheckDistance, wallLayer);
        Vector3 directionToWall = isWallRight ? hitRight.normal : hitLeft.normal;
        if (!isWallRight && !isWallLeft)
        {

            if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.WallRunning)
            {
                playerObject.GetComponent<Rigidbody>().useGravity = true;
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.falling;
            }
            if (wallruntime <= 0)
            {
                wallruntimeReset();
            }
            return;
        }
        if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.WallRunning)
        {
            if (wallruntime > 0)
            {
                wallruntime -= 0.01f;
            }
            if (wallruntime <= 0)
            {
                playerObject.GetComponent<Rigidbody>().useGravity = true;
            }
            if (playerObject.GetComponent<Rigidbody>().linearVelocity.magnitude <= 5f)
            {
                playerObject.GetComponent<Rigidbody>().useGravity = true;
            }
            float distanceToWall = isWallRight ? Vector3.Distance(playerObject.transform.position, hitRight.point) : Vector3.Distance(playerObject.transform.position, hitLeft.point);
            wallRunDirection = Vector3.ProjectOnPlane(playerObject.GetComponent<Rigidbody>().linearVelocity, -directionToWall);
            wallRunDirection = new Vector3(wallRunDirection.x, 0, wallRunDirection.z);
            //Wallrun
            playerObject.GetComponent<Rigidbody>().AddForce(wallRunDirection * wallRunForce * wallruntime / 6);
            if(playerObject.GetComponent<Rigidbody>().linearVelocity.y >3f)
            {
            playerObject.GetComponent<Rigidbody>().linearVelocity = new Vector3(playerObject.GetComponent<Rigidbody>().linearVelocity.x, 0, playerObject.GetComponent<Rigidbody>().linearVelocity.z);
            }
            //Walljump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerObject.GetComponent<Rigidbody>().AddForce(directionToWall * wallJumpForce * 100);
                playerObject.GetComponent<Rigidbody>().useGravity = true;
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.falling;
            }
            else if (distanceToWall > 0.5f)
            {
                playerObject.GetComponent<Rigidbody>().AddForce(-directionToWall * wallRunForce);
            }
        }
        wall = isWallRight ? hitRight.collider.gameObject : hitLeft.collider.gameObject;
        if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.falling)
        {


            if (Vector3.Angle(orientation.forward, -directionToWall) > 50 && Vector3.Angle(orientation.forward, -directionToWall) < 85 && wallruntime > 0)
            {
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.WallRunning;
                playerObject.GetComponent<Rigidbody>().useGravity = false;
            }
            if (Vector3.Angle(-orientation.forward, -directionToWall) > 50 && Vector3.Angle(-orientation.forward, -directionToWall) < 85 && wallruntime > 0)
            {
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.WallRunning;
                playerObject.GetComponent<Rigidbody>().useGravity = false;
            }



        }

    }
    public void wallruntimeReset()
    {
        wallruntime = defaultwallruntime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerObject.transform.position, orientation.right * wallcheckDistance);
        Gizmos.DrawRay(playerObject.transform.position, -orientation.right * wallcheckDistance);
    }
}
