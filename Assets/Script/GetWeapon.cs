using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject text;
    [SerializeField] float speed;
    [SerializeField] GameObject Weapon;
    [SerializeField] WallRun wallRun;
    private void OnTriggerEnter(Collider other)
    {
        text.SetActive(true);
        text.GetComponent<Animator>().speed = speed;
        if(Weapon != null)
        Weapon.SetActive(true);
        if(wallRun !=null)
        wallRun.enabled = true;
        Destroy(gameObject, 0.2f);
    }
}
