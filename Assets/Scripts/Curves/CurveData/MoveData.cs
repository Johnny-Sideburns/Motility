using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MoveData
{
    public CurveMoverContext.EMover mover;
    public List<WeightedCurveData> weightedCurvedata = new List<WeightedCurveData>();
}
