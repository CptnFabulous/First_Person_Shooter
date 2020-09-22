using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof (Selectable))]
public class SelectionData : MonoBehaviour//, IPointerEnterHandler
{
    public Sprite graphic;
    public string flavourText;

    public Selectable s;

    public Menu m;
    
    // Start is called before the first frame update
    void Awake()
    {
        s = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnSelect(BaseEventData eventData)
    {
        // Do something.
        //Debug.Log("<color=red>Event:</color> Completed selection.");

        m.icon.sprite = graphic;
        
    }

}
