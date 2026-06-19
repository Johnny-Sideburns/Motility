using UnityEngine;

public class WalkState : MovementState
{
    public WalkState(MoveContext context, MovementMachine.EMoveState estate) : base(context, estate)
    {
        Context = context;
    }
    CurveMovePlayer _activeMover;
    CurveMovePlayer _otherMover;
    FootDetector _activeFootDetector;
    FootDetector _otherFootDetector;
    float _lerper;
    bool _placing;


    public override void EnterState()
    {
        Debug.Log("Enter State Walk");

        SelectActiveFoot();
        _activeMover.ResetValues();
        _activeMover.Prepare();
        _activeMover._timer = Context.WalkArms.TimeMax /1.3f;

        _activeMover.UnHold();
        _activeMover.Go();

        _otherMover.Prepare();
        _otherMover.Hold();
        _otherMover.Go();
        //_otherMover.Stop();
        _otherMover.ResetValues();

        /*
        */
        Context.WalkArms.Prepare();
        Context.WalkArms.ResetValues();
        Context.WalkArms._timer = Context.WalkArms.TimeMax /1.3f;
        Context.WalkArms.Reverse *= -1;
        Context.WalkArms.UnHold();
        Context.WalkArms.Go();

    }
    public override void UpdateState()
    {
        if (_activeFootDetector.HasTarget)
        {
            //_activeMover.Hold();
            Reseting();
            SwitchFeet();
            _otherMover.Stop();
            _lerper = 0;
            _placing = true;


            
        } else if (_activeMover.IsDone)
        {
            Reseting();
            SwitchFeet();
        }

        if (!_placing) return;
        Placing();
    }


    public override void ExitState()
    {
        Debug.Log("Exit State Walk");
        _otherMover.Stop();
        _activeMover.Stop();
        Context.WalkArms.Stop();
    }
    public override MovementMachine.EMoveState GetNextState()
    {
        if (Context.Rb.linearVelocity.magnitude < 0.01)
        {
            return MovementMachine.EMoveState.StepShuffle;
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
        Context.WalkArms.Reverse *= -1;
        if (Context.WalkArms.Reverse ==-1) Context.WalkArms._timer = 1;



    }

    void SelectActiveFoot()
    {
        _activeMover = Context.WalkLeftFoot;
        _activeFootDetector = Context.LeftFootDetector;
        _otherMover = Context.WalkRightFoot;
        _otherFootDetector = Context.RightFootDetector;
        Context.WalkArms.Reverse = -1;
        
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
        Context.WalkArms.Prepare();
        Context.WalkArms.ResetValues();
        Context.WalkArms.UnHold();
        Context.WalkArms.Go();        
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

        }

    }
}

