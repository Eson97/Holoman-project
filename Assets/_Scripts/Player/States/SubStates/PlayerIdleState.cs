using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    private bool _enterIdle;
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
    }

    public override void EnterState()
    {
        _enterIdle = true;
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
        if (_enterIdle)
        {
            Ctx.Rigidbody.velocity = new Vector2(0f, Ctx.Rigidbody.velocity.y);
            _enterIdle =false;
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.PlayerController.IsMoving && Ctx.PlayerController.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        else if (Ctx.PlayerController.IsMoving && !Ctx.PlayerController.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.PlayerController.IsCrouchPressed && Ctx.CanCrouch)
        {
            SwitchState(Factory.Crouch());
        }
    }
}
