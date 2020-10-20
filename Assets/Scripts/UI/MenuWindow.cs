using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    
    public MenuWindow parent;

    MenuHandler menuHandler;

    [Header("Scene elements")]
    public ScrollRect selections;
    public Text flavourText;
    public Image selectionGraphic;


    private void Awake()
    {
        menuHandler = GetComponentInParent<MenuHandler>();
    }

    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    public void ReturnToPreviousWindow()
    {
        menuHandler.SwitchWindow(parent);
    }

    public void OnDisable()
    {
        // Reset window for next time
        flavourText.text = null;
        selectionGraphic.sprite = null;
    }
}
