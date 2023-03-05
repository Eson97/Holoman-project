using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoldingStickyWallState : PlayerBaseState, IRootState
{
    public PlayerHoldingStickyWallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isHoldingStickyWall", true);
        InitializeSubState();
    }

    public override void ExitState()
    {
        Ctx.PlayerVisualAnimator.SetBool("isHoldingStickyWall", false);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        Ctx.Rigidbody.velocity = Vector2.zero;
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsHoldingFromStickyWall)
        {
            SwitchState(Factory.Fall());
        }
        else if(Ctx.IsHoldingFromStickyWall && PlayerInputManager.Instance.IsJumpPressedThisFrame)
        {
            SwitchState(Factory.WallJump());
        }
    }

    public void InitializeSubState()
    {
        SetSubState(null);
    }
}
