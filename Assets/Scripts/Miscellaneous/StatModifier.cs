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
    public List<StatEffect> effects = new List<StatEffect>();

    public void ApplyEffect(string name, float value, float duration)
    {
        StatEffect se = new StatEffect(); // Creates new effect with appropriate variables
        se.name = name;
        se.value = value;
        se.duration = duration;
        effects.Add(se);
    }

    public float CompileEffects()
    {
        float f = 1; // 1 is default value to multiply by, i.e. nothing is changed
        foreach (StatEffect se in effects)
        {
            f += se.value; // Multiplier is increased or reduced
        }
        return f;
    }

    public float NewFloat(float f) // f equals whatever float you want to alter
    {
        return f * CompileEffects(); // Multiplies variable by compiled multiplier and returns
    }

    public int NewInt(int i) // i equals whatever int you want to alter
    {
        return Mathf.RoundToInt(i * CompileEffects()); // Multiplies variable by compiled multiplier, rounds to an integer and returns
    }

    public void CheckStatDuration() // Run this every update
    {
        effects.RemoveAll(s => s.duration <= 0); // Removes effects with expired duration
        effects.RemoveAll(s => s.value == 0); // Removes effects that do not actually affect stat
        foreach (StatEffect se in effects)
        {
            se.duration -= Time.deltaTime; // Counts down duration of effect
        }
    }
}