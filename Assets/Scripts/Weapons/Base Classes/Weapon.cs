using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    projectile,
    melee,
    throwable
}

public abstract class Weapon : MonoBehaviour
{
    [Header("General Stats")]
    public int damage = 10;
    public float switchSpeed = 0.75f;
    public Sprite icon;
    [HideInInspector]
    public WeaponType type;
}
