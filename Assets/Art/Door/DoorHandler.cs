using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public bool isLocked;

    public Animator animationHandler;
    public string openBoolName;
    public Interactable openFunction;
    public Interactable closeFunction;

    IEnumerator toggleStateTimer;

    public void SetOpen(bool openState)
    {
        animationHandler.StartPlayback();
        animationHandler.SetBool(openBoolName, openState);

        if (openState == true)
        {
            toggleStateTimer = Open();
        }
        else
        {
            toggleStateTimer = Close();
        }

        StartCoroutine(toggleStateTimer);
    }

    IEnumerator Open()
    {
        yield return new WaitForSeconds(openFunction.cooldown);
        openFunction.enabled = false;
        openFunction.gameObject.SetActive(false);
        closeFunction.enabled = true;
        closeFunction.gameObject.SetActive(true);
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(closeFunction.cooldown);
        closeFunction.enabled = false;
        closeFunction.gameObject.SetActive(false);
        openFunction.enabled = true;
        openFunction.gameObject.SetActive(true);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        CancelAnimation();
    }
    */
    public void CancelAnimation()
    {
        StopCoroutine(toggleStateTimer);
        animationHandler.StopPlayback();
    }
}
