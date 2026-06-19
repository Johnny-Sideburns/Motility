using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CurveMover : MonoBehaviour
{
    [SerializeField] private bool _isLocked;
    [SerializeField] private float _snapSpeed;
    [SerializeField] private float _timeX;
    private Dictionary<CurveMoveData,Tuple<float, Transform>> _moveData;
    private Vector3 _lockPos;
    private Quaternion _lockRot;
    private float _weightSum;
    public CurveMoverContext _context;
    void FixedUpdate()
    {
        // or NOT move
        if (_isLocked) {
            
            transform.position = _lockPos;
            transform.rotation = _lockRot;
            return;
        }
        //MixMove();
    }

    public void MixMove()
    {        
        transform.position = Vector3.MoveTowards(transform.position, GetPositionMixed(), _snapSpeed * Time.deltaTime);

        transform.rotation = GetRotationMixed();
    }
    private Vector3 GetPositionMixed()
    {
        Vector3 result = Vector3.zero;
        foreach (KeyValuePair<CurveMoveData, Tuple<float, Transform>> item in _moveData)
        {
            float weight = item.Value.Item1/_weightSum;
            float t = item.Key.TimePos.Evaluate(_timeX);
            Transform relative = item.Value.Item2;
            result += relative.TransformPoint(BezierCurve.GetBezierPosition(item.Key.CurvePositions, t)) * weight;
        }

        return result;
    }
    
    //"Estimating the Average of a Set of Rotations" (Markley et al., 2007)
    private Quaternion GetRotationMixed()
    {        
        // Find reference quaternion (first non-zero weight)
        Quaternion reference = Quaternion.identity;
        bool found = false;
        foreach (KeyValuePair<CurveMoveData, Tuple<float, Transform>> kvp in _moveData)
        {
            if (kvp.Value.Item1 != 0)
            {
                reference = kvp.Value.Item2.rotation * GetRotation(kvp.Key);
                found = true;
                break;
            }
        }
        if (!found) return Quaternion.identity;

        Vector4 accum = Vector4.zero;
        foreach (KeyValuePair<CurveMoveData, Tuple<float, Transform>> kvp in _moveData)
        {
            float w = kvp.Value.Item1 / _weightSum;
            Quaternion q = kvp.Value.Item2.rotation * GetRotation(kvp.Key);
            if (Quaternion.Dot(reference, q) < 0f) q = new Quaternion(-q.x, -q.y, -q.z, -q.w);
            accum += new Vector4(q.x, q.y, q.z, q.w) * w;
        }

        float mag = accum.magnitude;
        // fallback
        if (mag < 1e-6f) return reference;
        accum /= mag;
        return new Quaternion(accum.x, accum.y, accum.z, accum.w);
    }

    //using direct linear interpolation between quaternions for rotations
    public Quaternion GetRotation(CurveMoveData data)
    {
        float t = data.TimeRot.Evaluate(_timeX);

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
   
    public void SetTimeX(float timeX)
    {
        _timeX = timeX;
    }
    public void SetSnapSpeed(float snapSpeed)
    {
        _snapSpeed = snapSpeed;
    }
    public void Hold()
    {
        _isLocked = true;
        _lockPos = transform.position;
        _lockRot = transform.rotation;
    }
    public void UnHold()
    {
        _isLocked = false;
    }

    public void SetMoveData(List<WeightedCurveData> curveDatas)
    {
        _weightSum = 0;
        _moveData = new Dictionary<CurveMoveData,Tuple<float, Transform>>();
        foreach (WeightedCurveData curveData in curveDatas)
        {
            _moveData[curveData.relativeCurveData.curveMoveData] = new Tuple<float, Transform>(curveData.weight, _context.GetBodyTransform(curveData.relativeCurveData.relativeTransform));
            _weightSum += curveData.weight;
        }
    }
}