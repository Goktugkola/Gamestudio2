using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        transform.position = player.transform.position;
    }
}
