using Unity.Mathematics;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    public float rayDistance = 100f;
    public LayerMask WallLayer; // Optional: to detect only specific walls
    public GameObject OriantationObj;

    void Update()
    {
        // Check to the right
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitRight, rayDistance, WallLayer))
        {
            Debug.Log("Wall on RIGHT");
        }
        // Check to the left
        else if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitLeft, rayDistance, WallLayer))
        {
            Debug.Log("Wall on LEFT");
        }
    }

    // Debug: visualize the rays in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * rayDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, -transform.right * rayDistance);
    }
}
