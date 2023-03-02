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
        PlayerInputManager.Instance.OnJumpCanceledDelegate += PlayerInputManager_OnJumpCanceledDelegate;
    }

    public override void EnterState()
    {
        InitializeSubState();
        _jump = true;
        _falling = false;
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
        if(_jump)
        {
            Ctx.Rigidbody.velocity = new Vector2(Ctx.Rigidbody.velocity.x, Ctx.JumpForce);
            _jump = false;
        }
        if (Ctx.Rigidbody.velocity.y < 0f)
        {
            _falling = true;
        }
    }

    public override void CheckSwitchStates()
    {
        if (PlayerInputManager.Instance.IsDashPressed && Ctx.CanDash)
        {
            SwitchState(Factory.Dashing());
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
        if (!PlayerInputManager.Instance.IsMoving && !PlayerInputManager.Instance.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (PlayerInputManager.Instance.IsMoving && !PlayerInputManager.Instance.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    private void PlayerInputManager_OnJumpCanceledDelegate()
    {
        Ctx.Rigidbody.velocity = new Vector2(Ctx.Rigidbody.velocity.x, 0f);
        _falling = true;
    }
}
