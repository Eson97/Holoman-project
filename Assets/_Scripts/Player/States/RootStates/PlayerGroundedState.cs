using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.PlayerController.IsDashPressed && Ctx.CanDash)
        {
            SwitchState(Factory.Dashing());
        }
        else if (Ctx.PlayerController.IsJumpPressed && Ctx.CanJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (!Ctx.IsGrounded)
        {
            SwitchState(Factory.Fall());
        }
    }
    
    public void InitializeSubState()
    {
        if (Ctx.PlayerController.IsMoving && Ctx.PlayerController.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
        else if (Ctx.PlayerController.IsMoving && !Ctx.PlayerController.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Idle());
        }
    }

}
