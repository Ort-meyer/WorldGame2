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
