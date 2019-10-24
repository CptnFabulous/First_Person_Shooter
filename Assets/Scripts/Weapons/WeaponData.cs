using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponData : ScriptableObject
{
    public string description;
    public Sprite icon;
    public RangedWeapon equipPrefab;
    public ItemPickup dropPrefab;
}
