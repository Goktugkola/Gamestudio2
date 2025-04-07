using UnityEngine;

public class DeadArea : MonoBehaviour
{
    public GameObject CheckPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print(other.GetComponentInParent<PlayerInteraction>().gameObject.GetComponentInChildren<Camera>().gameObject.name);
            print(other.GetComponentInParent<PlayerMovement>().gameObject.name);

            other.GetComponentInParent<PlayerInteraction>().gameObject.GetComponentInChildren<Camera>().transform.position = CheckPoint.transform.position;
            other.GetComponentInParent<PlayerMovement>().gameObject.transform.position = CheckPoint.transform.position;
        }
    }
}
