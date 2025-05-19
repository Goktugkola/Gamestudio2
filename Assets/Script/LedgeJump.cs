using UnityEngine;

public class LedgeJump : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float ledgeDistance = 1.5f;
    [SerializeField] private Collider ledgeCollider;
    [SerializeField] private LayerMask ledgeLayer = 6; // Use LayerMask for better readability

    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Transform playerOrientation;



    void Awake()
    {

        if (playerObject != null)
        {
            playerRigidbody = playerObject.GetComponent<Rigidbody>();
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player Object is not assigned in LedgeJump script.", this);
        }
    }

    void Update()
    {
        ledgeCollider.transform.rotation = playerOrientation.rotation;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player Rigidbody is cached
        if (playerRigidbody == null) return;

        // Use LayerMask for comparison
        if (other.GetComponent<LedgeJumpObject>() != null)
        {
            // Check input and distance condition
            if (playerMovement.State == PlayerMovement.MovementState.Falling && ledgeCollider.bounds.max.y - playerTransform.position.y < ledgeDistance)
            {

                float mag = playerRigidbody.linearVelocity.magnitude;
                Vector3 direction = playerRigidbody.linearVelocity.normalized;
                playerMovement.State = PlayerMovement.MovementState.Mantling;
                playerRigidbody.linearVelocity = Vector3.zero;

                StartCoroutine(PerformJump(other, direction, mag));
            }
        }
    }
    private System.Collections.IEnumerator PerformJump(Collider other, Vector3 direction, float mag)
    {
        
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerRigidbody.useGravity = false;
        yield return new WaitUntil(() => playerCollider.bounds.min.y > other.bounds.max.y);
        playerRigidbody.AddForce(direction * mag, ForceMode.Impulse);
        playerRigidbody.useGravity = true;
        playerMovement.State = PlayerMovement.MovementState.Falling;
    }


}
