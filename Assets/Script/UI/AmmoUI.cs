using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject AmmoEmpty;
    [SerializeField] private GameObject AmmoFull;
    [SerializeField] private Shotgun shotgun;
    void Update()
    {
        if (shotgun != null)
        {
            if(shotgun.ammoCurrent == shotgun.ammoMax)
            {
                AmmoEmpty.SetActive(false);
                AmmoFull.SetActive(true);
            }
            else
            {
                AmmoEmpty.SetActive(true);
                AmmoFull.SetActive(false);
            }
        }
    }
}
