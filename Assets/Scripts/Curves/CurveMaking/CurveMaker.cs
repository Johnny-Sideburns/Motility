using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class CurveMaker : MonoBehaviour
{
    [SerializeField] private List<Transform> _curvePositionTransforms;
    [SerializeField] private List<Transform> _curveRotationTransforms;
    [SerializeField] private AnimationCurve _timePos;
    [SerializeField] private AnimationCurve _timeRot;
    public RelativeCurveData _relatedData;
    private CurveMoveData _curveMoveData = new CurveMoveData();
    public int _copyPosition;
    public Transform _target;
    public CurveMoverContext _context;


    void OnDrawGizmos()
    {
        PreviewDrawer();
    }

    [ContextMenu("Save Positions")]
    void SavePositions()
    {
        _curveMoveData.CurvePositions = _curvePositionTransforms.Select(i =>  _context.GetBodyTransform(_relatedData.relativeTransform).InverseTransformPoint(i.position) ).ToList();
        _curveMoveData.TimePos = _timePos;
    }

    [ContextMenu("Save Rotations")]
    void SaveRotations()
    {
        _curveMoveData.CurveRotations = _curveRotationTransforms.Select(i => Quaternion.Inverse(_context.GetBodyTransform(_relatedData.relativeTransform).rotation) * i.rotation).ToList();
        _curveMoveData.TimeRot = _timeRot;
    }

    [ContextMenu("Save Data")]
    public void SaveData()
    {
        _context = transform.parent.GetComponent<CurveMakerController>()._context;

        SavePositions();
        SaveRotations();
        _relatedData.curveMoveData = _curveMoveData;
    }

    void PreviewDrawer()
    {
        if (_curvePositionTransforms.Count < 1) return;
        
        Gizmos.color = Color.red;
        int res = _curvePositionTransforms.Count * 20;
        
        Vector3 prev = _curvePositionTransforms[0].position;
        
        for (int i = 0; i < res; i++)
        {
            float t = i/(float)res;
            Vector3 point = BezierCurve.GetBezierPosition(_curvePositionTransforms.Select(i => i.position).ToList(), t);
            Gizmos.DrawLine(prev, point);
            prev = point;
        }   
    }

    [ContextMenu ("Copy Position")]
    void CopyPosition()
    {
        if (_curvePositionTransforms.Count > _copyPosition)
        {
            _curvePositionTransforms[_copyPosition].position = _target.position;
        }
    }

    [ContextMenu ("Copy Rotation")]
    void CopyRotation()
    {
        if (_curveRotationTransforms.Count > _copyPosition)
        {
            _curveRotationTransforms[_copyPosition].rotation = _target.rotation;
        }
    }
}
