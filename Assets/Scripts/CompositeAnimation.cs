using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeAnimation
{
    public List<RelativeCurveData> _relativeCurveData;
    public List<float> _weights;
    public List<float> _timeSpan;
    public float _currentTime;

    public void AdvanceTime(float t)
    {
        _currentTime += 1/1/(_timeSpan.Sum()/ _timeSpan.Count) *t;
    }
}
