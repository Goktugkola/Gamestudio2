using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;

    private Rigidbody rb;
    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        
        // Initialize rotations
        xRotation = transform.eulerAngles.x;
        yRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        HandleMouseLook();

    }


    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical camera rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply camera rotation
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Horizontal player rotation
        yRotation += mouseX;
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(orientation.position, orientation.forward * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(orientation.position, orientation.right * 2);
    }
}