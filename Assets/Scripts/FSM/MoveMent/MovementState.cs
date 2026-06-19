using UnityEngine;

public abstract class MovementState : BaseState<MovementMachine.EMoveState>
{
    protected MoveContext Context;

    public MovementState(MoveContext context, MovementMachine.EMoveState stateKey) : base(stateKey)
    {
        Context = context;
    }
}