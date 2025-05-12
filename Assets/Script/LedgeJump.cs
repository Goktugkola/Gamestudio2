using UnityEngine;
using System.Collections;

public class LedgeJump : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform orientation;
    [SerializeField] private float positionMultiplierH;
    [SerializeField] private float positionMultiplierV;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float ledgeDistance = 1.5f;
    [SerializeField] private Collider ledgeCollider;
    [SerializeField] private LayerMask ledgeLayer = 6; // Use LayerMask for better readability

    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private Transform ledgeTransform;
    [SerializeField] private PlayerMovement playerMovement; // Reference to PlayerMovement script

    private bool isMantling = false;
    private bool canMantle = true;
    [SerializeField] private float mantleDuration = 1f;
    [SerializeField] private float mantleEndUpwardBoost = 2f;
    [SerializeField] private float playerOffsetFromWall = 0.5f;
    [SerializeField] private float playerHeightAboveLedge = 1f;
    [SerializeField] private CapsuleCollider playerCollider;
    private Vector3 detectedLedgePoint;
    private Vector3 detectedWallNormal;

    void Awake() // Use Awake for initialization before Start
    {
        // Cache components and transforms
        if (playerObject != null)
        {
            playerRigidbody = playerObject.GetComponent<Rigidbody>();
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player Object is not assigned in LedgeJump script.", this);
        }

        if (ledgeCollider != null)
        {
            ledgeTransform = ledgeCollider.transform; // Cache ledge transform
        }
        else
        {
            Debug.LogError("Ledge Collider is not assigned in LedgeJump script.", this);
        }
    }

    // Consider using OnTriggerStay if you want the jump to be possible
    // anytime the player is in the trigger and presses Space.
    // OnTriggerEnter only fires once upon entry.
    void OnTriggerEnter(Collider other)
    {
        // Check if the player Rigidbody is cached
        if (playerRigidbody == null) return;

        // Use LayerMask for comparison
        if (((1 << other.gameObject.layer) & ledgeLayer) != 0)
        {
            // Check input and distance condition
            if (Input.GetKey(KeyCode.Space) && ledgeCollider.bounds.max.y - playerTransform.position.y < ledgeDistance)
            {
                // Apply jump force using cached Rigidbody
                // Changed state to Jumping if this is not initiating a full mantle.
                // If a full mantle (PerformMantleCoroutine) is intended here, 
                // then that coroutine should be started, and it handles the Mantling state.
                playerMovement.State = PlayerMovement.MovementState.Jumping; 
                playerRigidbody.linearVelocity = Vector3.zero; // Reset velocity before applying force
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void Update()
    {
        // Check if transforms are cached
        if (playerTransform == null || ledgeTransform == null || orientation == null) return;

        // Calculate the forward direction in the player's horizontal plane (based on camera/orientation yaw)
        Vector3 forwardDirection = orientation.forward;
        forwardDirection.y = 0; // Ignore camera pitch for XZ displacement

        // Normalize to ensure the distance is purely controlled by positionMultiplierV,
        // especially if orientation.forward initially had a Y component, making its XZ magnitude < 1.
        // Also, handle the case where the player is looking straight up or down.
        if (forwardDirection.sqrMagnitude > 0.001f)
        {
            forwardDirection.Normalize();
        }
        else
        {
            // If looking straight up/down, forwardDirection is (0,0,0).
            // In this case, the ledge detector will be at the player's XZ, at the calculated Y.
            // No further action needed for forwardDirection here.
        }

        // Calculate the target XZ position part
        Vector3 targetPosition = playerTransform.position + forwardDirection * positionMultiplierV;

        // Calculate the target Y position, independent of camera pitch/roll.
        // Assumes positionMultiplierH is intended as a downward offset from the player's current Y position.
        float targetY = playerTransform.position.y - positionMultiplierH;

        // Set the ledgeTransform's final position
        ledgeTransform.position = new Vector3(targetPosition.x, targetY, targetPosition.z);
    }

    IEnumerator PerformMantleCoroutine()
    {
        if (!canMantle || playerRigidbody == null || playerTransform == null || playerMovement == null) yield break;

        isMantling = true;
        canMantle = false; // Consume mantle opportunity

        // --- Set Player State to Mantling ---
        playerMovement.State = PlayerMovement.MovementState.Mantling;

        playerRigidbody.isKinematic = true; 
        playerRigidbody.linearVelocity = Vector3.zero; 

        Vector3 startPosition = playerTransform.position;
        float playerPivotToFeet = playerCollider != null ? playerCollider.height / 2f - playerCollider.center.y : playerHeightAboveLedge / 2f;
        Vector3 mantleTargetPosition = detectedLedgePoint +
                                       (Vector3.up * playerPivotToFeet) + 
                                       (detectedWallNormal * playerOffsetFromWall);

        Vector3 controlPoint = startPosition + (mantleTargetPosition - startPosition) / 2 + Vector3.up * 0.5f; 

        float elapsedTime = 0f;
        while (elapsedTime < mantleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / mantleDuration);
            t = t * t * (3f - 2f * t); // SmoothStep interpolation
            
            playerTransform.position = Vector3.Lerp(startPosition, mantleTargetPosition, t);
            
            yield return null;
        }

        playerTransform.position = mantleTargetPosition; 
        playerRigidbody.isKinematic = false; 

        playerRigidbody.AddForce(Vector3.up * mantleEndUpwardBoost, ForceMode.VelocityChange);
        playerRigidbody.AddForce(playerRigidbody.transform.forward * 2f, ForceMode.VelocityChange);
        // --- Reset Player State ---
        // Set state to Falling after mantle is complete
        playerMovement.State = PlayerMovement.MovementState.Falling; 

        isMantling = false;

        yield return new WaitForSeconds(0.1f);
    }
}
