using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _states;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states = new Dictionary<PlayerStates, PlayerBaseState>();

        _states.Add(PlayerStates.Idle,  new PlayerIdleState(_context, this, PlayerStates.Idle));
        _states.Add(PlayerStates.Walk,  new PlayerWalkState(_context, this, PlayerStates.Walk));
        _states.Add(PlayerStates.Run,   new PlayerRunState(_context, this, PlayerStates.Run));
        _states.Add(PlayerStates.Fall,  new PlayerFallState(_context, this, PlayerStates.Fall));
        _states.Add(PlayerStates.Jump,  new PlayerJumpState(_context, this, PlayerStates.Jump));
        _states.Add(PlayerStates.Grounded,  new PlayerGroundedState(_context, this, PlayerStates.Grounded));
        _states.Add(PlayerStates.Dashing,   new PlayerDashState(_context, this, PlayerStates.Dashing));
        _states.Add(PlayerStates.Crouch,    new PlayerCrouchState(_context, this, PlayerStates.Crouch));
        _states.Add(PlayerStates.Slide,     new PlayerSlideState(_context, this, PlayerStates.Slide));
        _states.Add(PlayerStates.WallJump,  new PlayerWallJumpState(_context, this, PlayerStates.WallJump));
        _states.Add(PlayerStates.HoldingStickyWall, new PlayerHoldingStickyWallState(_context, this, PlayerStates.HoldingStickyWall));
    }

    public PlayerBaseState Idle() => _states[PlayerStates.Idle];
    public PlayerBaseState Walk() => _states[PlayerStates.Walk];
    public PlayerBaseState Run() => _states[PlayerStates.Run];
    public PlayerBaseState Fall() => _states[PlayerStates.Fall];
    public PlayerBaseState Jump() => _states[PlayerStates.Jump];
    public PlayerBaseState Grounded() => _states[PlayerStates.Grounded];
    public PlayerBaseState Dashing() => _states[PlayerStates.Dashing];
    public PlayerBaseState Crouch() => _states[PlayerStates.Crouch];
    public PlayerBaseState Slide() => _states[PlayerStates.Slide];
    public PlayerBaseState HoldingStickyWall() => _states[PlayerStates.HoldingStickyWall];
    public PlayerBaseState WallJump() => _states[PlayerStates.WallJump];

}
public enum PlayerStates
{
    None,
    Idle,
    Walk,
    Run,
    Fall,
    Jump,
    Grounded,
    Dashing,
    Crouch,
    Slide,
    HoldingStickyWall,
    WallJump,

}
