using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float elapsedTime = 0f;
    [SerializeField] bool isRunning = false;
    [SerializeField] TextMeshProUGUI timerText; // Optional: Assign in Inspector
    private float referenceTime;
    private bool wasRunningOnDisable = false; // New field to track state

    // Public properties to access the current time components
    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }
    public int Milliseconds { get; private set; }

    // Optional: Assign a TextMeshProUGUI component in the Inspector to display the timer
    // public TextMeshProUGUI timerTextDisplay;

    void Update()
    {
        if (isRunning)
        {
            elapsedTime = Time.realtimeSinceStartup - referenceTime;
        }
        UpdateTimerDisplayValues();
        timerText.text = GetFormattedTime();// Debug output to console
        // Example of updating a TextMeshProUGUI element:
        // if (timerTextDisplay != null)
        // {
        //     timerTextDisplay.text = GetFormattedTime();
        // }
    }

    private void UpdateTimerDisplayValues()
    {
        Hours = (int)(elapsedTime / 3600f);
        Minutes = (int)((elapsedTime % 3600f) / 60f);
        Seconds = (int)(elapsedTime % 60f);
        Milliseconds = (int)((elapsedTime * 1000f) % 1000f);
    }

    /// <summary>
    /// Starts or resumes the timer.
    /// </summary>
    public void StartTimer()
    {
        if (!isRunning)
        {
            isRunning = true;
            // Adjust referenceTime to account for already elapsed time if resuming
            referenceTime = Time.realtimeSinceStartup - elapsedTime;
        }
    }

    /// <summary>
    /// Stops or pauses the timer.
    /// </summary>
    public void StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            // elapsedTime is already up-to-date from the Update loop
        }
    }

    void OnDisable()
    {
        if (isRunning)
        {
            // Update elapsedTime one last time before "pausing"
            elapsedTime = Time.realtimeSinceStartup - referenceTime;
            wasRunningOnDisable = true;
            isRunning = false; // Effectively pauses the timer logic in Update
        }
    }

    void OnEnable()
    {
        if (wasRunningOnDisable)
        {
            // To resume, we need to recalculate referenceTime based on the
            // elapsedTime when disabled and the new Time.realtimeSinceStartup
            referenceTime = Time.realtimeSinceStartup - elapsedTime;
            isRunning = true;
            wasRunningOnDisable = false;
        }
    }

    /// <summary>
    /// Resets the timer to zero.
    /// </summary>
    public void ResetTimer()
    {
        isRunning = false;
        elapsedTime = 0f;
        // referenceTime will be correctly set if StartTimer is called next
        UpdateTimerDisplayValues(); // Update display values to zero
    }

    /// <summary>
    /// Gets the current elapsed time as a formatted string (HH:MM:SS.mmm).
    /// </summary>
    /// <returns>Formatted time string.</returns>
    public string GetFormattedTime()
    {
        return $"{Hours:00}:{Minutes:00}:{Seconds:00}.{Milliseconds:000}";
    }

    /// <summary>
    /// Gets the raw elapsed time in seconds.
    /// </summary>
    /// <returns>Elapsed time in seconds.</returns>
    public float GetElapsedTimeInSeconds()
    {
        return elapsedTime;
    }

    /// <summary>
    /// Checks if the timer is currently running.
    /// </summary>
    /// <returns>True if the timer is running, false otherwise.</returns>
    public bool IsRunning()
    {
        return isRunning;
    }
}
