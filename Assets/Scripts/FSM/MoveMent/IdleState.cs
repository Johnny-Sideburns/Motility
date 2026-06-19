using UnityEngine;

public class IdleState : MovementState
{
    public IdleState(MoveContext context, MovementMachine.EMoveState estate) : base(context, estate)
    {
        MoveContext Context = context;
    }

    public override void EnterState()
    {
        Debug.Log("Enter State Idle");
        Context.IdleArms.Prepare();
        Context.IdleArms.Go();
    }
    public override void UpdateState()
    {
        
    }
    public override void ExitState()
    {
        Debug.Log("Exit State Idle");
        Context.IdleArms.Stop();
    }
    public override MovementMachine.EMoveState GetNextState()
    {
        if (Vector3.Magnitude(Context.Rb.linearVelocity) > 0.01)
        {
            return MovementMachine.EMoveState.Walk;
        }
        return StateKey;
    }
}