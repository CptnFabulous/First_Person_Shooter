using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


// How to use
// Add this percentage modifier to a VariableValueFloat in Start(), and enable and disable when appropriate. It will destroy itself when the class using it no longer exists.
[System.Serializable]
public class PercentageModifier // Works for both floats and ints
{
    // How much is the original float being influenced?
    public float percentageValue;
    // is the float multiplied by this value? If not, this value is used additively.
    public bool multiplicative;
    // If disabled, this modifier will only be counted if the original object is disabled
    public bool ignoresOriginActiveState;

    // Is this modifier actually active? Set to 1 to determine if the effect is counted, It's a float instead of a bool for if a value needs to be changed or slowly activated through a lerp.
    float intensity;
    // Which object is the source of this modifier? When calling the result, check if this object still exists, and destroy this class if not.
    [HideInInspector] public MonoBehaviour origin;

    #region Constructors
    public PercentageModifier(float _percentageValue, bool _multiplicative, bool _ignoresOriginActiveState)
    {
        percentageValue = _percentageValue;
        multiplicative = _multiplicative;
        ignoresOriginActiveState = _ignoresOriginActiveState;
    }

    public PercentageModifier()
    {
        // Creates an empty, default PercentageModifier
    }
    #endregion

    #region Enable/disable
    // Used for analog control over the effect's intensity
    public void SetIntensity(float f)
    {
        intensity = Mathf.Clamp(f, 0, 1);
    }

    // Used for turning the effect on and off in a binary state
    public void SetActiveFully(bool active)
    {
        if (active)
        {
            SetIntensity(1);
        }
        else
        {
            SetIntensity(0);
        }
    }
    #endregion

    #region Obtain values
    // What is the percentage being influenced by the intensity?
    public float Get()
    {
        return percentageValue * intensity / 100;
    }

    // Is the effect enabled?
    public bool IsActive()
    {
        // If the modifier itself is active
        // If either the original object is active OR the modifier has been set to be registered regardless
        return intensity > 0 && (origin.enabled == true || ignoresOriginActiveState);
    }
    #endregion
}

// Add modifier to variablevaluefloat in Awake() or Start(). Enable and disable when appropriate

[System.Serializable]
public class VariableValueFloat
{
    /*
    FloatModifiers are added to the list when the object is first created/enabled/whatever. Maybe in Start()?
    The modifiers are enabled/disabled when appropriate.
    Whenever Result() is called, it performs calculations based on which modifiers are active at the present time.
    It also checks the list and removes any modifiers which aren't supposed to exist anymore.
    I thought I had to figure out a horrible way to do this in Update(),
    but I realised that the list of modifiers only matters when it's being called,
    I can just perform calculations then.
    */
    
    public float defaultValue = 1; // The original value, when no external forces are acting on it
    [HideInInspector] public List<PercentageModifier> influencingPercentages; // A list of all modifiers influencing the final value

    public VariableValueFloat(float _defaultValue)
    {
        defaultValue = _defaultValue;
    }

    public VariableValueFloat()
    {
        // Create a default VariableValueFloat
    }

    // I don't need to figure out how to do this in Update() because it only needs to be checked when the variable is called
    public void ValidateModifiers()
    {
        // Check float modifiers and update by removing all instances that aren't supposed to exist anymore
        influencingPercentages.RemoveAll(fm => fm.origin == null);
    }

    public float Calculate()
    {
        ValidateModifiers();

        float finalValue = defaultValue;

        // Should I have some kind of code that only recalculates the variables if the list of modifiers change?
        float additivePercentage = 0;
        // Checks all modifiers
        foreach (PercentageModifier fm in influencingPercentages)
        {
            // Is it worth it to bother with the IsActive bool? Since a float is used to determine intensity, if it's set to zero then it'll just return zero
            if (fm.IsActive())
            {
                if (fm.multiplicative == true)
                {
                    // Directly multiply the value if it's a multiplicative modifier
                    finalValue *= fm.Get();
                }
                else
                {
                    // Otherwise, add it to the flat percentage multiplier at the end
                    additivePercentage += fm.Get();
                }
            }
        }

        return finalValue * (1 + additivePercentage);
    }

    public void Add(PercentageModifier pm, MonoBehaviour origin)
    {
        pm.origin = origin;
        influencingPercentages.Add(pm);
    }
}