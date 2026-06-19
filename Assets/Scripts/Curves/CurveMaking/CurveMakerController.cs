using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[ExecuteAlways]
public class CurveMakerController : MonoBehaviour
{
    [Header ("Hip")]
    public CurveMaker _hip;
    public bool _useHip;
    [Header ("Chest")]
    public CurveMaker _chest;
    public bool _useChest;
    [Header ("Left Hand")]
    public CurveMaker _leftHandTarget;
    public CurveMaker _leftHandPull;
    public bool _useLeftHand;
    [Header ("Right Hand")]
    public CurveMaker _rightHandTarget;
    public CurveMaker _rightHandPull;
    public bool _useRightHand;
    [Header ("Left Foot")]
    public CurveMaker _leftFootTarget;
    public CurveMaker _leftFootPull;
    public bool _useLeftFoot;
    [Header ("Right Foot")]
    public CurveMaker _rightFootTarget;
    public CurveMaker _rightFootPull;
    public bool _useRightFoot;
    [Range(0,1)]public float _t;
    public float _playtime;
    public bool _rig, _preview, _play, _reverse, _loop;
    private List<CurveMover> _movers;
    public RigEdit _rigEdit;
    private float _lastTime;
    private Dictionary<CurveMaker, CurveMoverContext.EMover> _map;
    public CurveMoverContext _context;
    public CurveAnimationHolder _curveAnimation;

    void Awake()
    {
        _lastTime = Time.realtimeSinceStartup;
        _movers = new List<CurveMover>();
        CreateMap();
    }
    void Update()
    {
        float _speed = 1f/_playtime;
        float nowTime = Time.realtimeSinceStartup;
        float time = nowTime - _lastTime;
        _lastTime = nowTime;
        if (_play)
        {
            if (_reverse)
            {
                float t = _t - time * _speed;
                if (t < 0)
                {
                    if (_loop) _t = 1;
                    else _t = 0;
                }
                else
                {
                    _t = t;
                }           
            }
            else
            {
                float t = _t + time * _speed;
                if (t > 1)
                {
                    if (_loop) _t = 0;
                    else _t = 1;
                }
                else
                {
                    _t = t;
                }
            }
        }
        if (_rig)
        {   
            if (_rigEdit != null) _rigEdit.Do();
            _rig = false;
        } 

        if (!_preview) return;
        StuffData();
        _movers.ForEach(x => x.SetTimeX(_t));
        _movers.ForEach(x => x.MixMove());
    }
    [ContextMenu ("Save Gesticulation")]
    void Save()
    {   
        CreateMap();  
        _curveAnimation._playtime = _playtime;
        foreach (KeyValuePair<CurveMaker, CurveMoverContext.EMover> item in _map)
        {   
            item.Key.SaveData();
            _curveAnimation._map[item.Value] = item.Key._relatedData;
        }
    }
    void StuffData()
    {
        if (_map == null) Save();
        if (_movers != null) _movers.Clear();
        foreach (KeyValuePair<CurveMoverContext.EMover, RelativeCurveData> item in _curveAnimation._map)
        {
            CurveMover mover = _context.GetMover(item.Key);
            if (mover == null) continue;
            List<WeightedCurveData> tmp = new List<WeightedCurveData>{new WeightedCurveData{ weight = 1f, relativeCurveData = item.Value}};
            mover.SetMoveData(tmp);
            _movers.Add(mover);
        }
    }
    [ContextMenu ("Create Map")]
    void CreateMap()
    {
        _map = new Dictionary<CurveMaker, CurveMoverContext.EMover>();
        if (_hip != null && _useHip)                    _map[_hip] = CurveMoverContext.EMover.Hip;
        if (_chest != null && _useChest)                _map[_chest] = CurveMoverContext.EMover.Chest;
        if (_leftHandTarget != null && _useLeftHand)    _map[_leftHandTarget] = CurveMoverContext.EMover.LeftHandTarget;
        if (_leftHandPull != null && _useLeftHand)      _map[_leftHandPull] = CurveMoverContext.EMover.LeftHandPull;
        if (_rightHandTarget != null && _useRightHand)  _map[_rightHandTarget] = CurveMoverContext.EMover.RightHandTarget;
        if (_rightHandPull != null && _useRightHand)    _map[_rightHandPull] = CurveMoverContext.EMover.RightHandPull;
        if (_leftFootTarget != null && _useLeftFoot)    _map[_leftFootTarget] = CurveMoverContext.EMover.LeftFootTarget;
        if (_leftFootPull != null && _useLeftFoot)      _map[_leftFootPull] = CurveMoverContext.EMover.LeftFootPull;
        if (_rightFootTarget != null && _useRightFoot)  _map[_rightFootTarget] = CurveMoverContext.EMover.RightFootTarget;
        if (_rightFootPull != null && _useRightFoot)    _map[_rightFootPull] = CurveMoverContext.EMover.RightFootPull;
    }
}
