using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCosmeticEffect : MonoBehaviour
{
    public float timer = 1;
    public bool destroyOnEnd;

    public ParticleSystem[] particles;
    public AudioClip[] soundEffects;




    [System.Serializable]
    public struct AlterLightData
    {
        public Light light;
        public float rangeMin;
        public float rangeMax;
        public AnimationCurve rangeCurve;
        public float intensityMin;
        public float intensityMax;
        public AnimationCurve intensityCurve;
    }

    [System.Serializable]
    public struct AlterScale
    {
        public Transform transform;
        public Vector3 firstScale;
        public Vector3 secondScale;
        public AnimationCurve curve;

        void SetScale(float timer)
        {
            transform.localScale = Vector3.Lerp(firstScale, secondScale, curve.Evaluate(timer));
        }
    }

    [System.Serializable]
    public struct AlterMaterialProperty
    {
        public Renderer thingBeingRendered;
        public int materialIndex;
        public string property;
        public float minValue;
        public float maxValue;
        public AnimationCurve curve;

        void SetMaterialProperty(float timer)
        {
            float value = Mathf.Lerp(minValue, maxValue, curve.Evaluate(timer));
            thingBeingRendered.materials[materialIndex].SetFloat(property, value);
        }
    }







}
