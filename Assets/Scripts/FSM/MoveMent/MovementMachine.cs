using UnityEngine;

public class MovementMachine : StateManager<MovementMachine.EMoveState>
{
    public enum EMoveState
    {
        StepShuffle,
        Idle,
        Walk
    }

    private MoveContext _context;
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

    void Awake()
    {
        _context = new MoveContext(
            _rb,
            _rootCollider,
            _walkArms,
            _walkLeftFoot,
            _walkRightFoot,
            _idleArms,
            _idleLeftFoot,
            _idleRightFoot,
            _leftFootDetector,
            _rightFootDetector
        );
        InitializeStates();
    }

    private void InitializeStates()
    {
        States.Add(EMoveState.Idle, new IdleState(_context, EMoveState.Idle));
        States.Add(EMoveState.Walk, new WalkState(_context, EMoveState.Walk));
        States.Add(EMoveState.StepShuffle, new StepShuffleState(_context, EMoveState.StepShuffle));
        CurrentState = States[EMoveState.Idle];
    }
}
