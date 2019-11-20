using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Generic Projectile", menuName = "ScriptableObjects/Projectiles/Generic Projectile", order = 1)]

public class ProjectileData : ScriptableObject
{
    public Projectile prefab;

    [Header("Physics")]
    public float velocity;
    public float diameter;
    public float gravityMultiplier;
    public LayerMask hitDetection = 1;

    
    public virtual GameObject NewProjectile(GameObject origin, Faction originFaction)
    {
        GameObject launchedProjectile = prefab.gameObject;
        Projectile p = launchedProjectile.GetComponent<Projectile>();

        p.velocity = velocity;
        p.diameter = diameter;
        p.gravityMultiplier = gravityMultiplier;
        p.hitDetection = hitDetection;
        p.origin = origin;
        p.originFaction = originFaction;

        return launchedProjectile;
    }
}
