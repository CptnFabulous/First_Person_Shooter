using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    
    public MenuWindow parent;
    public MenuWindow[] children;

    [Header("Scene elements")]
    public ScrollRect selections;
    public Text flavourText;
    public Image selectionGraphic;

    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */




    public void ReturnToPreviousWindow()
    {
        GetComponentInParent<MenuHandler>().ReturnToPreviousWindow();
    }

    public void ReturnToRootWindow()
    {
        GetComponentInParent<MenuHandler>().ReturnToRootWindow();
    }

    public void OnDisable()
    {
        // Reset window for next time
        flavourText = null;
        selectionGraphic.sprite = null;
    }
}
