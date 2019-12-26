using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static float GetDiffAngle2D(Vector3 forward, Vector3 vectorToTarget)
    {
        //// Check for paralell vectors
        //if(IsParallel(forward, vectorToTarget))
        //{
        //    return 0;
        //}

        //Vector2 v1 = new Vector2(0, 1);
        //Vector2 v2 = new Vector2(0, 1);
        //Vector2 v3 = v1;
        //float diff1 = Vector2.Angle(v1, v2);
        //float diff2 = Vector2.Angle(v1, v3);

        Vector2 currentDirection = new Vector2(forward.x, forward.z);
        Vector2 targetDirection = new Vector2(vectorToTarget.x, vectorToTarget.z); 

        //float x1 = currentDirection.x;
        //float x2 = targetDirection.x;
        //float y1 = currentDirection.y;
        //float y2 = currentDirection.y;

        //string sx1 = currentDirection.x.ToString();
        //string sx2 = targetDirection.x.ToString();
        //string sy1 = currentDirection.y.ToString();
        //string sy2 = targetDirection.y.ToString();

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

    //public static bool IsParallel(Vector3 v1, Vector3 v2)
    //{
    //    //return Vector3.Equals(Vector3.Cross(v1, v2), Vector3.zero);
    //    Vector3 v3 = new Vector3(0, 0, 1);
    //    Vector3 v4 = new Vector3(0, 0, 1);
    //    bool derp = false;
    //    if(v3 == v4)
    //    {
    //        derp = true;
    //    }

    //    Vector3 v5 = v3;
    //    if(v3 == v5)
    //    {
    //        derp = true;
    //    }

    //    Vector3 cross = Vector3.Cross(v1, v2);
    //    Vector3 zero = Vector3.zero;
    //    Vector3 zero2 = new Vector3(0, 0, 0);
    //    if(cross == zero)
    //    {
    //        return true;
    //    }
    //    else if(cross == zero2)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
