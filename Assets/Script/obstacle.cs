using UnityEngine;

public class obstacle : MonoBehaviour
{
    Animation hitanim;
    void Hit()
    {
        if(hitanim != null)
        {
            hitanim.Play("HitAnimation"); // Replace with your animation name
        }
    }
}
