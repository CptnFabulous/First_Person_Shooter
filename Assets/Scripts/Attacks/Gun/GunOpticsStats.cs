using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOpticsStats : MonoBehaviour
{
    public float magnification;
    public float transitionTime;
    public float moveSpeedReductionPercentage;


    public Transform aimPosition;

    public Transform sightLine;
    public Sprite scopeGraphic;
    public bool disableReticle;
}