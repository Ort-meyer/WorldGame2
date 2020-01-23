using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static float GetDiffAngle2D(Vector3 forward, Vector3 vectorToTarget)
    {
        Vector2 currentDirection = new Vector2(forward.x, forward.z);
        Vector2 targetDirection = new Vector2(vectorToTarget.x, vectorToTarget.z); 

        float diffAngle = Vector2.Angle(currentDirection, targetDirection);

        // For some reason, this angle is absolute. Do some algebra magic to get negative angle
        Vector3 cross = Vector3.Cross(forward, vectorToTarget);
        if (cross.y < 0)
        {
            diffAngle *= -1;
        }
        return diffAngle;
    }
    public static float Sign(float number)
    {
        if (number == 0)
        {
            return 0;
        }
        else
        {
            return Mathf.Sign(number);
        }
    }

    // Limits the number to absLimit, with sign (i.e. -40 limited to 30 returns -30)
    public static float LimitWithSign(float number, float absLimit)
    {
        if (Mathf.Abs(number) > absLimit)
        {
            number = Mathf.Sign(number) * absLimit;
        }
        return number;
    }

    // Taken from https://www.gamedev.net/forums/topic/107074-calculating-projectile-launch-angle-to-hit-a-target/?page=3
    // Returns angle necessary to hit the target with the given parameters
    public static float GetAngleToHit(float distanceToTarget, float heightDifference, float speed)
    {
        // Broken up for debugging purposes. Keeping it around for readability
        float u = speed;
        float us = Mathf.Pow(u, 2);
        float x = distanceToTarget;
        float xs = Mathf.Pow(x, 2);
        float y = heightDifference;
        float g = 9.81f;

        float xsus = xs / us;
        float gxsus = g * xsus;
        float part0 = y + 0.5f * gxsus;
        float part0point5 = us - 2 * g * part0;
        float part1 = u - Mathf.Sqrt(part0point5);
        float part2 = g * (x / u);
        float angle = Mathf.Atan(part1 / part2);

        return angle;
    }

    // Returns the closes GameObject from a list, to a given GameObject 
    public static GameObject GetClosestObject(GameObject fromObj, List<GameObject> toObjects)
    {
        float currentDistance = 100000;
        GameObject closestObj = null;
        foreach(GameObject obj in toObjects)
        {
            float thisDistance = (fromObj.transform.position - obj.transform.position).magnitude;
            if(thisDistance < currentDistance)
            {
                currentDistance = thisDistance;
                closestObj = obj;
            }
        }
        return closestObj;
    }

    // Rotates a point around a pivot. This feels unnecessary. Remote?
    public static Vector3 RotatePointAroundPivot(Quaternion rotation, Vector3 point, Vector3 pivot)
    {
        return rotation * (point - pivot) + pivot;
    }
}
