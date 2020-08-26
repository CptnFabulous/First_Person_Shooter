﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Jobs;

public static class AIFunction
{
    public static RaycastHit[] VisionCone(Transform origin, float angle, float range, LayerMask viewable, float raycastSpacing = 0.2f)
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        Collider[] objects = Physics.OverlapSphere(origin.position, range, viewable);
        foreach (Collider c in objects)
        {
            // Produces a position the same distance from the origin as the collider, but straight on
            float distanceFromOrigin = Vector3.Distance(origin.position, c.bounds.center);
            Vector3 centreOfConeAtDistanceEquivalentToCollider = origin.position + (origin.forward.normalized * distanceFromOrigin);

            // Figures out the part of the collider that is the closest to the centre of the cone's diameter
            Vector3 closestPoint = c.bounds.ClosestPoint(centreOfConeAtDistanceEquivalentToCollider);

            if (Vector3.Angle(origin.forward, closestPoint - origin.position) < angle) // If the angle of that point is inside the cone, perform a raycast check
            {
                // Finds the largest of the bounds' 3 size axes and produces a value that always exceeds that distance, regardless of the shape and angle.
                float maxBoundsSize = Mathf.Max(c.bounds.size.x, c.bounds.size.y);
                maxBoundsSize = Mathf.Max(maxBoundsSize, c.bounds.size.z) * 2;

                // Use Bounds.ClosestPoint four times, with points to the left, right, up and down of the bounding box (relative to the cone centre). Then use Vector3.Distance to calculate the distances and produce a rectangle of certain dimensions.
                Vector3 upPoint = c.bounds.center + origin.up * maxBoundsSize;
                Vector3 downPoint = c.bounds.center + -origin.up * maxBoundsSize;
                Vector3 leftPoint = c.bounds.center + -origin.right * maxBoundsSize;
                Vector3 rightPoint = c.bounds.center + origin.right * maxBoundsSize;
                upPoint = c.bounds.ClosestPoint(upPoint);
                downPoint = c.bounds.ClosestPoint(downPoint);
                leftPoint = c.bounds.ClosestPoint(leftPoint);
                rightPoint = c.bounds.ClosestPoint(rightPoint);

                // Produces dimensions for a rectangular area to sweep with raycasts
                float scanAreaY = Vector3.Distance(upPoint, downPoint);
                float scanAreaX = Vector3.Distance(leftPoint, rightPoint);

                // Divide the rectangle dimensions by the sphereCastDiameter to obtain the amount of spherecasts necessary to cover the area.
                int raycastArrayLength = Mathf.CeilToInt(scanAreaX / raycastSpacing);
                int raycastArrayHeight = Mathf.CeilToInt(scanAreaY / raycastSpacing);

                // Creates variables to determine how far apart to space the raycasts on each axis
                float spacingX = scanAreaX / raycastArrayLength;
                float spacingY = scanAreaY / raycastArrayHeight;

                // Creates axes along which to align the raycasts
                Vector3 raycastGridAxisX = (rightPoint - c.bounds.center).normalized;
                Vector3 raycastGridAxisY = (upPoint - c.bounds.center).normalized;

                // Cast an array of rays to 'sweep' the square for line of sight.
                for (int y = 0; y < raycastArrayHeight; y++)
                {
                    for (int x = 0; x < raycastArrayLength; x++)
                    {
                        // Creates coordinates along the sweep area for the raycast. 0,0 would be the centre, so for each axis I am taking away a value equivalent to half of that axis dimension, so I can use the bottom-left corner as 0,0
                        float distanceX = spacingX * x - scanAreaX / 2;
                        float distanceY = spacingY * y - scanAreaY / 2;

                        // Starts with c.bounds.centre, then adds direction values multiplied by the appropriate distance value for each axis, to create the point that you want the raycast to hit.
                        Vector3 raycastAimPoint = c.bounds.center + (raycastGridAxisX * distanceX) + (raycastGridAxisY * distanceY);

                        // From that point, it creates a direction value for the raycast to aim in.
                        Vector3 raycastAimDirection = raycastAimPoint - origin.position;

                        // Launches raycast
                        if (Vector3.Angle(raycastAimDirection, origin.forward) < angle) // If the raycast direction is still inside the cone
                        {
                            // It's actually a BoxCast, so the raycasts tesselate and there are no blind spots.
                            RaycastHit lineOfSightCheck;
                            if (Physics.BoxCast(origin.position, new Vector3(raycastSpacing / 2, raycastSpacing / 2, raycastSpacing / 2), raycastAimDirection, out lineOfSightCheck, Quaternion.identity, range, viewable))
                            {
                                if (lineOfSightCheck.collider == c && (hits.Contains(lineOfSightCheck) == false))
                                {
                                    hits.Add(lineOfSightCheck);

                                    // Ends for loops prematurely, ensuring no more unnecessary rays are cast if an object has already been found.
                                    x = raycastArrayLength + 1;
                                    y = raycastArrayHeight + 1;
                                    //This is important for performance, otherwise this function will be very laggy.
                                    //Even then, do not run this every frame. I would probably advise running this every second or so on a timer.
                                }
                            }
                        }
                    }
                }
            }
        }

        return hits.ToArray();
    }




    public static float ComplexColliderAngle(Collider c, Vector3 origin, Vector3 compareDirection)
    {
        // Produces a position the same distance from the origin as the collider, but straight on
        float distanceFromOrigin = Vector3.Distance(origin, c.bounds.center);
        Vector3 centreOfConeAtDistanceEquivalentToCollider = origin + (compareDirection.normalized * distanceFromOrigin);

        // Figures out the part of the collider that is the closest to the centre of the cone's diameter
        Vector3 closestPoint = c.bounds.ClosestPoint(centreOfConeAtDistanceEquivalentToCollider);

        return Vector3.Angle(compareDirection, closestPoint - origin);
    }


    
    public static bool ComplexLineOfSightCheck(Collider c, Transform origin, float angle, out RaycastHit objectHit, float range, LayerMask viewable, float raycastSpacing = 0.2f)
    {
        
        
        
        #region Create raycast grid data
        // Finds the largest of the bounds' 3 size axes and produces a value that always exceeds that distance, regardless of the shape and angle.
        float maxBoundsSize = Mathf.Max(Mathf.Max(c.bounds.size.x, c.bounds.size.y), c.bounds.size.z) * 2;

        // Use Bounds.ClosestPoint four times, with points to the left, right, up and down of the bounding box (relative to the cone centre). Then use Vector3.Distance to calculate the distances and produce a rectangle of certain dimensions.
        Vector3 upPoint = c.bounds.center + origin.up * maxBoundsSize;
        Vector3 downPoint = c.bounds.center + -origin.up * maxBoundsSize;
        Vector3 leftPoint = c.bounds.center + -origin.right * maxBoundsSize;
        Vector3 rightPoint = c.bounds.center + origin.right * maxBoundsSize;
        upPoint = c.bounds.ClosestPoint(upPoint);
        downPoint = c.bounds.ClosestPoint(downPoint);
        leftPoint = c.bounds.ClosestPoint(leftPoint);
        rightPoint = c.bounds.ClosestPoint(rightPoint);

        // Produces dimensions for a rectangular area to sweep with raycasts
        float scanAreaY = Vector3.Distance(upPoint, downPoint);
        float scanAreaX = Vector3.Distance(leftPoint, rightPoint);

        // Divide the rectangle dimensions by the sphereCastDiameter to obtain the amount of spherecasts necessary to cover the area.
        int raycastArrayLength = Mathf.CeilToInt(scanAreaX / raycastSpacing);
        int raycastArrayHeight = Mathf.CeilToInt(scanAreaY / raycastSpacing);

        // Creates variables to determine how far apart to space the raycasts on each axis
        float spacingX = scanAreaX / raycastArrayLength;
        float spacingY = scanAreaY / raycastArrayHeight;

        // Creates axes along which to align the raycasts
        Vector3 raycastGridAxisX = (rightPoint - c.bounds.center).normalized;
        Vector3 raycastGridAxisY = (upPoint - c.bounds.center).normalized;
        #endregion

        int totalRaycasts = raycastArrayLength * raycastArrayHeight;
        // Create RaycastCommand
        var results = new NativeArray<RaycastHit>(totalRaycasts, Allocator.Temp);
        var commands = new NativeArray<RaycastCommand>(totalRaycasts, Allocator.Temp);

        // Cast an array of rays to 'sweep' the square for line of sight.
        for (int y = 0; y < raycastArrayHeight; y++)
        {
            for (int x = 0; x < raycastArrayLength; x++)
            {
                // Creates coordinates along the sweep area for the raycast. 0,0 would be the centre, so for each axis I am taking away a value equivalent to half of that axis dimension, so I can use the bottom-left corner as 0,0
                float distanceX = spacingX * x - scanAreaX / 2;
                float distanceY = spacingY * y - scanAreaY / 2;

                // Starts with c.bounds.centre, then adds direction values multiplied by the appropriate distance value for each axis, to create the point that you want the raycast to hit.
                Vector3 raycastAimPoint = c.bounds.center + (raycastGridAxisX * distanceX) + (raycastGridAxisY * distanceY);

                // From that point, it creates a direction value for the raycast to aim in.
                Vector3 raycastAimDirection = raycastAimPoint - origin.position;

                int raycastNumber = (raycastArrayLength * y) + x;

                commands[raycastNumber] = new RaycastCommand(origin.position, raycastAimDirection, range, viewable);
            }
        }

        // Schedule the batch of raycasts
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));
        // Wait for the batch processing job to complete
        handle.Complete();

        

        for(int i = 0; i < results.Length; i++)
        {
            // Copy the result. If batchedHit.collider is null there was no hit
            RaycastHit batchedHit = results[i];

            if (batchedHit.collider == c)
            {
                objectHit = batchedHit;

                // Dispose the buffers
                results.Dispose();
                commands.Dispose();
                return true;
            }
            
        }

        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
        return false;
    }







    public static RaycastHit[] VisionConeBetterOptimised(Transform origin, float angle, float range, LayerMask checkingFor, LayerMask viewable, float raycastSpacing = 0.2f)
    {
        List<RaycastHit> hits = new List<RaycastHit>();

        Collider[] objects = Physics.OverlapSphere(origin.position, range, checkingFor);
        foreach (Collider c in objects)
        {
            if (ComplexColliderAngle(c, origin.position, origin.forward) < angle) // If the angle of that point is inside the cone, perform a raycast check
            {
                RaycastHit lineOfSightCheck;
                if (Physics.Raycast(origin.position, c.transform.position - origin.position, out lineOfSightCheck, range, viewable)) // Launch a raycast to determine line of sight.
                {
                    if (lineOfSightCheck.collider == c && (hits.Contains(lineOfSightCheck) == false))
                    {
                        hits.Add(lineOfSightCheck);
                    }
                }
                else // If a simple check towards the centre of the transform doesn't work, perform a more thorough and intensive check. The simple check is done first to reduce performance.
                {
                    // Finds the largest of the bounds' 3 size axes and produces a value that always exceeds that distance, regardless of the shape and angle.
                    float maxBoundsSize = Mathf.Max(Mathf.Max(c.bounds.size.x, c.bounds.size.y), c.bounds.size.z) * 2;

                    // Use Bounds.ClosestPoint four times, with points to the left, right, up and down of the bounding box (relative to the cone centre). Then use Vector3.Distance to calculate the distances and produce a rectangle of certain dimensions.
                    Vector3 upPoint = c.bounds.center + origin.up * maxBoundsSize;
                    Vector3 downPoint = c.bounds.center + -origin.up * maxBoundsSize;
                    Vector3 leftPoint = c.bounds.center + -origin.right * maxBoundsSize;
                    Vector3 rightPoint = c.bounds.center + origin.right * maxBoundsSize;
                    upPoint = c.bounds.ClosestPoint(upPoint);
                    downPoint = c.bounds.ClosestPoint(downPoint);
                    leftPoint = c.bounds.ClosestPoint(leftPoint);
                    rightPoint = c.bounds.ClosestPoint(rightPoint);

                    // Produces dimensions for a rectangular area to sweep with raycasts
                    float scanAreaY = Vector3.Distance(upPoint, downPoint);
                    float scanAreaX = Vector3.Distance(leftPoint, rightPoint);

                    // Divide the rectangle dimensions by the sphereCastDiameter to obtain the amount of spherecasts necessary to cover the area.
                    int raycastArrayLength = Mathf.CeilToInt(scanAreaX / raycastSpacing);
                    int raycastArrayHeight = Mathf.CeilToInt(scanAreaY / raycastSpacing);

                    // Creates variables to determine how far apart to space the raycasts on each axis
                    float spacingX = scanAreaX / raycastArrayLength;
                    float spacingY = scanAreaY / raycastArrayHeight;

                    // Creates axes along which to align the raycasts
                    Vector3 raycastGridAxisX = (rightPoint - c.bounds.center).normalized;
                    Vector3 raycastGridAxisY = (upPoint - c.bounds.center).normalized;

                    // Cast an array of rays to 'sweep' the square for line of sight.
                    for (int y = 0; y < raycastArrayHeight; y++)
                    {
                        for (int x = 0; x < raycastArrayLength; x++)
                        {
                            // Creates coordinates along the sweep area for the raycast. 0,0 would be the centre, so for each axis I am taking away a value equivalent to half of that axis dimension, so I can use the bottom-left corner as 0,0
                            float distanceX = spacingX * x - scanAreaX / 2;
                            float distanceY = spacingY * y - scanAreaY / 2;

                            // Starts with c.bounds.centre, then adds direction values multiplied by the appropriate distance value for each axis, to create the point that you want the raycast to hit.
                            Vector3 raycastAimPoint = c.bounds.center + (raycastGridAxisX * distanceX) + (raycastGridAxisY * distanceY);

                            // From that point, it creates a direction value for the raycast to aim in.
                            Vector3 raycastAimDirection = raycastAimPoint - origin.position;

                            // Launches raycast
                            if (Vector3.Angle(raycastAimDirection, origin.forward) < angle) // If the raycast direction is still inside the cone
                            {
                                // It's actually a BoxCast, so the raycasts tesselate and there are no blind spots.
                                if (Physics.Raycast(origin.position, raycastAimDirection, out lineOfSightCheck, range, viewable))
                                {
                                    if (lineOfSightCheck.collider == c && (hits.Contains(lineOfSightCheck) == false))
                                    {
                                        hits.Add(lineOfSightCheck); // Records hit

                                        // Ends for loops prematurely, ensuring no more unnecessary rays are cast if an object has already been found.
                                        x = raycastArrayLength + 1;
                                        y = raycastArrayHeight + 1;
                                        //This is important for performance, otherwise this function will be very laggy.
                                        //Even then, do not run this every frame. I would probably advise running this every second or so on a timer.
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return hits.ToArray();
    }

    










}