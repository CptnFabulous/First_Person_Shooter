using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOpticsStats : MonoBehaviour
{
    public float magnification = 4;
    public float transitionTime = 0.5f;
    public float moveSpeedReductionPercentage = 0.5f;
    //public float aimSwaySpeed = 1;
    public bool disableReticle;

    [Header("Cosmetics")]
    public Transform aimPosition;
    public Transform sightLine;
    public Sprite scopeGraphic;
    public float whenToDisableWeaponVisual = 0.5f;




}