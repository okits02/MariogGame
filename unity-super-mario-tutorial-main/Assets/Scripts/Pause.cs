using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI;  
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);  
        Time.timeScale = 0f; 
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);  
        Time.timeScale = 1f; 
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;  
        SceneManager.LoadScene("Menu");  
    }
}
