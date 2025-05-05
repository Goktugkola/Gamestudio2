using System;
using UnityEngine;

public class FovController : MonoBehaviour
{
    public Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleFOVEffects();
    }
    private void HandleFOVEffects()//Could be put inside HandleSpeedLineEffects with the name of HandleSpeedEffects
    {                              //I didn't want to risk it without asking 
        if (rb.linearVelocity.magnitude >= 20f)
        {
            Camera.main.fieldOfView = 90f + Mathf.Sqrt(rb.linearVelocity.magnitude - 20f);
        }

        else
        {
            Camera.main.fieldOfView = 90f;
        }
    }
}
