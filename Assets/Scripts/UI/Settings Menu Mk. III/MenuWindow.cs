using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : MonoBehaviour
{
    
    public MenuWindow parent;

    MenuHandler menuHandler;

    [Header("Scene elements")]
    public Selectable firstSelectedOption;
    public ScrollRect selections;
    public Text flavourText;
    public Image selectionGraphic;

    // Start is called before the first frame update
    void Start()
    {
        menuHandler = GetComponentInParent<MenuHandler>();
    }

    public void ReturnToPreviousWindow()
    {
        menuHandler.SwitchWindow(parent);
        //GetComponentInParent<MenuHandler>().SwitchWindow(parent);
    }

    public virtual void OnEnable()
    {
        // Reset window for next time
        PopulateSelectionInformation("", null);
    }

    public void PopulateSelectionInformation(string description, Sprite graphic)
    {
        if (flavourText != null)
        {
            flavourText.text = description;
        }

        if (selectionGraphic != null)
        {
            selectionGraphic.sprite = graphic;
        }
    }
}
