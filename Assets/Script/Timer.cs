using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float elapsedTime = 0f;
    [SerializeField] bool isRunning = false;
    [SerializeField] TextMeshProUGUI timerText; // Optional: Assign in Inspector
    private float referenceTime;
    private bool wasRunningOnDisable = false;

    // New field to track Time.timeScale of the previous frame
    private float previousTimeScale = 1f;

    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }

    void Awake()
    {
        // Initialize previousTimeScale to the current Time.timeScale
        previousTimeScale = Time.timeScale;

        // If isRunning is true (e.g., set in Inspector for auto-start),
        // initialize referenceTime correctly.
        if (isRunning)
        {
            referenceTime = Time.realtimeSinceStartup - elapsedTime;
        }
    }

    void Update()
    {
        // If the timer should be running AND the game just unpaused
        if (isRunning && Time.timeScale > 0f && previousTimeScale == 0f)
        {
            // Adjust referenceTime to seamlessly resume from the paused elapsedTime
            referenceTime = Time.realtimeSinceStartup - elapsedTime;
        }

        // Only accumulate time if the timer is running AND the game is not paused
        if (isRunning && Time.timeScale > 0f)
        {
            elapsedTime = Time.realtimeSinceStartup - referenceTime;
        }
        // If isRunning is false or Time.timeScale is 0, elapsedTime effectively pauses.

        UpdateTimerDisplayValues();

        if (timerText != null) // Check if timerText is assigned
        {
            timerText.text = GetFormattedTime();
        }
        
        // Store the current Time.timeScale for the next frame
        previousTimeScale = Time.timeScale;
    }

    private void UpdateTimerDisplayValues()
    {
        Hours = (int)(elapsedTime / 3600f);
        Minutes = (int)((elapsedTime % 3600f) / 60f);
        Seconds = (int)(elapsedTime % 60f);
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            // Adjust referenceTime to account for already elapsed time if resuming
            referenceTime = Time.realtimeSinceStartup - elapsedTime;
            // Ensure previousTimeScale is current, in case StartTimer is called while paused
            previousTimeScale = Time.timeScale;
        }
    }

    public void StopTimer()
    {
        if (isRunning)
        {
            // If stopping while game is not paused by timescale, update elapsedTime one last time.
            // If game is paused by timescale (Time.timeScale == 0), elapsedTime is already frozen correctly.
            if (Time.timeScale > 0f)
            {
                elapsedTime = Time.realtimeSinceStartup - referenceTime;
            }
            isRunning = false;
        }
    }

    void OnDisable()
    {
        if (isRunning)
        {
            // Update elapsedTime one last time before "pausing",
            // but only if the game isn't already paused by Time.timeScale.
            if (Time.timeScale > 0f)
            {
                elapsedTime = Time.realtimeSinceStartup - referenceTime;
            }
            wasRunningOnDisable = true;
            isRunning = false; // Stop the timer logic as the component is disabled
        }
    }

    void OnEnable()
    {
        // Initialize previousTimeScale with the current Time.timeScale
        // This ensures correct behavior in the first Update after enabling.
        previousTimeScale = Time.timeScale;

        if (wasRunningOnDisable)
        {
            // Restore the timer's state from before it was disabled.
            // elapsedTime already holds the correct value.
            // referenceTime needs to be set as if we are resuming.
            referenceTime = Time.realtimeSinceStartup - elapsedTime;
            isRunning = true; // Now, mark it as running.
            wasRunningOnDisable = false; // Clear the flag.
        }
    }

    public void ResetTimer()
    {
        isRunning = false;
        elapsedTime = 0f;
        referenceTime = Time.realtimeSinceStartup; // Reset reference time for a clean start if started again
        UpdateTimerDisplayValues(); // Update display values to zero
        if (timerText != null) // Also update text on reset
        {
            timerText.text = GetFormattedTime();
        }
    }

    public string GetFormattedTime()
    {
        return $"{Hours:00}:{Minutes:00}:{Seconds:00}";
    }

    public float GetElapsedTimeInSeconds()
    {
        return elapsedTime;
    }

    public bool IsRunning()
    {
        return isRunning; // This now reflects user's intent, actual running depends on Time.timeScale too
    }
}
