using UnityEngine;


public class PlayerEffectsController : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("The GameObject containing the speed line particle system or effect.")]
    [SerializeField] private GameObject speedLines;
    [Tooltip("The threshold speed for activating speed lines.")]
    [SerializeField] private float SpeedlineTreshold = 0.5f; 
    [Header("References")]
    public PlayerMovement playerMovement;
    public Rigidbody rb;
    [Header("WallRun")]
    [Tooltip("The GameObject representing the player's camera.")]
    [SerializeField] private GameObject playerCam;
    [SerializeField] private WallRun wallRun;
    
    void Awake()
    {

        // Validate references
        if (speedLines == null) Debug.LogError("Speed Lines GameObject not assigned!", this);
        if (playerMovement == null) Debug.LogError("PlayerMovement component not found!", this);
        if (rb == null) Debug.LogError("Rigidbody component not found!", this);
    }
    void Start()
    {
        if (playerMovement != null)
        {
        }
    }

    void Update()
    {
        HandleSpeedLineEffects();
        HandleWallRunEffects();
    }

    [Header("WallRun Effects")]
    [Tooltip("The speed at which the camera tilts during wall running.")]
    [SerializeField] private float wallRunTiltSpeed = 5f;

    private Quaternion targetCameraRotation = Quaternion.identity;

    private void HandleWallRunEffects()
    {
        // Early exit if references are not valid
        if (playerMovement == null || playerCam == null || wallRun == null) return;

        float targetZRotation = 0f;

        // Determine the target camera rotation only when wall running
        if (playerMovement.State == PlayerMovement.MovementState.WallRunning)
        {
            if (wallRun.isWallLeft)
            {
                targetZRotation = -15f;
            }
            else if (wallRun.isWallRight)
            {
                targetZRotation = 15f;
            }
        }
        // If not wall running, the target rotation is identity (no tilt)

        targetCameraRotation = Quaternion.Euler(playerCam.transform.localRotation.eulerAngles.x, playerCam.transform.localRotation.eulerAngles.y, targetZRotation);

        // Smoothly interpolate towards the target rotation
        // Only apply Slerp if the current rotation is not already the target
        if (playerCam.transform.localRotation != targetCameraRotation)
        {
            playerCam.transform.localRotation = Quaternion.Slerp(
                playerCam.transform.localRotation,
                targetCameraRotation,
                wallRunTiltSpeed * Time.deltaTime
            );
        }
    }

    private void HandleSpeedLineEffects()
    {
        // Early exit if references are not valid
        if (speedLines == null || playerMovement == null || rb == null) return;

        bool shouldShowSpeedLines = false;

        // Determine if speed lines should be active
        if (playerMovement.State == PlayerMovement.MovementState.Falling &&
            rb.linearVelocity.sqrMagnitude >= playerMovement.maxSpeed + SpeedlineTreshold)
        {
            shouldShowSpeedLines = true;
        }

        // Activate/Deactivate SpeedLines only if the state needs to change
        if (speedLines.activeSelf != shouldShowSpeedLines)
        {
            speedLines.SetActive(shouldShowSpeedLines);
        }
    }
}