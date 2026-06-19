using UnityEngine;

public class StepShuffleState : MovementState
{
    public StepShuffleState(MoveContext context, MovementMachine.EMoveState estate) : base(context, estate)
    {
        Context = context;
    }
    CurveMovePlayer _activeMover;
    CurveMovePlayer _otherMover;
    FootDetector _activeFootDetector;
    FootDetector _otherFootDetector;
    float _lerper;
    bool _placing;
    int _done;

    public override void EnterState()
    {
        Debug.Log("Enter State Step");

        SelectActiveFoot();
        _activeMover.ResetValues();
        _activeMover.Prepare();

        _activeMover.UnHold();
        _activeMover.Go();

        _otherMover.Prepare();
        _otherMover.Hold();
        _otherMover.Go();
        _otherMover.ResetValues();

        Context.IdleArms.Prepare();
        Context.IdleArms.ResetValues();
        Context.IdleArms.Reverse *= -1;
        Context.IdleArms.UnHold();
        Context.IdleArms.Go();

        _done = 0;

    }
    public override void UpdateState()
    {
        if (_activeFootDetector.HasTarget)
        {
            _otherMover.Stop();
            _lerper = 0;
            _placing = true;
            
        }

        if (!_placing) return;
        Placing();
    }


    public override void ExitState()
    {
        Debug.Log("Exit State Step");
        _otherMover.Stop();
        _activeMover.Stop();
        Context.IdleArms.Stop();
    }
    public override MovementMachine.EMoveState GetNextState()
    {
        if (_done >= 2)
        {
            return MovementMachine.EMoveState.Idle;
        }
        if (Context.Rb.linearVelocity.magnitude > 0.01)
        {
            return MovementMachine.EMoveState.Walk;
        }
        return StateKey;
    }

    void SwitchFeet()
    {
        CurveMovePlayer tmpMover = _activeMover;
        FootDetector tmpDetector = _activeFootDetector;
        _activeMover = _otherMover;
        _activeFootDetector = _otherFootDetector;
        _otherMover = tmpMover;
        _otherFootDetector = tmpDetector;
        Context.IdleArms.Reverse *= -1;
        if (Context.IdleArms.Reverse ==-1) Context.IdleArms._timer = 1;

    }

    void SelectActiveFoot()
    {
        _activeMover = Context.IdleLeftFoot;
        _activeFootDetector = Context.LeftFootDetector;
        _otherMover = Context.IdleRightFoot;
        _otherFootDetector = Context.RightFootDetector;
        Context.IdleArms.Reverse = -1;
        
        if (!_activeFootDetector.IsGrounded && _otherFootDetector.IsGrounded) return;
        if (_activeFootDetector.IsGrounded && !_otherFootDetector.IsGrounded)
        {
            SwitchFeet();
            return;
        }
        Vector3 projectedPosition = Context.Rb.transform.position + Context.Rb.transform.forward;
        if (Vector3.Distance(_activeFootDetector.transform.position, projectedPosition) < Vector3.Distance(_otherFootDetector.transform.position, projectedPosition))
        {
            SwitchFeet();
            return;
        }
        
    }

    void Reseting()
    {
        _activeMover.ResetValues();
        Context.IdleArms.Prepare();
        Context.IdleArms.ResetValues();
        Context.IdleArms.UnHold();
        Context.IdleArms.Go();        
        _otherMover.ResetValues();
        _otherMover.UnHold();
        _otherMover.Go();
    }

    void Placing()
    {
        _lerper += Time.deltaTime *5;
        _otherFootDetector.transform.position = Vector3.Lerp(_otherFootDetector.transform.position, _otherFootDetector.GetTargetPositon, _lerper);
        _otherFootDetector.transform.rotation = Quaternion.Slerp(_otherFootDetector.transform.rotation, _otherFootDetector.GetTargetRotation, _lerper);
        if (_lerper >= 1)
        {
            
            _placing = false;
            _otherMover.Hold();
            _otherMover.Go();
            _done +=1;
            Reseting();
            SwitchFeet();
        }

    }
}