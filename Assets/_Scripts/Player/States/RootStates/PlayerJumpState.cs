using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    private bool _jump;
    public bool _falling;

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isJumping", true);

        InitializeSubState();
        _jump = true;
        _falling = false;
    }

    public override void ExitState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isJumping", false);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        if(_jump)
        {
            Ctx.Rigidbody.velocity = new Vector2(Ctx.Rigidbody.velocity.x, Ctx.JumpForce);
            _jump = false;
        }
        if (Ctx.Rigidbody.velocity.y < 0f)
        {
            _falling = true;
        }
        if (!Ctx.PlayerController.IsJumpPressed)
        {
            Ctx.Rigidbody.velocity = new Vector2(Ctx.Rigidbody.velocity.x, 0f);
            _falling = true;
        }
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
        else if (_falling)
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
