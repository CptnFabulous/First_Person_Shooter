using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem), typeof(StandaloneInputModule))]
public class MenuHandler : MonoBehaviour
{
    /*
    [System.Serializable]
    public class MenuAndChildren
    {
        public MenuWindow window;
        public MenuAndChildren[] children;
    }

    public MenuAndChildren hierarchy;
    */
    EventSystem eventSystem;
    StandaloneInputModule menuInputHandler;


    public MenuWindow rootWindow;

    


    [HideInInspector] public MenuWindow[] differentWindows;
    public MenuWindow CurrentWindow { get; private set; }




    
    







    private void Awake()
    {
        eventSystem = GetComponent<EventSystem>();
        menuInputHandler = GetComponent<StandaloneInputModule>();
        
        differentWindows = GetComponentsInChildren<MenuWindow>(true);
        //SortMenus();
    }

    private void OnEnable()
    {
        SwitchWindow(rootWindow);
    }

    public void SwitchWindow(MenuWindow newWindow)
    {
        foreach (MenuWindow w in differentWindows)
        {
            w.gameObject.SetActive(false);
        }
        newWindow.gameObject.SetActive(true);
        CurrentWindow = newWindow;
        eventSystem.firstSelectedGameObject = CurrentWindow.firstSelectedOption.gameObject;
    }


    #region Functions

    public void ResumeGame()
    {
        GameStateHandler gsh = GetComponentInParent<GameStateHandler>();
        gsh.ResumeGame();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // UNTESTED
    public IEnumerator LoadSceneWithLoadingScreen(string sceneName, string loadingScreen)
    {
        // Pause time to prevent time from passing in the new scene
        Time.timeScale = 0;

        // Reference current scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Load a separate loading screen scene to mask things
        SceneManager.LoadScene(loadingScreen);

        // Load new scene in the background
        AsyncOperation levelLoading = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitWhile(() => levelLoading.progress < 1);

        // Once the actual scene is finished loading, unload the loading screen and resume normal time flow
        SceneManager.UnloadSceneAsync(currentScene);
        Time.timeScale = 1;
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
