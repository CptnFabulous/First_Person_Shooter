﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider))]
public class KillBox : MonoBehaviour
{
    public float damagePerSecond;
    public DamageType damageType;
    public bool isSevere;
    
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

    private void OnTriggerStay(Collider c)
    {
        DamageHitbox dh = c.GetComponent<DamageHitbox>();
        if (dh != null)
        {
            dh.Damage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime), gameObject, null, damageType, isSevere);
        }
    }
}
