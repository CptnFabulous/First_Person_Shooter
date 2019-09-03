using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect
{
    public string name; // A name used to easily reference certain effects
    public float value; // A multiplier for the stat being influenced. The values of every Effect are combined into a single multiplier that affects the desired stat.
    public float duration; // How long a particular effect lasts for.
}

public class StatModifier
{
    public List<StatEffect> appliedEffects;
}

public static class ModifyStat
{
    public static void ApplyEffect(StatModifier sm, string name, float value, float duration)
    {
        StatEffect se = new StatEffect(); // Creates new effect with appropriate variables
        se.name = name;
        se.value = value;
        se.duration = duration;
        sm.appliedEffects.Add(se); // ERROR HERE
    }

    public static float CompileEffects(StatModifier sm) // Compiles all effect values into a single multiplier
    {
        float f = 1; // 1 is default value to multiply by, i.e. nothing is changed;
        foreach (StatEffect se in sm.appliedEffects)
        {
            f += se.value; // Multiplier is increased or reduced
        }
        return f;
    }

    public static float ModifiedFloat(float f, StatModifier sm) // f equals whatever float you want to alter
    {
        return f * CompileEffects(sm); // Multiplies variable by compiled multiplier and returns
    }

    public static int ModifiedInt(int i, StatModifier sm) // i equals whatever int you want to alter
    {
        return Mathf.RoundToInt(i * CompileEffects(sm)); // Multiplies variable by compiled multiplier, rounds to an integer and returns
    }

    public static void CheckStatDuration(StatModifier sm) // Run this every update
    {
        foreach (StatEffect se in sm.appliedEffects) // (ERROR HERE) Checks each active effect
        {
            //e.duration -= Time.deltaTime; // Counts down duration of effect
            if (se.duration <= 0) // If effect duration has expired
            {
                sm.appliedEffects.Remove(se); // Remove from list of active effects
            }
            se.duration -= Time.deltaTime; // Counts down duration of effect
        }
    }
}
/*
public class StatModifier
{
    public List<StatEffect> appliedEffects;

    public static void ApplyEffect(StatModifier sm, string name, float value, float duration)
    {
        StatEffect se = new StatEffect(); // Creates new effect with appropriate variables
        se.name = name;
        se.value = value;
        se.duration = duration;
        sm.appliedEffects.Add(se); // ERROR HERE
    }

    public static float CompileEffects(StatModifier sm) // Compiles all effect values into a single multiplier
    {
        float f = 1; // 1 is default value to multiply by, i.e. nothing is changed;
        foreach (StatEffect se in sm.appliedEffects)
        {
            f += se.value; // Multiplier is increased or reduced
        }
        return f;
    }

    public static float ModifiedFloat(float f, StatModifier sm) // f equals whatever float you want to alter
    {
        return f * CompileEffects(sm); // Multiplies variable by compiled multiplier and returns
    }

    public static int ModifiedInt(int i, StatModifier sm) // i equals whatever int you want to alter
    {
        return Mathf.RoundToInt(i * CompileEffects(sm)); // Multiplies variable by compiled multiplier, rounds to an integer and returns
    }

    public static void CheckStatDuration(StatModifier sm) // Run this every update
    {
        foreach (StatEffect se in sm.appliedEffects) // (ERROR HERE) Checks each active effect
        {
            //e.duration -= Time.deltaTime; // Counts down duration of effect
            if (se.duration <= 0) // If effect duration has expired
            {
                sm.appliedEffects.Remove(se); // Remove from list of active effects
            }
            se.duration -= Time.deltaTime; // Counts down duration of effect
        }
    }
}


public class ModifyStat : MonoBehaviour
{
    
    public void ApplyEffect(StatModifier sm, string name, float value, float duration)
    {
        StatEffect se = new StatEffect(); // Creates new effect with appropriate variables
        se.name = name;
        se.value = value;
        se.duration = duration;
        sm.appliedEffects.Add(se); // ERROR HERE
    }

    public float CompileEffects(StatModifier sm) // Compiles all effect values into a single multiplier
    {
        float f = 1; // 1 is default value to multiply by, i.e. nothing is changed;
        foreach (StatEffect se in sm.appliedEffects)
        {
            f += se.value; // Multiplier is increased or reduced
        }
        return f;
    }

    public float ModifiedFloat(float f, StatModifier sm) // f equals whatever float you want to alter
    {
        return f * CompileEffects(sm); // Multiplies variable by compiled multiplier and returns
    }

    public int ModifiedInt(int i, StatModifier sm) // i equals whatever int you want to alter
    {
        return Mathf.RoundToInt(i * CompileEffects(sm)); // Multiplies variable by compiled multiplier, rounds to an integer and returns
    }

    public void CheckStatDuration(StatModifier sm) // Run this every update
    {
        foreach (StatEffect se in sm.appliedEffects) // (ERROR HERE) Checks each active effect
        {
            //e.duration -= Time.deltaTime; // Counts down duration of effect
            if (se.duration <= 0) // If effect duration has expired
            {
                sm.appliedEffects.Remove(se); // Remove from list of active effects
            }
            se.duration -= Time.deltaTime; // Counts down duration of effect
        }
    }
}
*/
/*
public class StatModifier : MonoBehaviour
{
    public List<StatEffect> appliedEffects;

    public void ApplyEffect(string name, float value, float duration)
    {
        StatEffect e = new StatEffect(); // Creates new effect with appropriate variables
        e.name = name;
        e.value = value;
        e.duration = duration;
        appliedEffects.Add(e); // ERROR HERE
    }

    public float CompileEffects() // Compiles all effect values into a single multiplier
    {
        float f = 1; // 1 is default value to multiply by, i.e. nothing is changed;
        foreach(StatEffect e in appliedEffects)
        {
            f += e.value; // Multiplier is increased or reduced
        }
        return f;
    }

    public float ModifiedFloat(float f) // f equals whatever float you want to alter
    {
        return f * CompileEffects(); // Multiplies variable by compiled multiplier and returns
    }

    public int ModifiedInt(int i) // i equals whatever int you want to alter
    {
        return Mathf.RoundToInt(i * CompileEffects()); // Multiplies variable by compiled multiplier, rounds to an integer and returns
    }

    void LateUpdate()
    {
        foreach (StatEffect e in appliedEffects) // (ERROR HERE) Checks each active effect
        {
            //e.duration -= Time.deltaTime; // Counts down duration of effect
            if (e.duration <= 0) // If effect duration has expired
            {
                appliedEffects.Remove(e); // Remove from list of active effects
            }
            e.duration -= Time.deltaTime; // Counts down duration of effect
        }
    }
}
*/

