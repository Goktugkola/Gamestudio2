using System.Collections;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 20f;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float spreadAngle = 10f;
    [SerializeField] private int bulletCount = 5;
    public int currentWeaponid = 0;
    public void Update()
    {
        HandleInput();
    }
    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && currentWeaponid == 0)
        {
            Shoot();
        }
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        if (bulletCount > 0 && fireCooldown > 0f)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
            rb.AddForce(firePoint.up * Random.Range(-spreadAngle, spreadAngle), ForceMode.Impulse);
            rb.AddForce(firePoint.right * Random.Range(-spreadAngle, spreadAngle), ForceMode.Impulse);
            Rigidbody playerRb = GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.AddForce(-firePoint.forward * bulletForce * 0.5f, ForceMode.Impulse);
            }
            bulletCount--;
            StartCoroutine(FireCooldownRoutine());
        }
    }
    private IEnumerator FireCooldownRoutine()
    {
        yield return new WaitForSeconds(fireCooldown);
        fireCooldown = 0f;
    }
}

