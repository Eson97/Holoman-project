using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    private bool _isRootState = false;

    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;

    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;

    protected bool IsRootState { set => _isRootState = value; }
    protected PlayerStateMachine Ctx => _ctx;
    protected PlayerStateFactory Factory => _factory;

    //borrar, solo para debug
    public PlayerBaseState SubState => _currentSubState;

    public PlayerBaseState(PlayerStateMachine currentContext,PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void CheckSwitchStates();

    public void UpdateStates()
    {
        UpdateState();
        _currentSubState?.UpdateStates();
    }
    public void FixedUpdateStates()
    {
        FixedUpdateState();
        _currentSubState?.FixedUpdateState();
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        //current state exits state
        ExitState();

        //new state enters state
        newState.EnterState();

        if (_isRootState)
        {
            //switch current state of context
            _ctx.CurrentState = newState;
        }
        else
        {
            //set the current superstate the new substateprivate
            _currentSuperState?.SetSubState(newState);
        }
    }
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState?.SetSuperState(this);
    }
}
