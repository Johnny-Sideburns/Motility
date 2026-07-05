using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StanceController : MonoBehaviour
{
    public CurveMoverContext _context;
    private CurveMover _leftFootMover;
    private CurveMover _rightFootMover;
    //private Vector3 _leftFootStancePosition;
    //private Vector3 _rightFootStancePosition;
    public FootDetector _leftFootDetector;
    public FootDetector _rightFootDetector;
    public Collider _collider;
    private List<Collider> _collidersInReach = new List<Collider>();
    private bool _leftIsGrounded, _rightIsGrounded, _leftPlacing, _rightPlacing;

    void Start()
    {
        _leftFootMover = _context.GetMover(CurveMoverContext.EMover.LeftFootTarget);
        _rightFootMover = _context.GetMover(CurveMoverContext.EMover.RightFootTarget);
    }
    void FixedUpdate()
    {
        PlaceFoot(_leftFootMover, out _leftIsGrounded);
        PlaceFoot(_rightFootMover, out _rightIsGrounded);
    }
    /*
    public void SetStancePositions(Vector3 left, Vector3 right)
    {
        _leftFootStancePosition = left;
        _rightFootStancePosition = right;
    }
    */

    public Vector3 GetStancePosition()
    {
        return Vector3.zero;
    }

    public float GetTargetHeight()
    {
        List<float> heights = new List<float>();
        if (_leftFootDetector.IsGrounded) heights.Add(_leftFootDetector.GetTargetPositon.y);
        if (_rightFootDetector.IsGrounded) heights.Add(_rightFootDetector.GetTargetPositon.y);
        if (heights.Count == 0) return 0;
        return heights.Sum()/heights.Count;
    }

    void OnTriggerEnter(Collider collider)
    {
        _collidersInReach.Add(collider);
        Debug.Log("Enter: " + collider.transform.name);
    }

    void OnTriggerExit(Collider collider)
    {
        _collidersInReach.Remove(collider);
    }

    public bool IsGrounded()
    {
        if (_leftIsGrounded || _rightIsGrounded) return true;
        return false;
    }

    private Vector3 ClosestValidFootPoint(CurveMover footMover)
    {
        //return new Vector3(footMover.transform.position.x, 0, footMover.transform.position.z -_footOffSet.z) + _footOffSet;
        return _collidersInReach.OrderBy(c => c.ClosestPoint(footMover.transform.position)).First().ClosestPoint(footMover.transform.position);
    }

    private void PlaceFoot(CurveMover footMover, out bool grounder)
    {
        if (_collidersInReach.Count == 0)
        {
            grounder = false;
            return;
        }
        if (Vector3.Distance(footMover.transform.position, ClosestValidFootPoint(footMover))< 0.1)
        {
            footMover.Hold();
            grounder = true;
            return;
        }
        grounder = false;
    }
}
