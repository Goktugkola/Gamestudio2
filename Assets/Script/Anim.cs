using UnityEditor.Callbacks;
using UnityEngine;

public class Anim : MonoBehaviour
{
    public Animator anim;
    [SerializeField] AudioClip[] footstepSoundsWood;
    [SerializeField] AudioClip[] footstepSoundsGrass;
    [SerializeField] AudioClip[] footstepSoundsStone;
    [SerializeField] AudioSource footstepAudioSource;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float velocityMagnitude = rb.linearVelocity.magnitude;
        anim.speed = Mathf.Log(velocityMagnitude + 1, 2); // Using logarithm base 2
    }
    void playfootstep()
    {
        if (footstepAudioSource != null)
        {
            AudioClip clip = null;
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
