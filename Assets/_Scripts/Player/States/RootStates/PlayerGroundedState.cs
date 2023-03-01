using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        : base(currentContext, playerStateFactory)
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
        if (PlayerInputManager.Instance.IsDashPressed && Ctx.CanDash)
        {
            SwitchState(Factory.Dashing());
        }
        else if (PlayerInputManager.Instance.IsJumpPressed && Ctx.CanJump)
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
        if(!PlayerInputManager.Instance.IsMoving && !PlayerInputManager.Instance.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if(PlayerInputManager.Instance.IsMoving && !PlayerInputManager.Instance.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

}
