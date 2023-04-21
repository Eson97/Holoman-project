using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    public PlayerSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
    }

    public override void EnterState()
    {
        Ctx.PlayerVisual.transform.localScale = new Vector3(1f, 0.5f, 1f);
        Ctx.StartCoroutine(Slide());
    }

    public override void ExitState()
    {
        Ctx.PlayerVisual.transform.localScale = Vector3.one;
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        var dir = Ctx.IsFlipped
            ? Vector2.left
            : Vector2.right;

        var VelocityX = dir.x * Ctx.MovementSpeed * Ctx.RunSpeedMultiplier * Time.fixedDeltaTime;

        Ctx.Rigidbody.velocity = new Vector2(VelocityX, Ctx.Rigidbody.velocity.y);
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
        else if(!Ctx.PlayerController.IsMoving && !Ctx.PlayerController.IsCrouchPressed)
        {
            SwitchState(Factory.Idle());
        }
    }
    
    IEnumerator Slide()
    {
        yield return new WaitForSeconds(Ctx.SlideDuration);
        CheckSwitchStates();
    }
}
