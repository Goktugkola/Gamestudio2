using UnityEditor.Callbacks;
using UnityEngine;

public class Anim : MonoBehaviour
{
    public Animator anim;
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
}
