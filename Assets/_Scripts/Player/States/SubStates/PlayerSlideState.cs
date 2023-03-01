using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    public PlayerSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
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
        if (PlayerInputManager.Instance.IsMoving && PlayerInputManager.Instance.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        else if (PlayerInputManager.Instance.IsMoving)
        {
            SwitchState(Factory.Walk());
        }
        else if (PlayerInputManager.Instance.IsCrouchPressed && !PlayerInputManager.Instance.IsMoving)
        {
            SwitchState(Factory.Crouch());
        }
        else if(!PlayerInputManager.Instance.IsMoving && !PlayerInputManager.Instance.IsCrouchPressed)
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
