using UnityEngine;

public class ShoesText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject text;
    private void OnTriggerEnter(Collider other)
    {
        text.SetActive(true);
        other.gameObject.GetComponentInParent<WallRun>().enabled = true;
        other.gameObject.GetComponentInParent<PlayerMovement>().canShiftToggle = true;
        Destroy(gameObject, 0.2f);
    }
}
