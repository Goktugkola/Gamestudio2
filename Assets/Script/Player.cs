using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Dictionary<string, int> weaponDictionary = new Dictionary<string, int>
{
    { "ShotGun", 1 },
    { "SwingGun", 0 }
};
    [SerializeField] private GameObject SwingGun;
    [SerializeField] private GameObject ShotGun;
    [SerializeField] private int currentWeaponid;
    public string currentWeapon;
    private void Start()
    {
    }
    private void Update()
    {
        //HandleWeaponSwitch();
    }
    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeaponid = weaponDictionary["ShotGun"];
            ShotGun.SetActive(true);
            SwingGun.SetActive(false);
            currentWeapon = "ShotGun";
            if (gameObject.GetComponentInChildren<SpringJoint>() != null)
            { Destroy(gameObject.GetComponentInChildren<SpringJoint>());}
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeaponid = weaponDictionary["SwingGun"];
            ShotGun.SetActive(false);
            SwingGun.SetActive(true);
            currentWeapon = "SwingGun";
        }
    }
}
