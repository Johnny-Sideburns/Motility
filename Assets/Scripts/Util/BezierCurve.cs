using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    public static Vector3 GetBezierPosition(List<Vector3> curve, float t)
    {
        return RecursiveLerp(curve, t);
    }
    public static Quaternion GetBezierRotation(List<Quaternion> curve, float t)
    {
        return RecursiveQLerp(curve, t);
    }
    private static Vector3 RecursiveLerp(List<Vector3> positions, float t)
    {
        List<Vector3> one_step_down_positions;
        while (positions.Count > 1)
        {
            one_step_down_positions = new List<Vector3>();
            for (int n = 0; n < positions.Count -1; n++)
            {   
                one_step_down_positions.Add(Vector3.Lerp(positions[n],positions[n+1], t));
            }
            positions = one_step_down_positions;
        }
        return positions[0];
        /*
        if (positions.Count == 2){
            return Vector3.Lerp(positions[0],positions[1], t);
        }
        
        List<Vector3> one_step_down_positions = new List<Vector3>();
        for (int n = 0; n < positions.Count -1; n++)
        {   
            one_step_down_positions.Add(Vector3.Lerp(positions[n],positions[n+1], t));
        }
        return RecursiveLerp(one_step_down_positions, t);
        */
    }
    private static Quaternion RecursiveQLerp(List<Quaternion> quaternions, float t)
    {
        if (quaternions.Count == 2){
            return Quaternion.Lerp(quaternions[0],quaternions[1], t);
        }
        
        List<Quaternion> one_step_down_quaternions = new List<Quaternion>();
        for (int n = 0; n < quaternions.Count -1; n++)
        {   
            one_step_down_quaternions.Add(Quaternion.Lerp(quaternions[n],quaternions[n+1], t));
        }
        return RecursiveQLerp(one_step_down_quaternions, t);
    }
    
}
