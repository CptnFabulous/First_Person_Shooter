using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModifier
{
    public string name; // A name used to easily reference certain effects
    public float value; // A multiplier for the stat being influenced. The values of every Effect are combined into a single multiplier that affects the desired stat.
    public float duration; // How long a particular effect lasts for.
}

public static class ModifyStat
{
    public static void ApplyEffect(List<StatModifier> sm, string name, float value, float duration)
    {
        if(value != 0) // If statEffect actually has a value, if value is zero it does not modify the stat and is unnecessary
        {
            StatModifier se = new StatModifier(); // Creates new effect with appropriate variables
            se.name = name;
            se.value = value;
            se.duration = duration;
            sm.Add(se); // ERROR HERE
        }
    }

    public static float CompileEffects(List<StatModifier> sm) // Compiles all effect values into a single multiplier
    {
        float f = 1; // 1 is default value to multiply by, i.e. nothing is changed;
        foreach (StatModifier se in sm)
        {
            f += se.value; // Multiplier is increased or reduced
        }
        return f;
    }

    public static float NewFloat(float f, List<StatModifier> sm) // f equals whatever float you want to alter
    {
        return f * CompileEffects(sm); // Multiplies variable by compiled multiplier and returns
    }

    public static int NewInt(int i, List<StatModifier> sm) // i equals whatever int you want to alter
    {
        return Mathf.RoundToInt(i * CompileEffects(sm)); // Multiplies variable by compiled multiplier, rounds to an integer and returns
    }

    public static void CheckStatDuration(List<StatModifier> sm) // Run this every update
    {
        sm.RemoveAll(s => s.duration <= 0); // Removes effects with expired duration
        sm.RemoveAll(s => s.value == 0); // Removes effects that do not actually affect stat
        foreach (StatModifier se in sm)
        {
            se.duration -= Time.deltaTime; // Counts down duration of effect
        }
    }
}