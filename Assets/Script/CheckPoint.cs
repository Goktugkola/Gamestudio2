using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool Checked = false;
    public DeadArea deadArea;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !Checked)
        {
            deadArea.CheckPoint = gameObject;
            Checked = true;
        }
    }
}
