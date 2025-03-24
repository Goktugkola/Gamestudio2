using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Sprite icon; // Icon to display in the UI when the player is in range of the interactable object
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }
}
