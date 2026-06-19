using System;
using System.Collections.Generic;
using UnityEngine;

public class CurveMoverContext : MonoBehaviour
{
    public enum EMover{
        Hip,
        Chest,
        LeftHandTarget,
        LeftHandPull,
        RightHandTarget,
        RightHandPull,
        LeftFootTarget,
        LeftFootPull,
        RightFootTarget,
        RightFootPull,
    }
    [Header ("Movers")]
    public CurveMover _hip;
    public CurveMover _chest;
    public CurveMover _leftHandTarget;
    public CurveMover _leftHandPull;
    public CurveMover _rightHandTarget;
    public CurveMover _rightHandPull;
    public CurveMover _leftFootTarget;
    public CurveMover _leftFootPull;
    public CurveMover _rightFootTarget;
    public CurveMover _rightFootPull;
    private Dictionary<EMover, CurveMover> _moverMap;
    public enum ERelativeTransform
    {
        Body,
        Hip,
        Chest,
        Head,
        LeftHand,
        RightHand
    }

    [Header ("Transforms")]
    public Transform _bodyTransform;
    public Transform _hipTransform;
    public Transform _chestTransform;
    public Transform _headTransform;
    public Transform _leftHandTransform;
    public Transform _rightHandTransform;
    private Dictionary<ERelativeTransform, Transform> _transformMap;
 
    void Awake()
    {
        CreateMoverMap();
        CreateTransformMap();
    }
    void CreateMoverMap()
    {
        _moverMap = new Dictionary<EMover, CurveMover>();
        if (_hip != null)               _moverMap[EMover.Hip] = _hip;
        if (_chest != null)             _moverMap[EMover.Chest] = _chest;
        if (_leftHandTarget != null)    _moverMap[EMover.LeftHandTarget] = _leftHandTarget;
        if (_leftHandPull != null)      _moverMap[EMover.LeftHandPull] = _leftHandPull;
        if (_rightHandTarget != null)   _moverMap[EMover.RightHandTarget] = _rightHandTarget;
        if (_rightHandPull != null)     _moverMap[EMover.RightHandPull] = _rightHandPull;
        if (_leftFootTarget != null)    _moverMap[EMover.LeftFootTarget] = _leftFootTarget;
        if (_leftFootPull != null)      _moverMap[EMover.LeftFootPull] = _leftFootPull;
        if (_rightFootTarget != null)   _moverMap[EMover.RightFootTarget] = _rightFootTarget;
        if (_rightFootPull != null)     _moverMap[EMover.RightFootPull] = _rightFootPull;
    }
    public CurveMover GetMover(EMover mover)
    {
        if (_moverMap == null) CreateMoverMap();
        _moverMap.TryGetValue(mover, out CurveMover result);
        return result;
    }
    private void CreateTransformMap()
    {
        _transformMap = new Dictionary<ERelativeTransform, Transform>();
        if (_bodyTransform != null)     _transformMap[ERelativeTransform.Body] = _bodyTransform;
        if (_hipTransform != null)     _transformMap[ERelativeTransform.Hip] = _hipTransform;
        if (_chestTransform != null)     _transformMap[ERelativeTransform.Chest] = _chestTransform;
        if (_headTransform != null)     _transformMap[ERelativeTransform.Head] = _headTransform;
        if (_leftHandTransform != null)     _transformMap[ERelativeTransform.LeftHand] = _leftHandTransform;
        if (_rightHandTransform != null)     _transformMap[ERelativeTransform.RightHand] = _rightHandTransform;
    }

    public Transform GetBodyTransform(ERelativeTransform eRelativeTransform)
    {
        CreateTransformMap();
        _transformMap.TryGetValue(eRelativeTransform, out Transform result);
        return result;
    }
}
