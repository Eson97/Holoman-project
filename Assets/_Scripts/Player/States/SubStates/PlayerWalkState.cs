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
        var dirX = PlayerInputManager.Instance.CurrentMovementInput.x;
        var VelocityX = dirX * Ctx.MovementSpeed * Time.fixedDeltaTime;

        if(Ctx.CanMove)
            Ctx.Rigidbody.velocity = new Vector2(VelocityX, Ctx.Rigidbody.velocity.y);
        else
            Ctx.Rigidbody.velocity = new Vector2(0, Ctx.Rigidbody.velocity.y);
    }

    public override void CheckSwitchStates()
    {
        if(PlayerInputManager.Instance.IsMoving && PlayerInputManager.Instance.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        else if (!PlayerInputManager.Instance.IsMoving && PlayerInputManager.Instance.IsCrouchPressed && Ctx.CanCrouch)
        {
            SwitchState(Factory.Crouch());
        }
        else if (!PlayerInputManager.Instance.IsMoving)
        {
            SwitchState(Factory.Idle());
        }
    }
}
