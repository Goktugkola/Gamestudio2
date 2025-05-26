using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject text;
    [SerializeField] float speed;
    [SerializeField] GameObject Weapon;
    [SerializeField] GameObject playerObject;
    private void OnTriggerEnter(Collider other)
    {
        text.SetActive(true);
        text.GetComponent<Animator>().speed = speed;
        if(Weapon != null)
        Weapon.SetActive(true);
        if(playerObject !=null)
        playerObject.GetComponent<WallRun>().enabled = true;
        playerObject.GetComponent<PlayerMovement>().canShiftToggle = true;
        Destroy(gameObject, 0.2f);
    }
}
