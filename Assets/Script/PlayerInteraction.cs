using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 2f;
    public LayerMask interactLayer;
    public GameObject interactUI; // Reference to the UI GameObject in the canvas

    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = Color.red; // Change color to red when in range
                interactUI.SetActive(true); // Enable the UI when an interactable object is in range
                interactUI.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Press E to interact with " + hit.collider.name;
                //interactUI.GetComponentInChildren<Image>().sprite = hit.collider.GetComponent<Interactable>().icon;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Call the interact method on the interactable object
                    hit.collider.SendMessage("Interact");
                }
                // Enable the interactable script on the object
                return;
            }
        }

        interactUI.SetActive(false); // Disable the UI if no interactable object is in range
    }

    private void Interact()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, interactLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                hit.collider.GetComponent<Interactable>().Interact();
            }
        }
    }
}
