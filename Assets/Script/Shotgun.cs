using System.Collections;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private float KnockbackForce = 20f;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private int maxBulletCount = 5;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Vector3 origin;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private Vector3 direction;
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse1;
    public void Update()
    {
        HandleInput();
        if(playerObject.GetComponent<PlayerMovement>().grounded)
        {
            bulletCount = maxBulletCount;
        }
    }
    public void HandleInput()
    {
        if (Input.GetKeyDown(fireKey))
        {
            Shoot();
        }

    }
    public void Shoot()
    {
        if (bulletCount > 0)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
            origin: origin,
            radius: sphereRadius,
            direction: gameObject.transform.forward,
            maxDistance: maxDistance,
            layerMask: targetLayers,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );
            for (int i =0; i < hits.Length; i++)
            {
                print(hits[i].collider.gameObject.name);
                Destroy(hits[i].collider.gameObject);
                Debug.DrawRay(hits[i].point, hits[i].normal * 2, Color.red, 2.0f);
            }
            
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

