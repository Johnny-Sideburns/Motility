using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Marionette : MonoBehaviour
{
    public CurveMoverContext _curveMoverContext;
    [Header ("Idle Animation")]
    public List<CurveAnimationHolder> _idleAnimations;
    public List<float> _idleWeights;
    private Dictionary<CurveMover, CompositeAnimation> _idleCompositeAnimations;
    [Header ("Walk Animation")]
    public List<CurveAnimationHolder> _walkAnimations;
    public List<float> _walkWeights;
    private Dictionary<CurveMover, CompositeAnimation> _walkCompositeAnimations;
    [Header ("Run Animation")]
    public List<CurveAnimationHolder> _runAnimations;
    public List<float> _runWeights;
    private Dictionary<CurveMover, CompositeAnimation> _runCompositeAnimations;

    [Header ("Other things")]
    [Range(0,1)]public float _transitionWeight;

    public Dictionary<CurveMover, List<CompositeAnimation>> _activeAnimations;
    public Collider _collider;
    public Transform _metarig;
    private List<Collider> _ragdollColliders;

    public bool _animate;

    void Awake()
    {
        CreateMoverMap();
        PrepareAnimations();
        ResetMoverTransforms();
        //SetRagdollColliders();
        //RagdollOff();
    }

    void CreateMoverMap()
    {
        _curveMoverContext.InitiateMaps();
        _activeAnimations = new Dictionary<CurveMover, List<CompositeAnimation>>();
        foreach (CurveMover curveMover in _curveMoverContext.Movers)
        {
            _activeAnimations[curveMover] = new List<CompositeAnimation>{null,null};
        }
    }

    void FixedUpdate()
    {
        if (!_animate) return;
        PlayAnimations();
    }
   
    public void AnimateIdle()
    {
        //PrepareAnimation(_idleAnimations, _idleWeights);
        //_curveMoverContext.GetMover(CurveMoverContext.EMover.LeftFootTarget).Hold();
        //_curveMoverContext.GetMover(CurveMoverContext.EMover.RightFootTarget).Hold();
        if (_idleCompositeAnimations == null)
        {
            return;
        }
        LoadAnimations(_idleCompositeAnimations);

    }
    private void PrepareAnimations()
    {
        _idleCompositeAnimations = PrepareAnimation(_idleAnimations, _idleWeights);
        
    }
    private Dictionary<CurveMover, CompositeAnimation> PrepareAnimation(List<CurveAnimationHolder> animations, List<float> weights)
    {
        Dictionary<CurveMover, CompositeAnimation> result = new Dictionary<CurveMover, CompositeAnimation>();
        for (int i = 0; i < animations.Count; i++)
        {
            CurveAnimationHolder currentAnimationHolder = animations[i];
            for (int n = 0; n < currentAnimationHolder._movers.Count; n++)
            {
                CurveMover currentEMover = _curveMoverContext.GetMover(currentAnimationHolder._movers[n]);
                if (result.ContainsKey(currentEMover))
                {
                    result[currentEMover]._relativeCurveData.Add(currentAnimationHolder._data[n]);
                    result[currentEMover]._timeSpan.Add(currentAnimationHolder._playtime);
                    result[currentEMover]._weights.Add(weights[i]);
                }
                else
                {
                    currentAnimationHolder._data[n].SetTransform(_curveMoverContext);
                    result[currentEMover] = new CompositeAnimation
                    {
                        _relativeCurveData = new List<RelativeCurveData>{currentAnimationHolder._data[n]},
                        _timeSpan = new List<float> {currentAnimationHolder._playtime},
                        _weights = new List<float> {weights[i]},
                        _currentTime = 0f
                    };
                }
            }
        }
        return result;
    }
    private void LoadAnimations(Dictionary<CurveMover, CompositeAnimation> compositeAnimations, bool reset = false, float startValue = 0, bool queue = false)
    {
        foreach (KeyValuePair<CurveMover, CompositeAnimation> kvp in compositeAnimations)
        {
            if (reset) kvp.Value._currentTime = startValue;
            if (queue && _activeAnimations[kvp.Key].Count > 0 ) _activeAnimations[kvp.Key][1] = kvp.Value;
            else
            {
                _activeAnimations[kvp.Key][1] = _activeAnimations[kvp.Key][0];
                _activeAnimations[kvp.Key][0] = kvp.Value;
            }
        }
    }
    
    private void PlayAnimations()
    {
        foreach (KeyValuePair<CurveMover, List<CompositeAnimation>> kvp in _activeAnimations)
        {
            //Debug.Log(kvp.Key.name + " : " + kvp.Value.Count);

            if (kvp.Value.Count < 1) continue;
            if (kvp.Value[0] == null) continue;

            CompositeAnimation firstCompositeAnimation = kvp.Value[0];
            //move time forward by a factor of the average of playtimes multiplied byt deltatime
            firstCompositeAnimation.AdvanceTime(Time.deltaTime);
            Vector3 accumulatedPosition;
            Quaternion finalRotation;
            GetPositionRotationFromCompositeAnimation(firstCompositeAnimation, firstCompositeAnimation._currentTime, out accumulatedPosition, out finalRotation);

            //then accumualte the seccond spot
            //#TODO

            //then interpolate
            //#TODO

            //finally move supply mover with desired position
            kvp.Key.Move(accumulatedPosition, finalRotation);
        }
    }

    private void GetPositionRotationFromCompositeAnimation(CompositeAnimation compositeAnimation, float t, out Vector3 position, out Quaternion rotation)
    {
        float weightSum = compositeAnimation._weights.Sum();

        Quaternion refferenceRotation = Quaternion.identity;
        bool foundRefference = false;
        Vector4 accumRotation = Vector4.zero;
        Vector3 accumulatedPosition = Vector3.zero;
        //first accumulate the first spot
        for (int i = 0; i < compositeAnimation._relativeCurveData.Count; i++)
        {
            float normalizedWeight = compositeAnimation._weights[i]/weightSum;
            if (normalizedWeight == 0) continue;
            accumulatedPosition = BezierCurve.MixPosition(compositeAnimation._relativeCurveData[i].curveMoveData, normalizedWeight, compositeAnimation._relativeCurveData[i].actualTransform, t, accumulatedPosition);
            
            if (!foundRefference) {
                refferenceRotation = BezierCurve.GetRotation(compositeAnimation._relativeCurveData[i].curveMoveData, t);
                foundRefference = true;
            }
            accumRotation = BezierCurve.MixRotation(compositeAnimation._relativeCurveData[i].curveMoveData, normalizedWeight, compositeAnimation._relativeCurveData[i].actualTransform, t, refferenceRotation, accumRotation);
        }
        Quaternion finalRotation = BezierCurve.FinalizeRotation(accumRotation, refferenceRotation);

        position = accumulatedPosition;
        rotation = finalRotation;
        
    }

    //this method is a bit of a mess r/n todo fix it up
    public void ResetMoverTransforms()
    {
        _metarig.SetParent(null, true);
        transform.position = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.Hip).transform.position;
        //transform.rotation = Quaternion.Euler(0, _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.Hip).transform.rotation.y ,0);

        /*
        Vector3 l = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.RightFoot).transform.position;
        Vector3 r = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.LeftFoot).transform.position;
        Vector3 h = (l + r)/2;
        Vector3 f = (h + _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.Hip).transform.position) /2;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        */        
        _metarig.SetParent(transform, true);
        
        _curveMoverContext.GetMover(CurveMoverContext.EMover.Hip).transform.position = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.Hip).transform.position;
        _curveMoverContext.GetMover(CurveMoverContext.EMover.Hip).transform.rotation = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.Hip).transform.rotation * Quaternion.Inverse(Quaternion.Euler(16,0,0));
    
        _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftHandTarget).transform.position = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.LeftHand).transform.position;
        _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftHandTarget).transform.rotation = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.LeftHand).transform.rotation * Quaternion.Inverse(Quaternion.Euler(-45,90,90));
    
        _curveMoverContext.GetMover(CurveMoverContext.EMover.RightHandTarget).transform.position = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.RightHand).transform.position;
        _curveMoverContext.GetMover(CurveMoverContext.EMover.RightHandTarget).transform.rotation = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.RightHand).transform.rotation * Quaternion.Inverse(Quaternion.Euler(225,90,90));

        _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftFootTarget).transform.position = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.LeftFoot).transform.position;
        _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftFootTarget).transform.rotation = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.LeftFoot).transform.rotation * Quaternion.Inverse(Quaternion.Euler(-122,180,0));

        _curveMoverContext.GetMover(CurveMoverContext.EMover.RightFootTarget).transform.position = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.RightFoot).transform.position;
        _curveMoverContext.GetMover(CurveMoverContext.EMover.RightFootTarget).transform.rotation = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.RightFoot).transform.rotation * Quaternion.Inverse(Quaternion.Euler(-122,180,0));
        
        _curveMoverContext.GetMover(CurveMoverContext.EMover.Chest).transform.position = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.Chest).transform.position;
        _curveMoverContext.GetMover(CurveMoverContext.EMover.Chest).transform.rotation = _curveMoverContext.GetBodyTransform(CurveMoverContext.ERelativeTransform.Chest).transform.rotation;
        
    }

    public void ActivateAnimations(bool activate)
    {
        gameObject.GetComponent<Animator>().enabled = activate;
        _animate = activate;    
    }

    private void SetRagdollColliders()
    {
        _ragdollColliders = gameObject.GetComponentsInChildren<Collider>().ToList();
        if (_ragdollColliders.Contains(_collider)) _ragdollColliders.Remove(_collider);
    }

    public void RagdollOff()
    {
        _collider.isTrigger = false;
        foreach(Collider collider in _ragdollColliders)
        {
            collider.isTrigger = true;
            collider.attachedRigidbody.linearVelocity = Vector3.zero;
            collider.attachedRigidbody.angularVelocity = Vector3.zero;
        }
        ActivateAnimations(true);
        _collider.attachedRigidbody.linearVelocity = Vector3.zero;
        _collider.attachedRigidbody.angularVelocity = Vector3.zero;
        //_collider.attachedRigidbody.useGravity = true;

    }

    public void RagDollOn()
    {
        ActivateAnimations(false);
        _collider.isTrigger = true;
        foreach (Collider collider in _ragdollColliders)
        {
            collider.isTrigger = false;
            collider.attachedRigidbody.linearVelocity = Vector3.zero;//_collider.attachedRigidbody.linearVelocity;
            collider.attachedRigidbody.angularVelocity = Vector3.zero;
        }
        _collider.attachedRigidbody.linearVelocity = Vector3.zero;
        _collider.attachedRigidbody.angularVelocity = Vector3.zero;
        _collider.attachedRigidbody.useGravity = false;
    }
/*
    //this needs to be changed to the other method (i.e. not putting into maps but just doing the calculation and passing the result)
    private void PrepareAnimation(List<CurveAnimationHolder> compositeAnimation, List<float> weights)
    {
        Dictionary<CurveMoverContext.EMover, List<WeightedCurveData>> tmpMovers = new Dictionary<CurveMoverContext.EMover, List<WeightedCurveData>>();
        for (int i = 0; i < compositeAnimation.Count; i++) 
        {
            
            //foreach (KeyValuePair<CurveMoverContext.EMover, RelativeCurveData> kvp in compositeAnimation[i]._map)
            for (int n = 0; n < compositeAnimation[i]._movers.Count; n++)
            {
                CurveMoverContext.EMover mover = compositeAnimation[i]._movers[n];
                WeightedCurveData weightedCurveData = new WeightedCurveData{ relativeCurveData = compositeAnimation[i]._data[n], weight = weights[0] };
                if (tmpMovers.ContainsKey(mover))
                {
                    tmpMovers[mover].Add(weightedCurveData);
                }
                else
                {
                    tmpMovers[mover] = new List<WeightedCurveData> {weightedCurveData};
                }
            }
        }

        foreach (KeyValuePair<CurveMoverContext.EMover, List<WeightedCurveData>> kvp in tmpMovers)
        {
            CurveMover curveMover = _curveMoverContext.GetMover(kvp.Key);
            curveMover.SetMoveData(kvp.Value);
            if (_activeMovers.Contains(curveMover)) continue;
            _activeMovers.Add(curveMover); 
        }
    }

    //this needs to run differently too
    private void PlayAnimations()
    {
        t += Time.deltaTime * d;
        if (t > 1 || t < 0) d *= -1; 
        foreach (CurveMover mover in _activeMovers)
        {
            mover.SetTimeX(t);
            mover.MixMove();
        }
    }
    */
    /*
    //int d = 1;
    private CurveMover _hip;
    private CurveMover _chest;
    private CurveMover _leftHandTarget;
    private CurveMover _leftHandPull;
    private CurveMover _rightHandTarget;
    private CurveMover _rightHandPull;
    private CurveMover _leftFootTarget;
    private CurveMover _leftFootPull;
    private CurveMover _rightFootTarget;
    private CurveMover _rightFootPull;
    */
    /*
    _hip = _curveMoverContext.GetMover(CurveMoverContext.EMover.Hip);
    _chest = _curveMoverContext.GetMover(CurveMoverContext.EMover.Chest);
    _leftHandTarget = _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftHandTarget);
    _leftHandPull = _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftHandPull);
    _rightHandTarget = _curveMoverContext.GetMover(CurveMoverContext.EMover.RightHandTarget);
    _rightHandPull = _curveMoverContext.GetMover(CurveMoverContext.EMover.RightHandPull);
    _leftFootTarget = _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftFootTarget);
    _leftFootPull = _curveMoverContext.GetMover(CurveMoverContext.EMover.LeftFootPull);
    _rightFootTarget = _curveMoverContext.GetMover(CurveMoverContext.EMover.RightFootTarget);
    _rightFootPull = _curveMoverContext.GetMover(CurveMoverContext.EMover.RightFootPull);
    */
}