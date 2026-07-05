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
        //Context.IdleArms.Prepare();
        //Context.IdleArms.Go();
        Context.Marionette.AnimateIdle();
    }
    public override void UpdateState()
    {
        
    }
    public override void ExitState()
    {
        Debug.Log("Exit State Idle");
        //Context.IdleArms.Stop();
    }
    public override MovementMachine.EMoveState GetNextState()
    {
        /*
        //ignore ragdoll falling stuff for now
        if (Context.Rb.linearVelocity.y < - 4)
        {
            return MovementMachine.EMoveState.Falling;
        }
        */
        if (Vector3.Magnitude(new Vector2(Context.Rb.linearVelocity.x, Context.Rb.linearVelocity.z)) > 0.01)
        {
            return MovementMachine.EMoveState.Walk;
        }
        return StateKey;
    }
}