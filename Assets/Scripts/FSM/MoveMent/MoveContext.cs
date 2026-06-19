using UnityEngine;
using UnityEngine.Animations.Rigging;
public class MoveContext
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] CapsuleCollider _rootCollider;
    [SerializeField] CurveMovePlayer _walkArms;
    [SerializeField] CurveMovePlayer _walkLeftFoot;
    [SerializeField] CurveMovePlayer _walkRightFoot;
    [SerializeField] CurveMovePlayer _idleArms;
    [SerializeField] CurveMovePlayer _idleLeftFoot;
    [SerializeField] CurveMovePlayer _idleRightFoot;
    [SerializeField] FootDetector _leftFootDetector;
    [SerializeField] FootDetector _rightFootDetector;

    public MoveContext(
        Rigidbody rb, 
        CapsuleCollider rootCollider,
        CurveMovePlayer walkArms,
        CurveMovePlayer walkLeftFoot,
        CurveMovePlayer walkRightFoot,
        CurveMovePlayer idleArms,
        CurveMovePlayer idleLeftFoot,
        CurveMovePlayer idleRightFoot,
        FootDetector leftFootDetector,
        FootDetector rightFootDetector
        )
    {
        _rb = rb;
        _rootCollider = rootCollider;
        _walkArms = walkArms;
        _walkLeftFoot = walkLeftFoot;
        _walkRightFoot = walkRightFoot;
        _idleArms = idleArms;
        _idleLeftFoot = idleLeftFoot;
        _idleRightFoot = idleRightFoot;
        _leftFootDetector = leftFootDetector;
        _rightFootDetector = rightFootDetector;
    }

    public Rigidbody Rb => _rb;
    public CapsuleCollider RootCollider => _rootCollider;
    public CurveMovePlayer WalkArms => _walkArms;
    public CurveMovePlayer WalkLeftFoot => _walkLeftFoot;
    public CurveMovePlayer WalkRightFoot => _walkRightFoot;
    public CurveMovePlayer IdleArms => _idleArms;
    public CurveMovePlayer IdleLeftFoot => _idleLeftFoot;
    public CurveMovePlayer IdleRightFoot => _idleRightFoot;
    public FootDetector LeftFootDetector => _leftFootDetector;
    public FootDetector RightFootDetector => _rightFootDetector;
}