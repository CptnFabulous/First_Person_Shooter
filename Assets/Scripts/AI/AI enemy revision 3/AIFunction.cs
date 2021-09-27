using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Unity.Collections;
using Unity.Jobs;
using System;

public enum SelfPreservation
{
    Skittish,
    Cautious,
    Aggressive,
    Suicidal
}

public static class AIFunction
{
    #region Miscellaneous

    // Checks if any part of a collider is within a certain distance of a point
    public static float ComplexColliderDistance(Collider c, Vector3 origin)
    {
        return Vector3.Distance(origin, c.bounds.ClosestPoint(origin));
    }

    // Checks if any part of a collider is within a certain angle
    public static float ComplexColliderAngle(Collider c, Vector3 origin, Vector3 compareDirection)
    {
        // Produces a position the same distance from the origin as the collider, but straight on
        float distanceFromOrigin = Vector3.Distance(origin, c.bounds.center);
        Vector3 centreOfConeAtDistanceEquivalentToCollider = origin + (compareDirection.normalized * distanceFromOrigin);

        // Figures out the part of the collider that is the closest to the centre of the cone's diameter
        Vector3 closestPoint = c.bounds.ClosestPoint(centreOfConeAtDistanceEquivalentToCollider);

        return Vector3.Angle(compareDirection, closestPoint - origin);
    }

    public static bool TwoAxisAngleCheck(Vector3 origin, Vector3 forward, Vector3 worldUp, Vector3 positionBeingChecked, Vector2 angles)
    {
        // Calculate position of positionBeingChecked relative to origin, and somehow rotate it based on worldUp to produce a position that would still be equally relative to the origin if the cone was upright

        Vector3 up = Vector3.up;


        Vector3 adjustedCheckPosition = positionBeingChecked;

        Vector3 xPositionCheck = new Vector3(adjustedCheckPosition.x, origin.y, adjustedCheckPosition.z);
        Vector3 yPositionCheck = new Vector3(origin.x, adjustedCheckPosition.y, adjustedCheckPosition.z);
        xPositionCheck = xPositionCheck - origin;
        yPositionCheck = yPositionCheck - origin;

        if (Vector3.Angle(Vector3.forward, xPositionCheck) < angles.x && Vector3.Angle(Vector3.forward, yPositionCheck) < angles.y)
        {
            return true;
        }







        return false;
    }

    #endregion

    #region Line of sight

    // Simply checks line of sight
    public static bool SimpleLineOfSightCheck(Vector3 lookingFor, Vector3 viewOrigin, LayerMask viewable)
    {
        RaycastHit rh;
        if (Physics.Raycast(viewOrigin, lookingFor - viewOrigin, out rh, Vector3.Distance(viewOrigin, lookingFor), viewable))
        {
            return false;
        }

        return true;
    }

    /*
    /// <summary>
    /// Checks line of sight, but allows a list of colliders that should be ignored. This overload allows directly inputting damage hitboxes rather than extracting their collider data.
    /// </summary>
    /// <param name="lookingFor"></param>
    /// <param name="viewOrigin"></param>
    /// <param name="viewable"></param>
    /// <param name="exceptions"></param>
    /// <returns></returns>
    public static bool LineOfSightCheckWithExceptions(Vector3 lookingFor, Vector3 viewOrigin, LayerMask viewable, DamageHitbox[] exceptions = null)
    {
        if (exceptions == null || exceptions.Length <= 0) // Returns a simpler and less performant check if there are no exceptions. This is for situations where there may or may not need to be exceptions
        {
            return SimpleLineOfSightCheck(lookingFor, viewOrigin, viewable);
        }

        //Debug.DrawRay(viewOrigin, lookingFor - viewOrigin, Color.yellow);


        RaycastHit[] hits = Physics.RaycastAll(viewOrigin, lookingFor - viewOrigin, Vector3.Distance(viewOrigin, lookingFor), viewable);
        foreach (RaycastHit rh in hits)
        {
            bool hitAnException = false;

            // Checks the collider in the RaycastHit against the list of exception colliders
            for (int i = 0; i < exceptions.Length; i++)
            {
                // If the collider in the raycast hit matches one of the exception colliders
                if (rh.collider == exceptions[i].Collider)
                {
                    // Dismiss this collider as an exception, end the exception check loop prematurely and move onto checking the next RaycastHit
                    hitAnException = true;
                    i = exceptions.Length;
                }
            }

            // If the collider did not match any of the exceptions and was not dismissed
            if (hitAnException == false)
            {
                // Line of sight has been broken by an actual obstacle, return false
                //Debug.Log("Returned false due to hitting " + rh.collider.name);
                return false;
            }
        }

        return true;
    }
    */
    //public static List<RaycastHit> hits = new List<RaycastHit>();
    public static bool LineOfSightCheckWithExceptions(Vector3 from, Vector3 to, LayerMask viewable, DamageHitbox[] fromColliders = null, DamageHitbox[] toColliders = null)
    {
        //hits.Clear();
        //hits.AddRange(Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), viewable));
        //Comparison<RaycastHit> distanceCheck = (one, two) => Vector3.Distance(from, one.point) < Vector3.Distance(from, two.point));
        //hits.Sort(distanceCheck);

        RaycastHit[] hits = Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), viewable);
        for (int i = 0; i < hits.Length; i++)
        {
            // If the raycast hit is one of the colliders belonging to the target object, return true
            for (int t = 0; t < toColliders.Length; t++)
            {
                if (hits[i].collider == toColliders[t].Collider)
                {
                    return true;
                }
            }

            // If the raycast hit a collider belonging to the object the raycast launched from, keep checking
            bool isException = false;
            for (int f = 0; f < fromColliders.Length; f++)
            {
                if (hits[i].collider == fromColliders[f].Collider)
                {
                    isException = true;
                    break;
                }
            }

            // If the raycast hit an object that isn't in either of the exception arrays, line of sight is blocked
            if (isException == true)
            {
                return false;
            }
        }

        // Nothing is present between the two points
        return true;
    }


    public static bool LineOfSightCheckForVisionCone(Collider c, Vector3 origin, Vector3 forward, Vector3 worldUp, float angle, out RaycastHit checkInfo, float range, LayerMask viewable, float raycastSpacing = 0.2f)
    {
        //float debugTime = 0.5f;
        
        #region Create raycast grid data
        // Finds the largest of the bounds' 3 size axes and produces a value that always exceeds that distance, regardless of the shape and angle.
        float maxBoundsSize = Mathf.Max(Mathf.Max(c.bounds.size.x, c.bounds.size.y), c.bounds.size.z) * 2;

        Vector3 originUp = Misc.PerpendicularUp(forward, worldUp);
        Vector3 originRight = Misc.PerpendicularRight(forward, worldUp);

        // Use Bounds.ClosestPoint four times, with points to the left, right, up and down of the bounding box (relative to the cone centre). Then use Vector3.Distance to calculate the distances and produce a rectangle of certain dimensions.
        Vector3 upPoint = c.bounds.center + originUp * maxBoundsSize;
        Vector3 downPoint = c.bounds.center + -originUp * maxBoundsSize;
        Vector3 leftPoint = c.bounds.center + -originRight * maxBoundsSize;
        Vector3 rightPoint = c.bounds.center + originRight * maxBoundsSize;
        upPoint = c.bounds.ClosestPoint(upPoint);
        downPoint = c.bounds.ClosestPoint(downPoint);
        leftPoint = c.bounds.ClosestPoint(leftPoint);
        rightPoint = c.bounds.ClosestPoint(rightPoint);

        // Produces dimensions for a rectangular area to sweep with raycasts
        float scanAreaY = Vector3.Distance(upPoint, downPoint);
        float scanAreaX = Vector3.Distance(leftPoint, rightPoint);

        // Figures out actual diameter of the area that needs to be covered
        float distanceToCollider = Vector3.Distance(origin, c.bounds.center);
        Vector3 l = origin + Misc.AngledDirection(new Vector3(0, -angle, 0), forward, worldUp).normalized * distanceToCollider;
        Vector3 r = origin + Misc.AngledDirection(new Vector3(0, angle, 0), forward, worldUp).normalized * distanceToCollider;
        /*
        #region Debugging cone
        Vector3 u = origin + Misc.AngledDirection(new Vector3(-angle, 0, 0), forward, worldUp).normalized * distanceToCollider;
        Vector3 d = origin + Misc.AngledDirection(new Vector3(angle, 0, 0), forward, worldUp).normalized * distanceToCollider;
        Vector3[] debugConePositions = new Vector3[]
        {
            origin, l, c.bounds.center, r, origin, u, c.bounds.center, d, origin
        };
        Misc.DrawMultipleDebugLines(debugConePositions, Colours.darkGreen, debugTime);
        #endregion
        */
        // Diameter of cone at required distance
        float coneDiameterAtDistance = Vector3.Distance(l, r);
        // Minimum spacing between raycasts so that there is always one inside the cone
        float maxSpacingWhileCoveringDiameter = (Vector2.one * coneDiameterAtDistance).x;
        // If the maximum allowed spacing is smaller than raycastSpacing, use that instead.
        // If the original value is smaller, keep it to have more precise scans for bigger objects
        raycastSpacing = Mathf.Min(maxSpacingWhileCoveringDiameter * 0.75f, raycastSpacing);
        // If the collider is close to the origin, area to cover can be smaller than default raycast spacing
        // Therefore it's possible for no raycasts to hit
        // The 1.2f variable is just for extra padding

        // Divide the rectangle dimensions by the sphereCastDiameter to obtain the amount of spherecasts necessary to cover the area.
        int raycastArrayLength = Mathf.CeilToInt(scanAreaX / raycastSpacing);
        int raycastArrayHeight = Mathf.CeilToInt(scanAreaY / raycastSpacing);

        // Creates variables to determine how far apart to space the raycasts on each axis
        float spacingX = scanAreaX / raycastArrayLength;
        float spacingY = scanAreaY / raycastArrayHeight;

        // Creates axes along which to align the raycasts
        Vector3 raycastGridAxisX = (rightPoint - c.bounds.center).normalized;
        Vector3 raycastGridAxisY = (upPoint - c.bounds.center).normalized;

        /*
        Vector3[] gridSquare = new Vector3[]
        {
            c.bounds.center + (raycastGridAxisX * (spacingX * 0 - scanAreaX / 2)) + (raycastGridAxisY * (spacingY * 0 - scanAreaY / 2)),
            c.bounds.center + (raycastGridAxisX * (spacingX * 0 - scanAreaX / 2)) + (raycastGridAxisY * (spacingY * 1 - scanAreaY / 2)),
            c.bounds.center + (raycastGridAxisX * (spacingX * 1 - scanAreaX / 2)) + (raycastGridAxisY * (spacingY * 1 - scanAreaY / 2)),
            c.bounds.center + (raycastGridAxisX * (spacingX * 1 - scanAreaX / 2)) + (raycastGridAxisY * (spacingY * 0 - scanAreaY / 2))
        };
        Misc.DrawMultipleDebugLines(gridSquare, Color.yellow, debugTime, true);
        */
        #endregion

        

        #region Perform raycast command batch processing
        // Creates a list of Vector3 directions
        List<Vector3> directions = new List<Vector3>();
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
                Vector3 raycastAimDirection = raycastAimPoint - origin;

                // Checks if the raycast is still detecting a point that is actually inside the cone
                if (Vector3.Angle(forward, raycastAimDirection) < angle)
                {
                    //Debug.DrawLine(origin, raycastAimPoint, Color.white, debugTime);

                    directions.Add(raycastAimDirection);
                }
                else
                {
                    //Debug.DrawLine(origin, raycastAimPoint, Colours.scarlet, debugTime);
                }
            }
        }

        // Create RaycastCommand
        var results = new NativeArray<RaycastHit>(directions.Count, Allocator.TempJob);
        var commands = new NativeArray<RaycastCommand>(directions.Count, Allocator.TempJob);

        // Assign directions to each RaycastCommand
        for (int i = 0; i < commands.Length; i++)
        {
            commands[i] = new RaycastCommand(origin, directions[i], range, viewable);
        }

        // Schedule the batch of raycasts, and wait for the batch processing job to complete
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1);
        handle.Complete();

        // Converts the NativeArray of results to a regular array
        RaycastHit[] hits = results.ToArray();
        //Debug.Log("Results length = " + hits.Length);

        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
        #endregion

        #region Evaluate results and determine true or false

        // Look through each result in the list of raycast hits
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.DrawLine(origin, hits[i].point, Colours.turquoise, 10);
            // If a RaycastHit is found that matches the collider being checked for, store it and return true
            if (hits[i].collider == c)
            {
                checkInfo = hits[i];
                return true;
            }
        }

        // In the event that no results were found, tokenly assign a redundant RaycastHit so the function completes properly
        if (Physics.Raycast(origin, forward, out checkInfo, range, viewable))
        {

        }

        return false;
        #endregion
    }




    /*
    // Uses raycast grids and bound checks to scan for partially concealed colliders
    public static bool LineOfSightCheckForVisionCone(Collider c, Vector3 origin, Vector3 forward, Vector3 worldUp, float angle, out RaycastHit checkInfo, float range, LayerMask viewable, float raycastSpacing = 0.2f)
    {
        #region Create raycast grid data
        // Finds the largest of the bounds' 3 size axes and produces a value that always exceeds that distance, regardless of the shape and angle.
        float maxBoundsSize = Mathf.Max(Mathf.Max(c.bounds.size.x, c.bounds.size.y), c.bounds.size.z) * 2;

        Vector3 originUp = Misc.PerpendicularUp(forward, worldUp);
        Vector3 originRight = Misc.PerpendicularRight(forward, worldUp);

        // Use Bounds.ClosestPoint four times, with points to the left, right, up and down of the bounding box (relative to the cone centre). Then use Vector3.Distance to calculate the distances and produce a rectangle of certain dimensions.
        Vector3 upPoint = c.bounds.center + originUp * maxBoundsSize;
        Vector3 downPoint = c.bounds.center + -originUp * maxBoundsSize;
        Vector3 leftPoint = c.bounds.center + -originRight * maxBoundsSize;
        Vector3 rightPoint = c.bounds.center + originRight * maxBoundsSize;
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

        

        #region Check that the gap between raycasts is not too small for the attack zone itself
        float distance = Vector3.Distance(origin, c.bounds.center);
        Vector3 l = Misc.AngledDirection(new Vector3(0, -angle, 0), forward, worldUp);
        Vector3 r = Misc.AngledDirection(new Vector3(0, angle, 0), forward, worldUp);
        l = origin + l.normalized * distance;
        r = origin + r.normalized * distance;
        // Obtains the actual size of the area that needs to be covered
        float maxDistanceToCover = Vector3.Distance(l, r);

        Debug.Log("Spacing = " + raycastSpacing + ", adjusted = " + spacingX + ", " + spacingY + ", cover = " + maxDistanceToCover);


        float diagonalSpaceBetweenRaycasts = Vector2.Distance(Vector2.zero, new Vector2(spacingX, spacingY));
        if (diagonalSpaceBetweenRaycasts > maxDistanceToCover) // If the gap between spaces is bigger than the distance
        {
            // This means it's possible for none of the raycasts to land inside the actual hit zone, and are all excluded.
            // Reverse the code used to get maxGapBetweenRaycasts from spacingX and spacingY, replacing with maxDistanceToCover
            // This should produce an appropriate grid spacing small enough to actually cover the cone of fire.
            Vector2 correctedDiagonal = new Vector2(spacingX, spacingY).normalized * maxDistanceToCover;
            spacingX = correctedDiagonal.x;
            spacingY = correctedDiagonal.y;
        }

        Debug.Log("Spacing = " + raycastSpacing + ", adjusted = " + spacingX + ", " + spacingY + ", cover = " + maxDistanceToCover);


        Vector3 u = Misc.AngledDirection(new Vector3(-angle, 0, 0), forward, worldUp);
        Vector3 d = Misc.AngledDirection(new Vector3(angle, 0, 0), forward, worldUp);
        u = origin + u.normalized * distance;
        d = origin + d.normalized * distance;
        Debug.DrawLine(origin, l, Colours.darkGreen, 10);
        Debug.DrawLine(origin, r, Colours.darkGreen, 10);
        Debug.DrawLine(origin, u, Colours.darkGreen, 10);
        Debug.DrawLine(origin, d, Colours.darkGreen, 10);
        Debug.DrawLine(l, c.bounds.center, Colours.darkGreen, 10);
        Debug.DrawLine(r, c.bounds.center, Colours.darkGreen, 10);
        Debug.DrawLine(u, c.bounds.center, Colours.darkGreen, 10);
        Debug.DrawLine(d, c.bounds.center, Colours.darkGreen, 10);

        #endregion





        // Creates axes along which to align the raycasts
        Vector3 raycastGridAxisX = (rightPoint - c.bounds.center).normalized;
        Vector3 raycastGridAxisY = (upPoint - c.bounds.center).normalized;
        #endregion

        #region Perform raycast command batch processing
        // Creates a list of Vector3 directions
        List<Vector3> directions = new List<Vector3>();


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
                Vector3 raycastAimDirection = raycastAimPoint - origin;

                // Checks if the raycast is still detecting a point that is actually inside the cone
                if (Vector3.Angle(forward, raycastAimDirection) < angle)
                {
                    Debug.DrawLine(origin, raycastAimPoint, Color.white, 10);

                    directions.Add(raycastAimDirection);
                }
                else
                {
                    Debug.DrawLine(origin, raycastAimPoint, Colours.scarlet, 10);
                }
            }
        }

        Debug.Log("Direction count = " + directions.Count);

        // Create RaycastCommand
        var results = new NativeArray<RaycastHit>(directions.Count, Allocator.TempJob);
        var commands = new NativeArray<RaycastCommand>(directions.Count, Allocator.TempJob);

        // Assign directions to each RaycastCommand
        for (int i = 0; i < commands.Length; i++)
        {
            commands[i] = new RaycastCommand(origin, directions[i], range, viewable);
        }

        // Schedule the batch of raycasts, and wait for the batch processing job to complete
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1);
        handle.Complete();

        // Converts the NativeArray of results to a regular array
        RaycastHit[] hits = results.ToArray();
        Debug.Log("Results length = " + hits.Length);

        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
        #endregion

        #region Evaluate results and determine true or false

        // Look through each result in the list of raycast hits
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.DrawLine(origin, hits[i].point, Colours.turquoise, 10);
            // If a RaycastHit is found that matches the collider being checked for, store it and return true
            if (hits[i].collider == c)
            {
                checkInfo = hits[i];
                return true;
            }
        }

        // In the event that no results were found, tokenly assign a redundant RaycastHit so the function completes properly
        if (Physics.Raycast(origin, forward, out checkInfo, range, viewable))
        {

        }

        return false;
        #endregion
    }
    */

    #region Deprecated but replacing them with the newer ones made things stop working and I'm not sure why
    
    public static bool LineOfSight(Vector3 origin, Transform target, LayerMask coverCriteria, float overlap = 0.01f)
    {
        // Launches a raycast between the cover position and the attacker
        RaycastHit lineOfSightCheck;
        if (Physics.Raycast(origin, target.position - origin, out lineOfSightCheck, Vector3.Distance(origin, target.position) + overlap, coverCriteria))
        {
            Transform t = lineOfSightCheck.collider.transform; // Gets transform of object


            DamageHitbox dh = t.GetComponent<DamageHitbox>(); // If object is a DamageHitbox, find the root object, which is the actual thing being tracked if it's an enemy
            if (dh != null)
            {
                if (dh.healthScript != null)
                {
                    if (dh.healthScript.transform == target)
                    {
                        return true;
                    }
                }   
            }

            if (t == target)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #endregion

    #region Vision cones

    /*
    public static void VisionCone(Vector3 origin, float range, LayerMask hitDetection, Type[] componentsToLookFor)
    {
        Collider[] objects = Physics.OverlapSphere(origin, range, hitDetection);
        foreach(Collider c in objects)
        {
            bool hasComponentWorthLookingFor = false;
            for (int tci = 0; tci < componentsToLookFor.Length; tci++)
            {
                Component desired = c.GetComponent(componentsToLookFor[tci]);
                if (desired != null)
                {
                    hasComponentWorthLookingFor = true;
                    tci = componentsToLookFor.Length;
                }
            }

            // If the collider contains a component indicating it's worth checking
            // If the angle of that point is inside the cone, perform a raycast check
            if (hasComponentWorthLookingFor && ComplexColliderAngle(c, origin, forward) < angle)
            {
                // Should I have a first, simpler check to improve performance?

                // Perform a more complex line of sight check
                RaycastHit lineOfSightCheck;
                if (LineOfSightCheckForVisionCone(c, origin, forward, worldUp, angle, out lineOfSightCheck, range, viewable, raycastSpacing))
                {
                    hits.Add(lineOfSightCheck); // Records hit
                }
            }
        }
    }
    */

    // A full featured vision cone that detects and stores everything inside certain parameters
    public static RaycastHit[] VisionCone(Vector3 origin, Vector3 forward, Vector3 worldUp, float angle, float range, LayerMask checkingFor, LayerMask viewable, float raycastSpacing = 0.2f)
    {
        // Assembles an empty list, to slowly be added to
        List<RaycastHit> hits = new List<RaycastHit>();

        // Obtains a list of objects within the layers specified. The separate layer mask is important to improve performance, so the game does not check for objects it does not need.
        Collider[] objects = Physics.OverlapSphere(origin, range, checkingFor);
        foreach (Collider c in objects)
        {
            // If the angle of that point is inside the cone, perform a raycast check
            if (ComplexColliderAngle(c, origin, forward) < angle)
            {
                // Should I have a first, simpler check to improve performance?
                
                // Perform a more complex line of sight check
                RaycastHit lineOfSightCheck;
                if (LineOfSightCheckForVisionCone(c, origin, forward, worldUp, angle, out lineOfSightCheck, range, viewable, raycastSpacing))
                {
                    hits.Add(lineOfSightCheck); // Records hit
                }
            }
        }

        return hits.ToArray();
    }

    /*
    // Checks if a specific set of objects is inside a vision cone
    public static bool VisionConeColliderCheck(Collider[] colliderSet, Vector3 origin, Vector3 forward, Vector3 worldUp, float angle, float range, LayerMask viewable, float raycastSpacing = 0.2f)
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        
        foreach (Collider c in colliderSet)
        {
            if (Vector3.Distance(c.bounds.ClosestPoint(origin), origin) < range) // If the collider is within range
            {
                if (ComplexColliderAngle(c, origin, forward) < angle) // If the angle of that point is inside the cone, perform a raycast check
                {
                    RaycastHit lineOfSightCheck;
                    if (LineOfSightCheckForVisionCone(c, origin, forward, worldUp, angle, out lineOfSightCheck, range, viewable, raycastSpacing))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Checks if a specific position is inside a vision cone
    public static bool VisionConePositionCheck(Vector3 positionChecked, Vector3 origin, Vector3 forward, float angle, float range, LayerMask viewable, Collider[] exceptions = null)
    {
        if (Vector3.Distance(origin, positionChecked) < range)
        {
            if (Vector3.Angle(forward, positionChecked - origin) < angle)
            {
                if (LineOfSightCheckWithExceptions(positionChecked, origin, viewable, exceptions))
                {
                    return true;
                }
            }
        }

        return false;
    }
    */

    /*
    public static RaycastHit[] VisionConeOld(Transform origin, float angle, float range, LayerMask viewable, float raycastSpacing = 0.2f)
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
    */

    /*
    public static List<Collider> VisionConeSimpleTwoAngles(Vector3 origin, Vector3 direction, Vector2 angles, float range, LayerMask viewable)
    {
        List<Collider> objectsInView = new List<Collider>();
        Collider[] objects = Physics.OverlapSphere(origin, range); // Checks for all objects in range
        foreach (Collider c in objects)
        {
            // Obtains the horizontal and vertical relative position data for the raycast hit point relative to the line of sight's origin.
            Vector3 relativePosition_X = new Vector3(c.transform.position.x, origin.y, c.transform.position.z) - origin;
            Vector3 relativePosition_Y = new Vector3(origin.x, c.transform.position.y, c.transform.position.z) - origin;
            Vector2 visionAngle = new Vector2(Vector3.Angle(relativePosition_X, direction), Vector3.Angle(relativePosition_Y, direction));
            if (visionAngle.x < angles.x && visionAngle.y < angles.y)
            {
                if (AI.LineOfSight(origin, c.transform, viewable))
                {
                    objectsInView.Add(c); // Add c.gameObject to viewedObjects array
                }
            }
        }

        return objectsInView; // Returns list of objects the player is looking at
    }
    */
    #endregion

    #region Navmesh related
    public static float NavMeshPathLength(NavMeshPath path)
    {
        // Calculate path length
        float pathLength = 0;
        for (int r = 1; r < path.corners.Length; r++)
        {
            // Gets the length between the current node and the previous one, and adds it to the final length;
            pathLength += Vector3.Distance(path.corners[r - 1], path.corners[r]);
            //Debug.DrawLine(path.corners[r - 1], path.corners[r], Color.Lerp(Color.red, Color.green, (float)(r - 1) / (path.corners.Length - 1)), 5);
        }
        return pathLength;
    }

    public static Vector3[] PositionsAroundPointInBox(Vector3 origin, Quaternion rotation, float minRadius, float maxRadius, float raycastSpacing)
    {
        // Calculate points in a grid around the player
        
        /*
        for (float x = -maxRadius; x < maxRadius; x += raycastSpacing)
        {
            for (float z = -maxRadius; z < maxRadius; z += raycastSpacing)
            {
                Vector3 position = new Vector3(x, 0, z);
                if ()
            }
        }




        Bounds extents = new Bounds(origin, new Vector3(maxRadius, maxRadius, maxRadius));
        
        for (float x = extents.min.x; x < extents.max.x; x += raycastSpacing)
        {
            for (float z = extents.min.z; z < extents.max.z; z += raycastSpacing)
            {
                
                
                
                
                // Creates positions to launch a raycast down onto the navmesh
                Vector3 rayOrigin = rotation * new Vector3(x, extents.max.y, z);
                rayOrigin += extents.center;
                Vector3 rayDirection = rotation * Vector3.down;

                // Find positions on the NavMesh, and for each one 
                //Physics.Ov

                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, meshBounds.extents.y * 2, terrainDetection);



            }
        }
        */


        return null;
    }

    /// <summary>
    /// Creates a ring of positions around the origin, in a spiral pattern
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="axis"></param>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    /// <param name="loops"></param>
    /// <param name="numberOfChecks"></param>
    /// <returns></returns>
    public static Vector3[] PositionsAroundPointInSpiral(Vector3 origin, Vector3 axis, float minRadius, float maxRadius, float loops, int numberOfChecks)
    {
        Vector3[] positions = new Vector3[numberOfChecks];
        for (int i = 0; i < numberOfChecks; i++)
        {
            float percentage = (float)i / (numberOfChecks - 1);
            float distance = Mathf.Lerp(minRadius, maxRadius, percentage);
            float angle = Mathf.Lerp(0, 360 * loops, percentage);
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            positions[i] = origin + (rotation * Vector3.forward * (minRadius + distance));

            //Color c = Color.Lerp(Color.green, Color.red, percentage);
            //Debug.DrawLine(origin, positions[i], Color.cyan, 10);
        }

        return positions;
    }


    #endregion
}