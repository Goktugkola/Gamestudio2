using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
public class MenuScreen : MonoBehaviour
{
    [SerializeField] private bool isMenuActive = false;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject menuScreen; // Assign in Inspector
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuScreen.SetActive(isMenuActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }
    public void ToggleMenu()
    {
          isMenuActive = !isMenuActive;
    menuScreen.SetActive(isMenuActive);

    if (isMenuActive)
    {
        Time.timeScale = 0; // Pause the game
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
    }
    else
    {
        Time.timeScale = 1; // Resume the game
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        #if UNITY_EDITOR
    EditorApplication.isPaused = true; // Oyunu durdurur
#endif
        Application.Quit();
        Debug.Log("Quit Game");
    }
    public void RestrartGame()
    {
        Debug.Log("Restart Game");
        SceneManager.LoadScene(sceneToLoad);
        Time.timeScale = 1; // Resume the game
    }
}
