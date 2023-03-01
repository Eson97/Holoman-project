using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
    }

    public override void EnterState()
    {
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
        Ctx.Rigidbody.velocity = new Vector2(0f, Ctx.Rigidbody.velocity.y);
    }

    public override void CheckSwitchStates()
    {
        if (PlayerInputManager.Instance.IsMoving && PlayerInputManager.Instance.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        else if (PlayerInputManager.Instance.IsMoving)
        {
            SwitchState(Factory.Walk());
        }
        else if (PlayerInputManager.Instance.IsCrouchPressed && Ctx.CanCrouch)
        {
            SwitchState(Factory.Crouch());
        }
    }
}
