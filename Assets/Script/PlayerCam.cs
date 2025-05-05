using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float sensitivity = 50f;
    [SerializeField] private float sensMultiplier = 1f;
    public Transform orientation;
    public float xRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        transform.position = player.transform.position;
        Look();
    }
    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = gameObject.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        gameObject.transform.localRotation = Quaternion.Euler(xRotation, desiredX, transform.localRotation.eulerAngles.z);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }
}
