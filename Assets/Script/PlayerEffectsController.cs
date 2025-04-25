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