using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static float GetDiffAngle2D(Vector3 forward, Vector3 vectorToTarget)
    {
        Vector2 currentDirection = new Vector2(forward.x, forward.z).normalized;
        Vector2 targetDirection = new Vector2(vectorToTarget.x, vectorToTarget.z).normalized;

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
}
