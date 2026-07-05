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
    
    public static Vector3 MixPosition(CurveMoveData curveMoveData, float normalizedWeight, Transform relative, float timeX, Vector3 accPosition)
    {
        float t = curveMoveData.TimePos.Evaluate(timeX);
        return accPosition += relative.TransformPoint(GetBezierPosition(curveMoveData.CurvePositions, t)) * normalizedWeight;
    }
    
    //using direct linear interpolation between quaternions for rotations
    public static Quaternion GetRotation(CurveMoveData data, float t)
    {
        //float t = data.TimeRot.Evaluate(timeX);

        if (t >= 1)
        {
            return data.CurveRotations[^1];
        }
        
        //find where it is in time between points
        float tConcrete = t * (data.CurveRotations.Count - 1);
        int i = Mathf.FloorToInt(tConcrete);

        // issue can occur here IF tr = 1
        i = Mathf.Clamp(i, 0, data.CurveRotations.Count - 2);

        float interT = tConcrete - i;
        return Quaternion.Slerp(data.CurveRotations[i], data.CurveRotations[i+1], interT);
    }

    public static Vector4 MixRotation(CurveMoveData data, float normalizedWeight, Transform relative, float timeX, Quaternion reference, Vector4 accum) {
        float t = data.TimeRot.Evaluate(timeX);

        Quaternion localRot = GetRotation(data, t);
        Quaternion worldRot = relative.rotation * localRot;

        // Hemisphere correction
        if (Quaternion.Dot(reference, worldRot) < 0f)
        {
            worldRot = new Quaternion(-worldRot.x, -worldRot.y, -worldRot.z, -worldRot.w);
        }

        accum += new Vector4(worldRot.x, worldRot.y, worldRot.z, worldRot.w) * normalizedWeight;

        return accum;
    }

    public static Quaternion FinalizeRotation(Vector4 accum, Quaternion fallback)
    {
        float mag = accum.magnitude;

        if (mag < 1e-6f)
            return fallback;

        accum /= mag;

        return new Quaternion(accum.x, accum.y, accum.z, accum.w);
    }
   
}
