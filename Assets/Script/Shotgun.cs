using System.Collections;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private float KnockbackForce = 20f;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private GameObject playerObject;
    public void Update()
    {
        HandleInput();
    }
    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

    }
    public void Shoot()
    {
        if (bulletCount > 0)
        {
            //Fire Anim and sound here
            //
            //Knockback
            Rigidbody playerRb = playerObject.GetComponentInChildren<Rigidbody>();
            if (playerRb != null)
            {
                print("Knockback");
                playerRb.AddForce(-gameObject.transform.forward * KnockbackForce, ForceMode.Impulse);
            }
            bulletCount--;

        }
    }

}

