using UnityEngine;

public class Anim : MonoBehaviour
{
    public Animator anim;

    [Header("Sound Effects")]
    [SerializeField] AudioSource SFXAudioSource;
    [Header("Dash")]
    [SerializeField] AudioClip dashSound;
    [Header("AirBlast")]
    [SerializeField] AudioClip airBlastSound;
    [Header("WallRun")]
    [SerializeField] AudioClip wallRunSound;
    [Header("Footstep Sounds")]
    [SerializeField] AudioClip[] footstepSoundsWood;
    [SerializeField] AudioClip[] footstepSoundsGrass;
    [SerializeField] AudioClip[] footstepSoundsStone;
    [SerializeField] AudioSource footstepAudioSource;
    [Header("References")]
    [SerializeField] public Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WallRun wallRun;
    [SerializeField] private Shotgun shotgun;
    [SerializeField] private GameObject AirBlastExplosionEffect;
    [SerializeField] private GameObject AirBlastEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float velocityMagnitude = rb.linearVelocity.magnitude;

        // Replace "YourSpecificStateName" with the actual name of the animator state
        // you want this dynamic speed to apply to.
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            anim.speed = Mathf.Log(velocityMagnitude + 1, 5f); // Adjust speed based on velocity
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            anim.speed = 5f; // Set a specific speed for the "Dash" state
        }
        else
        {
            // For any other state that is not "Dash" and not "YourSpecificStateName",
            // set a default speed (e.g., 1 for normal speed).
            anim.speed = 1f;
        }
        if (playerMovement.State == PlayerMovement.MovementState.Running)
        {
            anim.SetBool("IsGround", true);
        }
        else if (playerMovement.State == PlayerMovement.MovementState.WallRunning)
        {
            if (wallRun.isWallLeft)
            {
                if (wallRunSound != null)
                {
                    if (SFXAudioSource != null)
                    {
                        // Ensure the correct clip is assigned
                        if (SFXAudioSource.clip != wallRunSound)
                        {
                            SFXAudioSource.clip = wallRunSound;
                        }
                        // Ensure the sound will loop
                        SFXAudioSource.loop = true;
                        // Play the sound if it's not already playing
                        if (!SFXAudioSource.isPlaying)
                        {
                            SFXAudioSource.Play();
                        }
                    }
                }
                anim.SetBool("IsWallLeft", true);
                anim.SetBool("IsWallRight", false);
            }
            else if (wallRun.isWallRight)
            {
                if (wallRunSound != null)
                {
                    if (SFXAudioSource != null)
                    {
                        // Ensure the correct clip is assigned
                        if (SFXAudioSource.clip != wallRunSound)
                        {
                            SFXAudioSource.clip = wallRunSound;
                        }
                        // Ensure the sound will loop
                        SFXAudioSource.loop = true;
                        // Play the sound if it's not already playing
                        if (!SFXAudioSource.isPlaying)
                        {
                            SFXAudioSource.Play();
                        }
                    }
                }
                anim.SetBool("IsWallRight", true);
                anim.SetBool("IsWallLeft", false);
            }
        }
        else
        {
            if (SFXAudioSource != null)
            {
                SFXAudioSource.loop = false;
                SFXAudioSource.Stop();
            }
            anim.SetBool("IsWallLeft", false);
            anim.SetBool("IsWallRight", false);
            anim.SetBool("IsGround", false);
        }
        if (shotgun != null)
        {
            if (shotgun.IsDash)
            {
                anim.SetBool("IsDash", true);
                if (dashSound != null)
                {
                    SFXAudioSource.PlayOneShot(dashSound);
                    SFXAudioSource.loop = false;
                }
                anim.Play("Dash");
                disableDash();
            }
            if (shotgun.IsAirBlast)
            {
                anim.SetBool("IsAirBlast", true);
                if (airBlastSound != null)
                {
                    AirBlastExplosionEffect.SetActive(true);
                    SFXAudioSource.PlayOneShot(airBlastSound);
                    SFXAudioSource.loop = false;
                }
                anim.Play("AirBlast");
                disableAirBlast();
            }
        }
    }
    void disableAirBlast()
    {
        if (AirBlastExplosionEffect != null)
        {
            AirBlastEffect.SetActive(false);
            AirBlastExplosionEffect.SetActive(false);
        }
        shotgun.IsAirBlast = false;
        anim.SetBool("IsAirBlast", false);
    }
    void disableDash()
    {
        if (AirBlastExplosionEffect != null)
        {
            AirBlastEffect.SetActive(false);
            AirBlastExplosionEffect.SetActive(false);
        }
        shotgun.IsDash = false;
        anim.SetBool("IsDash", false);
    }
    void EnableExplosionEffect()
    {
        if (AirBlastExplosionEffect != null)
        {
            AirBlastExplosionEffect.SetActive(true);
        }
    }
    void EnableAirBlastEffect()
    {
        if (AirBlastEffect != null)
        {
            AirBlastEffect.SetActive(true);
        }
    }
    void playfootstep()
    {
        if (footstepAudioSource != null)
        {
            AudioClip clip = null;
            if (playerMovement != null)
            {
                if (playerMovement.State == PlayerMovement.MovementState.WallRunning)
                {
                    clip = footstepSoundsStone[Random.Range(0, footstepSoundsStone.Length)];
                }
            }
            switch (playerMovement.currentSurface)
            {
                case PlayerMovement.SurfaceType.Wood:
                    if (footstepSoundsWood != null && footstepSoundsWood.Length > 0)
                    {
                        clip = footstepSoundsWood[Random.Range(0, footstepSoundsWood.Length)];
                    }
                    break;
                case PlayerMovement.SurfaceType.Grass:
                    if (footstepSoundsGrass != null && footstepSoundsGrass.Length > 0)
                    {
                        clip = footstepSoundsGrass[Random.Range(0, footstepSoundsGrass.Length)];
                    }
                    break;
                case PlayerMovement.SurfaceType.Stone:
                    if (footstepSoundsStone != null && footstepSoundsStone.Length > 0)
                    {
                        clip = footstepSoundsStone[Random.Range(0, footstepSoundsStone.Length)];
                    }
                    break;
            }

            if (clip != null)
            {
                footstepAudioSource.PlayOneShot(clip);
            }
        }
    }
}
