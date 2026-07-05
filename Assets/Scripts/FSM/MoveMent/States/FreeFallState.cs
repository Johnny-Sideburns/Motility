using UnityEngine;

public class FreeFallState : MovementState
{
    public float _timer;
    public int d = 1;
    public FreeFallState(MoveContext context, MovementMachine.EMoveState estate) : base(context, estate)
    {
        MoveContext Context = context;
    }

    public override void EnterState()
    {
        Debug.Log("Enter State Falling");
        Context.Marionette.RagDollOn();
        d = 1;
        _timer = 5;
        Context.PlayerController.FreezeSpeed();

    }
    public override void UpdateState()
    {
        if (Context.Rb.linearVelocity.y > - 0.001) d = -1;
        _timer += Time.deltaTime * d;
        Debug.Log(_timer);
    }
    public override void ExitState()
    {
        Debug.Log("Exit State Falling");
        Context.Marionette.RagdollOff();
        Context.Marionette.ResetMoverTransforms();
        
    }
    public override MovementMachine.EMoveState GetNextState()
    {
        if (_timer < 0)
        {
            return MovementMachine.EMoveState.Recover;
        }
        return StateKey;
    }
}