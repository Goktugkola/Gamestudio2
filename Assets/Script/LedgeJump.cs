using UnityEngine;

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
                playerRigidbody.linearVelocity = Vector3.zero; // Reset velocity before applying force
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void Update()
    {
        // Check if transforms are cached
        if (playerTransform == null || ledgeTransform == null || orientation == null) return;

        // Update ledge collider position using cached transforms
        ledgeTransform.position = playerTransform.position + orientation.forward * positionMultiplierV - orientation.up * positionMultiplierH; // Corrected vector math
    }
}
