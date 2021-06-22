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
}