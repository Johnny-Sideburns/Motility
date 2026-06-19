using System.Collections.Generic;
using UnityEngine;

public class CurveAnimationHolder : MonoBehaviour
{
    public float _playtime;
    public Dictionary<CurveMoverContext.EMover, RelativeCurveData> _map = new Dictionary<CurveMoverContext.EMover, RelativeCurveData>();
}
