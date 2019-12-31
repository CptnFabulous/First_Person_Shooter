using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealth : Health
{
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */

    public override void Die(DamageType causeOfDeath, GameObject lastAttacker)
    {
        base.Die(causeOfDeath, lastAttacker);
        Destroy(gameObject);
    }
}
