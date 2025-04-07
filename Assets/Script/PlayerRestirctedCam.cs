using UnityEngine;

public class PlayerRestirctedCam : MonoBehaviour
{
    public Transform player;
    [SerializeField] private float sensitivity = 50f;
    [SerializeField] private float sensMultiplier = 1f;
    public Transform orientation;

    public bool restrictView = false; // Bu boolean d��ar�dan true yap�lacak

    // K�s�tlama parametreleri
    public float horizontalLimit = 30f; // Sa�a-sola max derece fark�
    public float verticalLimit = 20f;   // Yukar�-a�a�� max derece fark�

    private float xRotation;
    private float desiredX;

    private float initialYaw;   // Ba�lang��taki yatay a��
    private float initialPitch; // Ba�lang��taki dikey a��

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
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Serbest modda dikey a�� s�n�r�
        }

        transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.localRotation = Quaternion.Euler(0, desiredX, 0);
    }
}