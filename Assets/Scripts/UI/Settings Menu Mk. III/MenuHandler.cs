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

    /// <summary>
    /// UNTESTED: Loads a scene over a period of time, using a loading screen
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadingScreen"></param>
    /// <returns></returns>
    public IEnumerator LoadSceneWithLoadingScreen(string sceneName, string loadingScreen)
    {
        // Pause time to prevent time from passing in the new scene
        Time.timeScale = 0;

        // Unload current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);

        // Load a separate loading screen scene to mask things
        SceneManager.LoadScene(loadingScreen);

        // Load new scene in the background
        AsyncOperation levelLoading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Scene newScene = SceneManager.GetSceneByName(sceneName);
        
        yield return new WaitWhile(() => levelLoading.progress < 1);

        // Wait until a key is pressed
        WaitForEndOfFrame loop = new WaitForEndOfFrame();
        while(Input.anyKeyDown == false)
        {
            yield return loop;
        }

        // Once the actual scene is finished loading and the player has responded, unload the loading screen and resume normal time flow
        SceneManager.UnloadSceneAsync(loadingScreen);
        SceneManager.SetActiveScene(newScene);
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
