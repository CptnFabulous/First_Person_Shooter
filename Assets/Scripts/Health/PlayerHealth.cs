using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHealth : Health
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

    [HideInInspector] public PlayerHandler ph;

    private void Awake()
    {
        ph = GetComponent<PlayerHandler>();
    }

    public override void Die(DamageType causeOfDeath)
    {
        print("Player died of " + causeOfDeath + "!");

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        ph.pc.enabled = false;
        ph.wh.equippedWeapon.enabled = false;
        ph.wh.enabled = false;
        ph.hud.enabled = false;
    }
    
}
