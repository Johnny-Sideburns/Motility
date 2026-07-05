using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RelativeCurveData
{
    public CurveMoveData curveMoveData;
    public CurveMoverContext.ERelativeTransform relativeTransform;
    public Transform actualTransform;

    public void SetTransform(CurveMoverContext context)
    {
        actualTransform = context.GetBodyTransform(relativeTransform);
    }
}
