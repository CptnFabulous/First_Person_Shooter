using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider))]
public class WinZone : MonoBehaviour
{
    public string nextLevelName;

    private void OnTriggerEnter(Collider c)
    {
        PlayerHandler ph = c.GetComponent<PlayerHandler>();
        if (ph != null)
        {
            // level completed state
            print("End of level reached");
        }
    }
}
