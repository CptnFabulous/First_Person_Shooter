﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string properName;
    public string description;
    public bool isUniquelyNamed;





    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
