using Unity.VisualScripting;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] float wallRunForce = 10f;
    [SerializeField] float wallcheckDistance = 1f;
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

        if (!isWallRight && !isWallLeft)
        {
            if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.WallRunning)
            {
                playerObject.GetComponent<Rigidbody>().useGravity = true;
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.falling;
            }
            return;
        }
        if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.WallRunning && playerObject.GetComponent<Rigidbody>().linearVelocity.magnitude < playerObject.GetComponent<PlayerMovement>().maxSpeed)
        {
            wallRunDirection = Vector3.ProjectOnPlane(playerObject.GetComponent<Rigidbody>().linearVelocity, wall.transform.forward);
            print(wallRunDirection);
            playerObject.GetComponent<Rigidbody>().AddForce(wallRunDirection.normalized * wallRunForce);
        }
        wall = isWallRight ? hitRight.collider.gameObject : hitLeft.collider.gameObject;
        if (playerObject.GetComponent<PlayerMovement>().State == PlayerMovement.MovementState.falling)
        {
            if (Vector3.Angle(orientation.forward, -wall.transform.forward) > 50)
            {
                playerObject.GetComponent<PlayerMovement>().State = PlayerMovement.MovementState.WallRunning;
                playerObject.GetComponent<Rigidbody>().useGravity = false;


            }

            float distanceToWall = Vector3.Distance(playerObject.transform.position, wall.transform.position);
            if (distanceToWall > 0.5f)
            {
                Vector3 directionToWall = (wall.transform.position - playerObject.transform.position).normalized;
                playerObject.GetComponent<Rigidbody>().AddForce(directionToWall * wallRunForce);
            }

        }

    }

}
