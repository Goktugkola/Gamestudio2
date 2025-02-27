using UnityEngine;

public class LedgeJump : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform orientation;
    [SerializeField] private float positionMultiplierH;
    [SerializeField] private float positionMultiplierV;
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float ledgeDistance = 1.5f;
    [SerializeField] private Collider ledgeCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (Input.GetKey(KeyCode.Space) && ledgeCollider.bounds.max.y - playerObject.transform.position.y < ledgeDistance)
            {
                playerObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                playerObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
    public void Update()
    {
        ledgeCollider.gameObject.transform.position = playerObject.transform.position + orientation.forward * positionMultiplierV + -orientation.up * positionMultiplierH;
    }
}
