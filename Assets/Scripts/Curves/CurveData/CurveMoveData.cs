using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurveMoveData
{
    public AnimationCurve TimePos;
    public AnimationCurve TimeRot;
    public List<Vector3> CurvePositions;    
    public List<Quaternion> CurveRotations;
}
