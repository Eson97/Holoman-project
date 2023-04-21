using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState, IRootState
{

    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type)
        : base(currentContext, playerStateFactory, type)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Ctx.StartCoroutine(WallJump());
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsHoldingFromStickyWall)
        {
            SwitchState(Factory.HoldingStickyWall());
        }
        else if (Ctx.PlayerController.IsDashPressed)
        {
            SwitchState(Factory.Dashing());
        }
        else if (!Ctx.IsHoldingFromStickyWall && !Ctx.IsGrounded)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public void InitializeSubState()
    {
        SetSubState(null);
    }

    private IEnumerator WallJump()
    {
        var dirX = Ctx.PlayerController.MoveDirection.normalized.x;
        Ctx.Rigidbody.velocity = new Vector2(dirX * -(Ctx.WallJumpForce), Ctx.JumpForce);

        yield return new WaitForSeconds(Ctx.WallJumpDuration);
        
        CheckSwitchStates();
    }
}
