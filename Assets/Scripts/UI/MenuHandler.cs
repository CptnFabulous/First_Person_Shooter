using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    /*
    [Header("UI element prefabs")]
    public Button buttonPrefab;
    public Toggle togglePrefab;
    public Slider sliderPrefab;
    public Scrollbar scrollbarPrefab;
    public ScrollRect scrollPanelPrefab;
    public Dropdown dropdownPrefab;
    */

    [HideInInspector] public MenuWindow[] differentMenus;
    [HideInInspector] public MenuWindow rootWindow;
    [HideInInspector] public MenuWindow currentMenu;

    private void Start()
    {
        print("Setting up menu");
        differentMenus = GetComponentsInChildren<MenuWindow>();
        foreach(MenuWindow m in differentMenus)
        {
            // Searches for a menu component in m's parent. The first menu with no parent becomes the root.
            m.parent = m.transform.parent.GetComponent<MenuWindow>();
            if (m.parent == null)
            {
                rootWindow = m;
            }

            // Searches for all MenuWindow scripts that are immediate children of m
            //m.children = GetComponentsInChildren<MenuWindow>().Where(.transform.parent == m.transform && x != m);

            /*
            // Requires System.Linq
            MenuWindow[] c = GetComponentsInChildren<MenuWindow>();
            List<MenuWindow> list = new List<MenuWindow>();
            foreach (MenuWindow mc in c.Where(x => x.transform.parent == m.transform && x != m))
            {
                list.Add(mc);
            }
            m.children = list.ToArray();
            */

            MenuWindow[] c = GetComponentsInChildren<MenuWindow>();
            List<MenuWindow> list = new List<MenuWindow>();
            foreach (MenuWindow mc in c)
            {
                if (mc.transform.parent == m.transform && mc != m)
                {
                    list.Add(mc);
                }
            }
            m.children = list.ToArray();

            // Separates m from its hierarchy, so it can be active even if its parent is disabled.
            m.transform.parent = transform;
        }

        ReturnToRootWindow();

    }


    #region Navigation

    public void SwitchToDifferentWindow(MenuWindow newMenu)
    {
        foreach(MenuWindow m in differentMenus)
        {
            if (m != newMenu)
            {
                m.gameObject.SetActive(false);
            }
            else
            {
                m.gameObject.SetActive(true);
                currentMenu = m;
            }
        }
    }

    public MenuWindow GetCurrentMenu()
    {
        foreach (MenuWindow m in differentMenus)
        {
            if (m.gameObject.activeSelf == true)
            {
                return m;
            }
        }
        return null;
    }

    public void ReturnToPreviousWindow()
    {
        SwitchToDifferentWindow(currentMenu.parent);
    }

    public void ReturnToRootWindow()
    {
        SwitchToDifferentWindow(rootWindow);
    }

    #endregion



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
