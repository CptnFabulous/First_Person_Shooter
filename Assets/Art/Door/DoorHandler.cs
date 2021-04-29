using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public Animator animationHandler;
    public string openBoolName;
    
    public Interactable openFunction;
    public Interactable closeFunction;
    


    IEnumerator SetOpen(bool openState)
    {
        animationHandler.SetBool(openBoolName, openState);

        Interactable y;
        Interactable n;
        if (openState == true)
        {
            y = openFunction;
            n = closeFunction;
        }
        else
        {
            y = closeFunction;
            n = openFunction;
        }

        //yield return new WaitForSeconds(y.possibleInteractions[y.currentlyActive].cooldown);
        yield return new WaitForSeconds(y.cooldown);


        y.enabled = false;
        n.enabled = true;
    }
}
