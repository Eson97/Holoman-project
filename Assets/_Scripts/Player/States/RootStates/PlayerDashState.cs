using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBaseState, IRootState
{
    public PlayerDashState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory, PlayerStates type) 
        : base(currentContext, playerStateFactory, type)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Ctx.StartCoroutine(Dash());
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
        if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsGrounded /* is touching a stickyWall */)
        {
            SwitchState(Factory.Fall());
        }
        //else: is falling
    }

    public void InitializeSubState()
    {
        SetSubState(null);
    }

    private IEnumerator Dash()
    {
        Ctx.CanDash = false;

        var originalGravity = Ctx.Rigidbody.gravityScale;
        Ctx.Rigidbody.gravityScale = 0f;

        var dashDir = Ctx.IsFlipped
            ? Vector2.left
            : Vector2.right;

        Ctx.Rigidbody.velocity = new Vector2(dashDir.x * Ctx.DashingForce, 0f);

        //_trailRenderer.emitting = true;
        
        yield return new WaitForSeconds(Ctx.DashingTime);
        
        //_trailRenderer.emitting = false;
        Ctx.Rigidbody.gravityScale = originalGravity;
        CheckSwitchStates();

        yield return new WaitForSeconds(Ctx.DashingCooldown);
        Ctx.CanDash = true;
    }
}
