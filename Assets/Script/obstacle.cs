using UnityEngine;

public class obstacle : MonoBehaviour
{
    Animation hitanim;
    bool animEnded = false;
    void Start()
    {
        hitanim = GetComponent<Animation>();
        if (hitanim == null)
        {
            Debug.LogError("Animation component not found on the object.", this);
        }
    }
    void Hit()
    {
        if(hitanim != null)
        {
            hitanim.Play("Target"); // Replace with your animation name
        }
    }
}
