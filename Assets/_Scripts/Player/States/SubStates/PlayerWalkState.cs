using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
    }

    public override void EnterState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isWalking",true);
    }

    public override void ExitState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isWalking", false);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        var dirX = Ctx.PlayerController.MoveDirection.x;
        var VelocityX = dirX * Ctx.MovementSpeed * Time.fixedDeltaTime;

        if(Ctx.CanMove)
            Ctx.Rigidbody.velocity = new Vector2(VelocityX, Ctx.Rigidbody.velocity.y);
        else
            Ctx.Rigidbody.velocity = new Vector2(0, Ctx.Rigidbody.velocity.y);
    }

    public override void CheckSwitchStates()
    {
        if(Ctx.PlayerController.IsMoving && Ctx.PlayerController.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        else if (!Ctx.PlayerController.IsMoving && Ctx.PlayerController.IsCrouchPressed && Ctx.CanCrouch)
        {
            SwitchState(Factory.Crouch());
        }
        else if (!Ctx.PlayerController.IsMoving)
        {
            SwitchState(Factory.Idle());
        }
    }
}
