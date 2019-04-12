using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanels : MonoBehaviour
{
    public static bool EnoughLemmingsHavePassed = false;
    public static bool GameIsPaused = false;
    public GameObject victoryMenu;
    public GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))

        {
            if (GameIsPaused)
            {
                Resume();
            }

            else
            {
                Pause();
            }
        }

        if (EnoughLemmingsHavePassed)
        {
            VictoryMenuPopUp();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void VictoryMenuPopUp()
    {
        victoryMenu.SetActive(true);
        Time.timeScale = 0f;
    }

}