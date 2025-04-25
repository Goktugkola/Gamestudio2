using UnityEngine;

public class Shoes : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;


    public void Interact()
    {
            playerObject.GetComponent<WallRun>().enabled = true;
            playerObject.GetComponent<PlayerMovement>().canShiftToggle = true;
    }
}
