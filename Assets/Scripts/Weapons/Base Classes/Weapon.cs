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
    [Tooltip("The amount of damage a weapon deals with a standard attack.")]
    public int damage = 10;
    [Tooltip("The time taken to draw or put away the weapon.")]
    public float switchSpeed = 0.75f;
    public Sprite icon;
    [HideInInspector]
    public WeaponType type;
}
