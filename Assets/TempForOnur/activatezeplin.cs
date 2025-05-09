using UnityEngine;
using System.Collections;

public class ActivateZeppelin : MonoBehaviour
{
    [SerializeField] private GameObject zeppelin_child_1;
    [SerializeField] private GameObject zeppelin_child_2;

    [SerializeField] private float zeppelin_child_1_time = 3.0f;
    [SerializeField] private float zeppelin_child_2_time = 6.0f;

    void Start()
    {
        
        if (zeppelin_child_1 != null)
            zeppelin_child_1.SetActive(false);

        if (zeppelin_child_2 != null)
            zeppelin_child_2.SetActive(false);

        
        StartCoroutine(ActivateWithDelay(zeppelin_child_1, zeppelin_child_1_time));
        StartCoroutine(ActivateWithDelay(zeppelin_child_2, zeppelin_child_2_time));
    }

    
    private IEnumerator ActivateWithDelay(GameObject obj, float delay)
    {
        
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            obj.SetActive(true);
        }
    }
}