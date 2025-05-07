using UnityEngine;

public class GetSwing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject text;
    private void OnTriggerEnter(Collider other)
    {
        text.SetActive(true);
        other.gameObject.transform.parent.transform.parent.GetComponentInChildren<Swing>().transform.gameObject.SetActive(true);
        Destroy(gameObject, 0.2f);
    }
}
