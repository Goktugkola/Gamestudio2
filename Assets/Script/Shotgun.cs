using System.Collections;
using UnityEngine; // Removed NUnit.Framework as it's likely not needed here

public class Shotgun : MonoBehaviour
{
    [Header("Shooting Mechanics")]
    [SerializeField] private float knockbackForce = 20f; // Renamed for convention
    [SerializeField] private float fireCooldown = 0.5f; // Time between shots
    [SerializeField] private int maxBulletCount = 5;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask targetLayers; // Layers considered as valid targets
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse1;

    [Header("Challenge Settings")]
    [SerializeField] private float challengeTimeLimit = 10.0f; // Time in seconds for the challenge
    [SerializeField] private int targetCountGoal = 5; // How many targets to hit for success

    [Header("References")]
    [SerializeField] private GameObject playerObject; // Assign the Player GameObject
    [SerializeField] private Camera playerCamera; // Assign the main Camera used for aiming

    // Internal State
    private int currentBulletCount;
    private float currentCooldown;
    private int shotTargetCount; // Renamed for convention
    private float currentChallengeTime;
    private bool isChallengeActive = false;
    private bool challengeCompleted = false; // Flag to prevent repeated success/fail messages

    // Cached Components
    private PlayerMovement playerMovement;
    private Rigidbody playerRb;

    void Awake()
    {
        // Cache components for performance
        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<PlayerMovement>();
            playerRb = playerObject.GetComponent<Rigidbody>(); // Assuming Rigidbody is on the main player object
        }
        else
        {
            Debug.LogError("Player Object not assigned in the inspector!", this);
        }

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not assigned in the inspector!", this);
            // Attempt to find the main camera as a fallback
            playerCamera = Camera.main;
            if (playerCamera == null)
                Debug.LogError("Could not find Main Camera!", this);
        }
    }

    void Start()
    {
        currentBulletCount = maxBulletCount;
        currentCooldown = 0f;
        // Optionally start the challenge immediately, or trigger it via another event/method
        // StartChallenge();
    }

    public void Update()
    {
        // Cooldown timer
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        HandleInput();
        HandleGroundReload();
        HandleChallengeTimer();
    }

    private void HandleInput()
    {
        // Allow shooting only if cooldown is over
        if (Input.GetKeyDown(fireKey) && currentCooldown <= 0)
        {
            Shoot();
        }
    }

    private void HandleGroundReload()
    {
        // Reload bullets when grounded (check if playerMovement is valid)
        if (playerMovement != null && playerMovement.grounded)
        {
            currentBulletCount = maxBulletCount;
        }
    }

    private void HandleChallengeTimer()
    {
        if (isChallengeActive && !challengeCompleted)
        {
            currentChallengeTime -= Time.deltaTime;

            // Check for Success Condition
            if (shotTargetCount >= targetCountGoal)
            {
                ChallengeSuccess();
            }
            // Check for Failure Condition (Timer runs out)
            else if (currentChallengeTime <= 0)
            {
                ChallengeFail();
            }
        }
    }

    // Call this method to begin the challenge
    public void StartChallenge()
    {
        Debug.Log("Challenge Started!");
        shotTargetCount = 0;
        currentChallengeTime = challengeTimeLimit;
        isChallengeActive = true;
        challengeCompleted = false;
    }

    private void ChallengeSuccess()
    {
        Debug.Log("Challenge SUCCESSFUL! Targets hit: " + shotTargetCount);
        isChallengeActive = false; // Stop the timer
        challengeCompleted = true;
        // --- ADD YOUR SUCCESS ACTIONS HERE ---
        // Example: Load next level, display UI message, grant reward, etc.
        // FindObjectOfType<GameManager>()?.ChallengeWon();
    }

    private void ChallengeFail()
    {
        Debug.Log("Challenge FAILED! Time ran out. Targets hit: " + shotTargetCount);
        isChallengeActive = false; // Stop the timer
        challengeCompleted = true;
        // --- ADD YOUR FAILURE ACTIONS HERE ---
        // Example: Restart level, display UI message, etc.
        // FindObjectOfType<GameManager>()?.ChallengeLost();
    }

    public void Shoot()
    {
        // Early exit if references are missing, out of bullets, or on cooldown
        if (playerCamera == null || playerObject == null || currentBulletCount <= 0 || currentCooldown > 0)
        {
            return;
        }

        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, sphereRadius, direction, maxDistance, targetLayers, QueryTriggerInteraction.Ignore);

        int targetsHitThisShot = 0; // Track hits in this specific shot

        for (int i = 0; i < hits.Length; i++)
        {
            // Check if the hit object's layer is within the targetLayers mask
            // This is a more robust way than comparing layer index directly
            if (hits[i].collider == null || hits[i].collider.gameObject.layer == 2) continue; // Skip if no collider
            if (hits[i].collider.gameObject.layer == targetLayers.value) // Assuming "Target" is the layer name
            {
                targetsHitThisShot++;
                if (isChallengeActive && !challengeCompleted)
                {
                    shotTargetCount++; // Only count towards challenge if active
                }

                Debug.Log("Hit Target: " + hits[i].collider.gameObject.name);
                Destroy(hits[i].collider.gameObject); // Destroy the hit target

                // Optional Debug Drawing
                Debug.DrawLine(origin, hits[i].point, Color.green, 2.0f);
                Debug.DrawRay(hits[i].point, hits[i].normal * 0.5f, Color.red, 2.0f); // Shortened normal ray
            }
        }

        // Apply effects only if at least one target was potentially hit by the spherecast
        // or even if no target was hit (depending on desired game feel)
        if (hits.Length > 0 || true) // Decide if effects apply even on miss
        {
            // Fire Anim and sound here (Placeholder)
            // AudioManager.PlaySound("ShotgunFire");
            // animator.SetTrigger("Shoot");

            // Knockback
            if (playerRb != null)
            {
                // Apply knockback opposite to the camera's forward direction
                playerRb.AddForce(-playerCamera.transform.forward * knockbackForce, ForceMode.Impulse);
                Debug.Log("Applied Knockback");
            }

            currentBulletCount--;
            currentCooldown = fireCooldown; // Start cooldown timer
        }
    }
}

