using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunMagazineStats : MonoBehaviour
{
    public Resource data;
    public int roundsReloadedPerCycle;
    public float reloadTime;

    public UnityEvent onReloadStart;
    public UnityEvent onRoundsReloaded;
    public UnityEvent onReloadEnd;



    /*
    public void ReloadHandler(Gun attachedGun)
    {
        // Checks if either the player has pressed the reload button, or the magazine is empty
        bool manualReload = Input.GetButtonDown("Reload") && data.current < data.max;
        bool automaticReload = data.current <= 0;

        // Checks for manual or automatic reload, and if the player is not already reloading
        if ((manualReload || automaticReload) && attachedGun.currentAction == null && RemainingAmmo() > 0)
        {
            // Starts the current reload sequence
            attachedGun.currentAction = ReloadSequence();
            attachedGun.StartCoroutine(attachedGun.currentAction);
        }

        // Put this into the main firing code
        if (Input.GetButtonDown("Fire") && data.current > 0)
        {
            CancelReloadSequence();
        }
    }

    IEnumerator ReloadSequence()
    {
        // Wait for firing sequence to end
        yield return new WaitUntil(() => fireControls.fireTimer >= 60 / fireControls.roundsPerMinute);

        // Start reload sequence, e.g. play animations

        magazine.onReloadStart.Invoke();

        while (magazine.data.current < magazine.data.max || RemainingAmmo() <= 0)
        {
            yield return new WaitForSeconds(magazine.reloadTime);
            // Add ammunition to the magazine based off roundsReloadedPerCycle. Unless there is not enough ammo for a full cycle, in which case load all remaining ammo.
            magazine.data.current += Mathf.Min(magazine.roundsReloadedPerCycle, RemainingAmmo());
            magazine.data.current = Mathf.Clamp(magazine.data.current, 0, magazine.data.max); // Ensure magazine is not overloaded

            magazine.onRoundsReloaded.Invoke();
        }

        // End reload sequence
        CancelReloadSequence();
    }

    void CancelReloadSequence()
    {
        if (reloading != null)
        {
            StopCoroutine(reloading);
            reloading = null;

            magazine.onReloadEnd.Invoke();
        }
    }

    int RemainingAmmo()
    {
        return playerHolding.handler.ammo.GetStock(general.ammoType) - magazine.data.current;
    }
    */
}