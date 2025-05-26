using UnityEngine;

public class LedgeJump : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float ledgeDistance = 1.5f;
    [SerializeField] private Collider ledgeCollider;
    [SerializeField] private float ledgeJumpCooldownDuration = 1.0f;
    [SerializeField] private float ledgeJumpDuration = 0.5f;
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Transform playerOrientation;
    private bool isLedgeJumpOnCooldown = false;

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

        // Prevent new ledge jump attempts if on cooldown
        if (isLedgeJumpOnCooldown) return;

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
    private void OnTriggerExit(Collider other)
    {
        playerMovement.State = PlayerMovement.MovementState.Falling;
        playerRigidbody.useGravity = true;
        StopAllCoroutines();
        // Ensure cooldown is reset if player exits the trigger area
        isLedgeJumpOnCooldown = false; 

    }
    private System.Collections.IEnumerator PerformJump(Collider other, Vector3 direction, float mag)
    {
        // Set cooldown flag and start the cooldown timer
        isLedgeJumpOnCooldown = true;
        StartCoroutine(ResetLedgeJumpCooldown());

        // Apply initial jump forces
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerRigidbody.AddForce(direction * mag, ForceMode.Impulse);
        playerRigidbody.useGravity = false;
        
        // Wait until player clears the ledge
        yield return new WaitForSecondsRealtime(ledgeJumpDuration);
        // Reset player state and gravity
        playerRigidbody.useGravity = true;
        playerMovement.State = PlayerMovement.MovementState.Falling;
    }

    private System.Collections.IEnumerator ResetLedgeJumpCooldown()
    {
        yield return new WaitForSeconds(ledgeJumpCooldownDuration);
        isLedgeJumpOnCooldown = false;
    }


}
