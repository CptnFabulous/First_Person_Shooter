using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public Canvas pauseMenu;
    public Canvas headsUpDisplay;

    public bool startPaused;

    bool isPaused;

    // Use this for initialization
    void Start()
    {

        if (startPaused)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }

        }
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        headsUpDisplay.enabled = false;
        pauseMenu.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    void UnpauseGame()
    {
        Time.timeScale = 1;
        pauseMenu.enabled = false;
        headsUpDisplay.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }
}
