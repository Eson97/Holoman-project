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
        Ctx.PlayerVisualAnimator.SetBool("isWalking", true);
        Ctx.StartRunningTime = 0f;
    }

    public override void ExitState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isWalking",false);
    }

    public override void UpdateState()
    {
        Ctx.StartRunningTime += Time.deltaTime;
        var dir = Ctx.PlayerController.MoveDirection.normalized;
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        var dirX = Ctx.PlayerController.MoveDirection.normalized.x;
        var VelocityX = dirX * Ctx.MovementSpeed * Ctx.RunSpeedMultiplier * Time.fixedDeltaTime;

        if(Ctx.CanMove)
            Ctx.Rigidbody.velocity = new Vector2(VelocityX, Ctx.Rigidbody.velocity.y);
        else
            Ctx.Rigidbody.velocity = new Vector2(0, Ctx.Rigidbody.velocity.y);
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.PlayerController.IsMoving)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.PlayerController.IsCrouchPressed && Ctx.PlayerController.IsMoving && Ctx.CanSlide)
        {
            SwitchState(Factory.Slide());
        }
        else if(Ctx.PlayerController.IsMoving && !Ctx.PlayerController.IsRunPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}
