using UnityEngine;

public class RecoverState : MovementState
{
    public float _timer;
    public int d = 1;
    public RecoverState(MoveContext context, MovementMachine.EMoveState estate) : base(context, estate)
    {
        MoveContext Context = context;
    }

    public override void EnterState()
    {
        Debug.Log("Enter State Recover");
        d = 1;
        _timer = 3;
        Context.Marionette.AnimateIdle();
    }
    public override void UpdateState()
    {
        if (Context.Rb.linearVelocity.y > - 0.001) d = -1;
        _timer += Time.deltaTime * d;
    }
    public override void ExitState()
    {
        Debug.Log("Exit State Recover");
        Context.Rb.linearVelocity = Vector3.zero;
        Context.Rb.angularVelocity = Vector3.zero;
        Context.Rb.useGravity = true;
        Context.PlayerController.ResetSpeed();
    }
    public override MovementMachine.EMoveState GetNextState()
    {
        if (_timer < 0)
        {
            return MovementMachine.EMoveState.Idle;
        }
        return StateKey;
    }
}