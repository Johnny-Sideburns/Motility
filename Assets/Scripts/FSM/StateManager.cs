using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class StateManager<EState> : MonoBehaviour where EState: Enum
{
    protected Dictionary<EState ,BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    public BaseState<EState> CurrentState {get; protected set;}
    protected bool IsTransitioningState = false;

    void Start()
    {
        CurrentState.EnterState();
    }
    void LateUpdate()
    {
        EState nextStateKey = CurrentState.GetNextState();

        if (!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey)) CurrentState.UpdateState();
        else if (!IsTransitioningState) TransitionToState(nextStateKey);
    }

    void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

/*
    void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }
*/
}
