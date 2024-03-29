﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Misc
{
    #region Calculating perpendicular Vector3 directions
    public static Vector3 PerpendicularRight(Vector3 forward, Vector3 worldUp)
    {
        return Vector3.Cross(forward, worldUp).normalized;
    }

    public static Vector3 PerpendicularUp(Vector3 forward, Vector3 worldUp)
    {
        Vector3 worldRight = PerpendicularRight(forward, worldUp);
        return Vector3.Cross(forward, worldRight).normalized;
    }

    public static Vector3 AngledDirection(Vector3 axes, Vector3 forward, Vector3 upward)
    {
        Vector3 right = PerpendicularRight(forward, upward);
        Vector3 up = PerpendicularUp(forward, upward);
        return Quaternion.AngleAxis(axes.x, right) * Quaternion.AngleAxis(axes.y, up) * Quaternion.AngleAxis(axes.z, forward) * forward;
    }
    #endregion

    #region Inverse clamp
    public static float InverseClamp(float value, float min, float max)
    {
        if (value > max)
        {
            return min;
        }
        else if (value < min)
        {
            return max;
        }
        else
        {
            return value;
        }
    }
    public static int InverseClamp(int value, int min, int max)
    {
        if (value > max)
        {
            return min;
        }
        else if (value < min)
        {
            return max;
        }
        else
        {
            return value;
        }
    }
    #endregion

    #region MultiLerp
    public static float MultiLerp(float[] values, float t)
    {
        #region Deals with arrays too short to work with properly
        if (values.Length == 1)
        {
            return values[0];
        }
        else if (values.Length <= 0)
        {
            return 0;
        }
        #endregion

        float segment = 1 / values.Length - 1;
        float wayThroughDecimal = t / segment;

        float percentageThroughCurrentLerp = GetDecimalFromFloat(wayThroughDecimal);
        int lerpsCompleted = Mathf.Min(Mathf.RoundToInt(wayThroughDecimal), Mathf.RoundToInt(wayThroughDecimal - 1));

        return Mathf.Lerp(values[lerpsCompleted], values[lerpsCompleted + 1], percentageThroughCurrentLerp);
    }

    public static Vector3 Vector3MultiLerp(Vector3[] values, float t)
    {
        #region Deals with arrays too short to work with properly
        if (values.Length == 1)
        {
            return values[0];
        }
        else if (values.Length <= 0)
        {
            return Vector3.zero;
        }
        #endregion

        float segment = 1 / values.Length - 1;
        float wayThroughDecimal = t / segment;

        float percentageThroughCurrentLerp = GetDecimalFromFloat(wayThroughDecimal);
        int lerpsCompleted = Mathf.Min(Mathf.RoundToInt(wayThroughDecimal), Mathf.RoundToInt(wayThroughDecimal - 1));
        
        return Vector3.Lerp(values[lerpsCompleted], values[lerpsCompleted + 1], percentageThroughCurrentLerp);
    }

    public static Quaternion QuaternionMultiLerp(Quaternion[] values, float t)
    {
        #region Deals with arrays too short to work with properly
        if (values.Length == 1)
        {
            return values[0];
        }
        else if (values.Length <= 0)
        {
            return Quaternion.identity;
        }
        #endregion

        float segment = 1 / values.Length - 1;
        float wayThroughDecimal = t / segment;

        float percentageThroughCurrentLerp = GetDecimalFromFloat(wayThroughDecimal);
        int lerpsCompleted = Mathf.Min(Mathf.RoundToInt(wayThroughDecimal), Mathf.RoundToInt(wayThroughDecimal - 1));

        return Quaternion.Lerp(values[lerpsCompleted], values[lerpsCompleted + 1], percentageThroughCurrentLerp);
    }
    #endregion

    #region Physics
    public static LayerMask CollisionMask(int layer)
    {
        // Checks collisions for each layer against our current one, and if true adds that layer to the mask
        LayerMask lm = 0;
        for (int i = 0; i < 32; i++)
        {
            if (Physics.GetIgnoreLayerCollision(layer, i) == false)
            {
                lm = lm | 1 << i;
            }
        }

        // Returns a layer mask of all the layers that will collide with our starting layer
        return lm;
    }
    #endregion

    #region UI

    /*
    public static string GetButtonPrompt(string input)
    {


        string[] blah = Input.GetJoystickNames();



        return null; 
    }
    
    public static Sprite GetButtonPrompt(string input)
    {

    }
    */
    #endregion

    #region X where child is at the same X as another separate X (find better names for these)
    public static Vector3 PositionWhereChildIsAtSamePositionAsAnotherTransform(Vector3 transformToMove, Vector3 child, Vector3 position)
    {
        /*
        Vector3 posA = wmt.position;
        Vector3 posB = optics.sightLine.position;
        Vector3 posC = pc.head.position;
        Vector3 relativePosition = posC + (posA - posB);
        */
        return position + (transformToMove - child);
    }

    public static Quaternion RotationWhereChildIsAtSameRotationAsAnotherTransform(Quaternion transformToMove, Quaternion child, Quaternion position)
    {
        /*
        Quaternion rotA = wmt.rotation;
        Quaternion rotB = optics.sightLine.rotation;
        Quaternion rotC = pc.head.rotation;
        Quaternion relativeRotation = rotA * (Quaternion.Inverse(rotB) * rotC);
        */
        return transformToMove * (Quaternion.Inverse(child) * position);
    }
    #endregion

    #region Audio


    public static void ThisFunctionIsNotCompletedYet_PlaySoundClipFromAnywhere(AudioClip clip, Vector3 position, float delay = 0)
    {
        GameObject audioSourceObject = Object.Instantiate(new GameObject(), position, Quaternion.identity);
        AudioSource source = audioSourceObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.PlayDelayed(delay);
    }

    #endregion

    #region Random
    public static bool RandomBool(float trueChance = 0.5f)
    {
        // Randomly returns true or false. trueChance is used for probability - 0 means always false, 1 means always true
        return Random.Range(0f, 1f) < trueChance;
    }

    /// <summary>
    /// Produces a random index value, that is always different from the previous one
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="previous"></param>
    /// <returns></returns>
    public static int NonRepeatedRandomIndex(int min, int max, int previous)
    {
        int index = Random.Range(min, max);
        if (index == previous)
        {
            index = InverseClamp(index + 1, min, max);
        }
        return index;
    }

    public static int WeightedIndex(int length, AnimationCurve weighting)
    {
        float random = weighting.Evaluate(Random.Range(0f, 1f));
        return Mathf.FloorToInt(random * length);
    }

    #endregion

    #region Even more miscellaneous

    public static float GetDecimalFromFloat(float f)
    {
        int wholeNumber = Mathf.RoundToInt(f);

        if (f < wholeNumber)
        {
            wholeNumber -= 1;
        }

        if (f > wholeNumber)
        {
            f -= wholeNumber;
        }

        return f;
    }

    public static float InverseCurveEvaluate(AnimationCurve curve, float t)
    {
        float curveMin = curve.keys[0].value;
        float curveMax = curve.keys[0].value;
        for (int i = 0; i < curve.keys.Length; i++)
        {
            curveMin = Mathf.Min(curveMin, curve.keys[i].value);
            curveMax = Mathf.Max(curveMax, curve.keys[i].value);
        }
        float range = curveMax - curveMin;

        return (1 - (curve.Evaluate(t) - curveMin) / range) * range + curveMin;
    }
    #endregion


    public static Mesh MeshFromNavMesh()
    {
        var navMesh = NavMesh.CalculateTriangulation();
        Vector3[] vertices = navMesh.vertices;
        int[] polygons = navMesh.indices;
        //navMesh.

        List<Vector3> verts = new List<Vector3>(vertices);

        Mesh mapMesh = new Mesh();
        mapMesh.SetVertices(verts);
        mapMesh.SetIndices(polygons, MeshTopology.Triangles, 0);
        mapMesh.RecalculateNormals();
        mapMesh.RecalculateTangents();
        mapMesh.RecalculateBounds();

        return mapMesh;
    }


    public static float DistanceBetweenDiagonals(Vector2 dimensions)
    {
        return Vector2.Distance(Vector2.zero, dimensions);
    }
    public static float DistanceBetweenDiagonals(float x, float y)
    {
        return DistanceBetweenDiagonals(new Vector2(x, y));
    }
    public static float DistanceBetweenDiagonals(float size)
    {
        return DistanceBetweenDiagonals(size, size);
    }

    public static void DrawMultipleDebugLines(Vector3[] positions, Color colour, bool looping = false)
    {
        for (int i = 1; i < positions.Length; i++)
        {
            Debug.DrawLine(positions[i - 1], positions[i], colour);
        }
        if (looping)
        {
            Debug.DrawLine(positions[positions.Length - 1], positions[0], colour);
        }
    }
    public static void DrawMultipleDebugLines(Vector3[] positions, Color colour, float duration, bool looping = false)
    {
        for (int i = 1; i < positions.Length; i++)
        {
            Debug.DrawLine(positions[i - 1], positions[i], colour, duration);
        }
        if (looping)
        {
            Debug.DrawLine(positions[positions.Length - 1], positions[0], colour);
        }
    }
    public static string DebugMultipleBools(bool[] bools, string name)
    {
        if (bools.Length < 1)
        {
            return "No bools to check";
        }
        
        string log = name + " values:\n";
        for (int i = 0; i < bools.Length; i++)
        {
            log += bools[i] +  "\n";
        }
        log += "End of log.";
        Debug.Log(log);
        return log;
    }

    public static void PauseForDebuggingPurposes()
    {
        UnityEditor.EditorApplication.isPaused = true;
    }

    public static void DrawDebugTextBox(string text, Vector3 position, Color colour, float duration)
    {

    }
}