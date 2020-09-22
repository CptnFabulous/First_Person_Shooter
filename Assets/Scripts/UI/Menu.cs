﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("UI element prefabs")]
    public Button buttonPrefab;
    public Toggle togglePrefab;
    public Slider sliderPrefab;
    public Scrollbar scrollbarPrefab;
    public ScrollRect scrollPanelPrefab;
    public Dropdown dropdownPrefab;

    [Header("Scene elements")]
    public ScrollRect options;
    public ScrollRect flavourText;
    public Image icon;



    





    #region Actual functions
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {

        Debug.Log("Quitting to menu");
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion



}
