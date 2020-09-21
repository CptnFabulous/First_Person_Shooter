using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof (Selectable))]
public class SelectionData : MonoBehaviour
{
    public Sprite image;
    public string flavourText;

    public Selectable s;
    
    // Start is called before the first frame update
    void Awake()
    {
        s = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    
}
