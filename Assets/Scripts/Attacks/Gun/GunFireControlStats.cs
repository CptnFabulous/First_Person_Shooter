using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFireControlStats : MonoBehaviour
{
    public float roundsPerMinute;
    public int maxBurst;
    [HideInInspector] public float fireTimer;
    [HideInInspector] public float burstCounter;
}