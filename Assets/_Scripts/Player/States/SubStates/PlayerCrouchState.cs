using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public PlayerCrouchState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
    }

    public override void EnterState()
    {
        Ctx.PlayerVisual.transform.localScale = new Vector3(1f, 0.5f, 1f);
    }

    public override void ExitState()
    {
        Ctx.PlayerVisual.transform.localScale = Vector3.one;
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
        else if (!PlayerInputManager.Instance.IsCrouchPressed)
        {
            SwitchState(Factory.Idle());
        }
    }
}