using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : ItemPickup
{
    public WeaponData data;

    public override void Pickup(Collider c)
    {
        base.Pickup(c);
    }
}
