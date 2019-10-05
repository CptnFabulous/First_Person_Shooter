using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Active,
    Paused,
    Won,
    Failed
}

public class GameStateHandler : MonoBehaviour
{
    public Canvas headsUpDisplay;
    public Canvas pauseMenu;
    public Canvas winMenu;
    public Canvas failMenu;

    GameState currentState;

    PlayerHandler ph;

    void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.Active);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (currentState == GameState.Active)
            {
                ChangeGameState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                ChangeGameState(GameState.Active);
            }
        }
    }

    public void ChangeGameState(GameState gm)
    {
        currentState = gm;
        switch(currentState)
        {
            case GameState.Active: // Resume game
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SwitchMenu(headsUpDisplay);
                ph.isActive = true;
                break;
            case GameState.Paused: // Pause game
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SwitchMenu(pauseMenu);
                ph.isActive = false;
                break;
            case GameState.Won: // Game is won, display win menu
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SwitchMenu(winMenu);
                ph.isActive = false;
                break;
            case GameState.Failed: // Game is lost, display fail menu
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SwitchMenu(failMenu);
                ph.isActive = false;
                break;
        }
    }

    void SwitchMenu(Canvas menu)
    {
        headsUpDisplay.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);
        failMenu.gameObject.SetActive(false);
        menu.gameObject.SetActive(true);
    }
}
