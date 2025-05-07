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

            
            other.transform.parent.transform.parent.GetComponentInChildren<Camera>().transform.position = CheckPoint.transform.position;
            other.transform.parent.transform.parent.GetComponentInChildren<Camera>().transform.rotation = CheckPoint.transform.rotation;
            other.transform.parent.transform.position = CheckPoint.transform.position;
            other.transform.parent.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        }
    }
}
