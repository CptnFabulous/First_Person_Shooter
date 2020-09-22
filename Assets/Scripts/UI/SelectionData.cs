using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof (Selectable))]
public class SelectionData : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text label;
    public Sprite graphic;
    public string flavourText;

    Menu m;

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
        m = GetComponentInParent<Menu>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        
        if (graphic != null && m.selectionGraphic != null)
        {
            m.selectionGraphic.sprite = graphic;
        }

        if (m.flavourText != null)
        {
            m.flavourText.text = flavourText;
        }
        
        //throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
