using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState, IRootState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isFalling", true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isFalling", false);
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
        else if(!Ctx.IsGrounded && Ctx.IsHoldingFromStickyWall)
        {
            SwitchState(Factory.HoldingStickyWall());
        }
        else if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
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
