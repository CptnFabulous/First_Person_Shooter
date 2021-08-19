using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//[RequireComponent(typeof (Selectable))]
public class SelectionData : MonoBehaviour, /*ISelectHandler, */IPointerEnterHandler, IPointerExitHandler
{
    public Text label;
    public Sprite graphic;
    public string flavourText;

    MenuWindow m;

    public void OnValidate()
    {
        if (label != null)
        {
            label.text = name;
        }
    }

    

    
    // Start is called before the first frame update



    void Awake()
    {
        m = GetComponentInParent<MenuWindow>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    /*
    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        m.PopulateSelectionInformation(flavourText, graphic);
        //throw new System.NotImplementedException();
    }
    */

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        m.PopulateSelectionInformation(flavourText, graphic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();

        m.PopulateSelectionInformation("", null);
    }
}
