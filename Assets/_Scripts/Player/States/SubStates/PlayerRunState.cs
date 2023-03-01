using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
    }

    public override void EnterState()
    {
        Ctx.StartRunningTime = 0f;
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        Ctx.StartRunningTime += Time.deltaTime;
        var dir = PlayerInputManager.Instance.CurrentMovementInput.normalized;
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        var dirX = PlayerInputManager.Instance.CurrentMovementInput.x;
        var VelocityX = dirX * Ctx.MovementSpeed * Ctx.RunSpeedMultiplier * Time.fixedDeltaTime;

        if(Ctx.CanMove)
            Ctx.Rigidbody.velocity = new Vector2(VelocityX, Ctx.Rigidbody.velocity.y);
        else
            Ctx.Rigidbody.velocity = new Vector2(0, Ctx.Rigidbody.velocity.y);
    }

    public override void CheckSwitchStates()
    {
        if (!PlayerInputManager.Instance.IsMoving)
        {
            SwitchState(Factory.Idle());
        }
        else if (PlayerInputManager.Instance.IsCrouchPressed && PlayerInputManager.Instance.IsMoving && Ctx.CanSlide)
        {
            SwitchState(Factory.Slide());
        }
        else if(PlayerInputManager.Instance.IsMoving && !PlayerInputManager.Instance.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}
