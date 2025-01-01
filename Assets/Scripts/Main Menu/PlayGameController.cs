using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameController : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject pauseMenuCanvas;
    
    void Start()
    {
        Time.timeScale = 1;
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1;
        Paused = false;
    }

    public void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0;
        Paused = true;
    }
}
