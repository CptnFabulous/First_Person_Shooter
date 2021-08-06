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

    // Simply checks line of sight, but allows a list of colliders that should be ignored
    public static bool LineOfSightCheckWithExceptions(Vector3 lookingFor, Vector3 viewOrigin, LayerMask viewable, Collider[] exceptions = null)
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
                if (rh.collider == exceptions[i])
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

        /*
        if (exceptions == null || exceptions.Length <= 0) // Returns a simpler and less performant check if there are no exceptions. This is for situations where there may or may not need to be exceptions
        {
            return SimpleLineOfSightCheck(lookingFor, viewOrigin, viewable);
        }

        RaycastHit[] hits = Physics.RaycastAll(viewOrigin, lookingFor - viewOrigin, Vector3.Distance(viewOrigin, lookingFor), viewable);
        foreach (RaycastHit rh in hits)
        {
            foreach (Collider c in exceptions)
            {
                if (rh.collider != c)
                {
                    return false;
                }
            }
        }

        return true;
        */
    }

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
                    directions.Add(raycastAimDirection);
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

        // Dispose the buffers
        results.Dispose();
        commands.Dispose();
        #endregion

        #region Evaluate results and determine true or false

        // Look through each result in the list of raycast hits
        for (int i = 0; i < hits.Length; i++)
        {
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
            //Debug.DrawLine(path.corners[r - 1], path.corners[r], Color.Lerp(Color.red, Color.green, 1 * ((r - 1) / (path.corners.Length - 1))), 5);
            Debug.DrawLine(path.corners[r - 1], path.corners[r], Color.Lerp(Color.red, Color.green, (float)(r - 1) / (path.corners.Length - 1)), 5);

        }

        return pathLength;
    }

    #endregion






}