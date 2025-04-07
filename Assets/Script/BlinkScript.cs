using System;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlinkScript : MonoBehaviour
{
    [SerializeField] Animator Eyelids;
    [SerializeField] List<GameObject> ObjectsToDestroy;

    [SerializeField] GameObject PlayerToEnable;
    [SerializeField] Material SkyboxMaterial;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
        Eyelids.SetTrigger("Blink");
        PlayerToEnable.SetActive(true);
        RenderSettings.skybox = SkyboxMaterial;
        foreach (GameObject obj in ObjectsToDestroy)
            {
                Destroy(obj);
            }
        
        }
    }
}
