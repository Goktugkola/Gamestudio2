using UnityEngine;

using UnityEngine.SceneManagement;
using System;

public class StartGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private String SceneTo;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneTo);
        }
    }
}
