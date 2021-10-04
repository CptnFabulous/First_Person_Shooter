using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// THIS CLASS IS IN PROGRESS AND DOES NOT WORK
/// </summary>
public class EnvironmentDependentEffect : MonoBehaviour
{
    /// <summary>
    /// THIS CLASS IS IN PROGRESS AND DOES NOT WORK (used for EnvironmentDependentEffect, which also doesn't work)
    /// </summary>
    public class Substance : ScriptableObject
    {
        public Material[] possibleMaterials;

        public static Substance[] AllTypes
        {
            get
            {
                return Resources.LoadAll<Substance>("");
            }
        }
    }


    //public Terrain t;

    public UnityEngine.UI.Slider.SliderEvent[] effects;


    public void InvokeEffects(RaycastHit hit)
    {
        int index;
        Renderer renderData = hit.collider.GetComponent<Renderer>();
        Substance[] types = Substance.AllTypes;
        for (int t = 0; t < types.Length; t++)
        {
            for (int m = 0; m < types[t].possibleMaterials.Length; m++)
            {
                if (types[t].possibleMaterials[m] == renderData.sharedMaterial)
                {
                    // Assigns the index to match this material, since 
                    index = t;
                    t = types.Length;
                    break;
                }
            }
        }
        
        //t.terrainData.GetA

        

        for (int i = 0; i < effects.Length; i++)
        {
            /*
            if (tag.transform.gameObject.CompareTag(effects[i].environmentTag))
            {

            }
            */
        }
        
        
    }
}
