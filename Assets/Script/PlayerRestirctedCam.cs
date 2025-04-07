using UnityEngine;

public class PlayerRestirctedCam : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float sensitivity = 50f;
    [SerializeField] private float sensMultiplier = 1f;
    public Transform orientation;

    public bool restrictView = false; // Bu boolean dýþarýdan true yapýlacak

    // Kýsýtlama parametreleri
    public float horizontalLimit = 30f; // Saða-sola max derece farký
    public float verticalLimit = 20f;   // Yukarý-aþaðý max derece farký

    private float xRotation;
    private float desiredX;

    private float initialYaw;   // Baþlangýçtaki yatay açý
    private float initialPitch; // Baþlangýçtaki dikey açý

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 rot = transform.localRotation.eulerAngles;
        initialYaw = rot.y;
        initialPitch = rot.x;
    }

    void Update()
    {
        transform.position = player.position;
        Look();
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime * sensMultiplier;

        desiredX += mouseX;
        xRotation -= mouseY;

        if (restrictView)
        {
            desiredX = Mathf.Clamp(desiredX, initialYaw - horizontalLimit, initialYaw + horizontalLimit);
            xRotation = Mathf.Clamp(xRotation, initialPitch - verticalLimit, initialPitch + verticalLimit);
        }
        else
        {
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Serbest modda dikey açý sýnýrý
        }

        transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.localRotation = Quaternion.Euler(0, desiredX, 0);
    }
}