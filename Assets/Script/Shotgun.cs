using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [Header("Shooting Mechanics")]
    [SerializeField] private float knockbackForce = 20f;
    [SerializeField] private float fireCooldown = 0.5f;
    public int ammoCurrent => currentBulletCount;
    public int ammoMax => maxBulletCount;
    [SerializeField] private int maxBulletCount = 5;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask targetLayers; // Layers considered as valid targets
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse1;

    [Header("Challenge Settings")]
    [SerializeField] private float challengeTimeLimit = 10.0f;
    [SerializeField] private int targetCountGoal = 5;

    [Header("References")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera playerCamera;

    [SerializeField] bool dashmode = false; // Optional: Use this to toggle dash mode

    // Internal State
    [SerializeField] KeyCode DashKey = KeyCode.LeftShift;
    private int currentBulletCount;
    private float currentCooldown;
    private int shotTargetCount;
    private float currentChallengeTime;
    private bool isChallengeActive = false;
    private bool challengeCompleted = false;
    public bool isDashing;
    public bool isShooting;

    // Cached Components
    private PlayerMovement playerMovement;
    private Rigidbody playerRb;
    public List<GameObject> targetsHit;

    // Optimization: Pre-allocate array for NonAlloc physics query
    private RaycastHit[] sphereCastHits = new RaycastHit[10]; // Adjust size as needed

    void Awake()
    {
        // Cache components (already good)
        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<PlayerMovement>();
            playerRb = playerObject.GetComponent<Rigidbody>();
        }
        else Debug.LogError("Player Object not assigned!", this);

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not assigned!", this);
            playerCamera = Camera.main;
            if (playerCamera == null) Debug.LogError("Could not find Main Camera!", this);
        }
    }

    void Start()
    {
        currentBulletCount = maxBulletCount;
        currentCooldown = 0f;
        // StartChallenge(); // Optional
    }

    public void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
        HandleTargets();
        HandleInput();
        HandleGroundReload();
        HandleChallengeTimer();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(DashKey) && currentBulletCount == maxBulletCount)
        {
            Dash();
        }
        if (Input.GetKeyDown(fireKey) && currentCooldown <= 0)
        {
            Shoot();
        }
    }

    private void HandleGroundReload()
    {
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

            if (shotTargetCount >= targetCountGoal) ChallengeSuccess();
            else if (currentChallengeTime <= 0) ChallengeFail();
        }
    }

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
        isChallengeActive = false;
        challengeCompleted = true;
        // --- ADD YOUR SUCCESS ACTIONS HERE ---
    }

    private void ChallengeFail()
    {
        Debug.Log("Challenge FAILED! Time ran out. Targets hit: " + shotTargetCount);
        isChallengeActive = false;
        challengeCompleted = true;
        // --- ADD YOUR FAILURE ACTIONS HERE ---
    }
    private void HandleTargets()
    {
        if (playerMovement != null && playerMovement.State == PlayerMovement.MovementState.Running)
        {
            for (int i = 0; i < targetsHit.Count; i++)
            {
                GameObject target = targetsHit[i];
                if (target != null && target.GetComponent<Collider>() != null)
                {
                    target.SendMessage("StopAnimation", SendMessageOptions.DontRequireReceiver); // Call StopAnimation on the target
                    target.GetComponent<Collider>().enabled = true;
                    print("Clear Target: " + target.name);
                    //target.GetComponent<Animation>().Play("Stop");
                    targetsHit.RemoveAt(i); // Remove the target from the list 
                }
            }

            shotTargetCount = 0; // Reset the shot target count
        }
    }
    public void Dash()
    {

        if (playerMovement.State != PlayerMovement.MovementState.Running)
        {
            isDashing = true;
            float velmagnitude = playerRb.linearVelocity.magnitude;
            playerRb.linearVelocity = new Vector3(0, 0, 0);
            playerRb.AddForce(playerCamera.transform.forward * (knockbackForce + velmagnitude), ForceMode.Impulse);
            currentBulletCount--;
        }
    }
    public void Shoot()
    {
        // Early exit checks (already good)
        if (playerCamera == null || playerObject == null || currentBulletCount <= 0 || currentCooldown > 0) return;

        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;
        if (playerRb != null)
        {
            float velmagnitude = playerRb.linearVelocity.magnitude;
            playerRb.linearVelocity = new Vector3(0, 0, 0);
            playerRb.AddForce(-direction * (knockbackForce + velmagnitude), ForceMode.Impulse);
        }
        // Optimization: Use SphereCastNonAlloc to avoid garbage allocation
        int hitCount = Physics.SphereCastNonAlloc(
            origin,
            sphereRadius,
            direction,
            sphereCastHits, // Use pre-allocated array
            maxDistance,
            targetLayers, // Use the LayerMask directly
            QueryTriggerInteraction.Ignore
        );

        int targetsHitThisShot = 0;

        // Iterate only up to the actual number of hits found
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = sphereCastHits[i]; // Get the hit info


            targetsHitThisShot++;
            if (isChallengeActive && !challengeCompleted)
            {
                shotTargetCount++;
            }

            Debug.Log("Hit Target: " + hit.collider.gameObject.name);
            bool hitState = playerMovement != null && playerMovement.State == PlayerMovement.MovementState.Running ? false : true;
            hit.collider.gameObject.SendMessage("Hit", hitState, SendMessageOptions.DontRequireReceiver);
            currentBulletCount++;
            targetsHit.Add(hit.collider.gameObject);
            // Debug Drawing (keep if useful)
            // Debug.DrawLine(origin, hit.point, Color.green, 2.0f);
            // Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.red, 2.0f);
        }
        print("Targets hit this shot: " + targetsHitThisShot);

        // Apply effects (knockback, cooldown, ammo reduction)
        // Consider if these should only happen if targetsHitThisShot > 0
        // The current logic applies them even on a miss.
        // if (targetsHitThisShot > 0) // Uncomment this line if effects should only apply on hit
        // {
        // Apply Knockback


        currentBulletCount--;
        currentCooldown = fireCooldown;
        // } // Uncomment this line if effects should only apply on hit
    }
    // Visualize the maximum range and radius of the spherecast in the Scene view
    private void OnDrawGizmos()
    {
        // Ensure we have a camera reference to draw from
        if (playerCamera == null) return;

        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        // Calculate the center of the sphere at maximum distance
        Vector3 sphereCenterAtMaxDist = origin + direction * maxDistance;

        // Set the color for the gizmo
        Gizmos.color = Color.yellow; // You can choose any color

        // Draw the wire sphere
        Gizmos.DrawWireSphere(sphereCenterAtMaxDist, sphereRadius);

        // Optionally, draw a line representing the cast direction
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, sphereCenterAtMaxDist);
    }
}

