using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class WallRun : MonoBehaviour
{
    [SerializeField] float wallRunForce = 10f;
    [SerializeField] float wallcheckDistance = 1f;
    [SerializeField] float wallJumpForce = 10f;
    [SerializeField] float wallruntime = 0.9f;
    [SerializeField] float wallruncooldown = 0.5f;
    private Vector3 wallRunDirection;
    public GameObject playerObject;
    public Transform orientation;
    private GameObject wall;
    [SerializeField] LayerMask wallLayer = 8;

    void Update()
    {
        wallRun();
    }

    private void wallRun()
    {
        bool isWallRight = Physics.Raycast(playerObject.transform.position, playerObject.transform.right, out RaycastHit hitRight, wallcheckDistance, wallLayer);
        bool isWallLeft = Physics.Raycast(playerObject.transform.position, -playerObject.transform.right, out RaycastHit hitLeft, wallcheckDistance, wallLayer);
        Vector3 directionToWall = isWallRight ? hitRight.normal : hitLeft.normal;
        if (!isWallRight && !isWallLeft)
        {
            if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.WallRunning)
            {
                playerObject.GetComponent<Rigidbody>().useGravity = true;
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.falling;
            }
            return;
        }
        if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.WallRunning)
        {
            if (wallruntime > 0)
            {
                wallruntime -= 0.01f;
            }
            else
            {
                playerObject.GetComponent<Rigidbody>().useGravity = true;
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.falling;
                StartCoroutine(WallRunCooldown());
            }


            float distanceToWall = isWallRight ? Vector3.Distance(playerObject.transform.position, hitRight.point) : Vector3.Distance(playerObject.transform.position, hitLeft.point);
            wallRunDirection = Vector3.ProjectOnPlane(new Vector3(playerObject.GetComponent<Rigidbody>().linearVelocity.x, 0, playerObject.GetComponent<Rigidbody>().linearVelocity.z), -directionToWall);
            if (playerObject.GetComponent<Rigidbody>().linearVelocity.magnitude < playerObject.GetComponent<PlayerMovement>().maxSpeed)
            {
                playerObject.GetComponent<Rigidbody>().AddForce(wallRunDirection * wallRunForce * wallruntime);
            }
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


            if (Vector3.Angle(orientation.forward, -directionToWall) > 50 && Vector3.Angle(orientation.forward, -directionToWall) < 85)
            {
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.WallRunning;
                playerObject.GetComponent<Rigidbody>().useGravity = false;
            }
            if (Vector3.Angle(-orientation.forward, -directionToWall) > 50 && Vector3.Angle(-orientation.forward, -directionToWall) < 85)
            {
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.WallRunning;
                playerObject.GetComponent<Rigidbody>().useGravity = false;
            }



        }

    }
    IEnumerator WallRunCooldown()
    {
        yield return new WaitForSeconds(wallruncooldown);
        wallruntime = 0.9f;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerObject.transform.position, playerObject.transform.right * wallcheckDistance);
        Gizmos.DrawRay(playerObject.transform.position, -playerObject.transform.right * wallcheckDistance);
    }
}
