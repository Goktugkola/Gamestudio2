using UnityEditor.Callbacks;
using UnityEngine;

public class Anim : MonoBehaviour
{
    public Animator anim;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioSource footstepAudioSource;
    [SerializeField] public Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        anim.speed = rb.linearVelocity.magnitude/5;
    }
    void playfootstep()
    {
        if (footstepAudioSource != null && footstepSounds.Length > 0)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            footstepAudioSource.PlayOneShot(clip);
        }
    }
}
