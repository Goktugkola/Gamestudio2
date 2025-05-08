using System.Collections;
using UnityEngine;

public class obstacle : MonoBehaviour
{
    Animation hitanim;
    bool animEnded = false;
    [SerializeField] float timer = 0.5f;
    [SerializeField] float defaultspeed = 1f;
    [SerializeField] float slowSpeed = 0.1f;
    void Start()
    {
        hitanim = GetComponent<Animation>();
        if (hitanim == null)
        {
            Debug.LogError("Animation component not found on the object.", this);
        }
    }
    void Hit(bool onair)
    {
        if(hitanim != null)
        {
            hitanim.Play("Target"); // Replace with your animation name
            hitanim["Target"].speed = defaultspeed; // Set the animation speed to default
            GetComponent<Collider>().enabled = false; // Disable the collider to prevent further hits
            hitanim["Target"].wrapMode = WrapMode.Loop;
            if(onair)
            {StartCoroutine(SlowDownCoroutine());}
            
        }
    }
    public void StopAnimation()
    {
        if (hitanim != null)
        {
            hitanim["Target"].wrapMode = WrapMode.Once; // Replace with your animation name
            animEnded = true;
            GetComponent<Collider>().enabled = true; // Re-enable the collider after the animation ends
        }
    }
    private IEnumerator SlowDownCoroutine()
    {
        defaultspeed = hitanim["Target"].speed; // Store the default speed
        float tempspeed = hitanim["Target"].speed; // Store the current speed
        while (tempspeed > slowSpeed) // Slow down the animation
        {
            tempspeed -= Time.deltaTime / 2f; // Adjust the speed reduction rate as needed
            hitanim["Target"].speed = tempspeed;
            yield return null; // Wait for the next frame
        }
        yield return null; // Wait for the animation to finish

    }
}
