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

        _states.Add(PlayerStates.Idle,  new PlayerIdleState(_context, this));
        _states.Add(PlayerStates.Walk,  new PlayerWalkState(_context, this));
        _states.Add(PlayerStates.Run,   new PlayerRunState(_context, this));
        _states.Add(PlayerStates.Fall,  new PlayerFallState(_context, this));
        _states.Add(PlayerStates.Jump,  new PlayerJumpState(_context, this));
        _states.Add(PlayerStates.Grounded, new PlayerGroundedState(_context, this));
        _states.Add(PlayerStates.Dashing, new PlayerDashState(_context, this));
    }

    public PlayerBaseState Idle() => _states[PlayerStates.Idle];
    public PlayerBaseState Walk() => _states[PlayerStates.Walk];
    public PlayerBaseState Run() => _states[PlayerStates.Run];
    public PlayerBaseState Fall() => _states[PlayerStates.Fall];
    public PlayerBaseState Jump() => _states[PlayerStates.Jump];
    public PlayerBaseState Grounded() => _states[PlayerStates.Grounded];
    public PlayerBaseState Dashing() => _states[PlayerStates.Dashing];

}
enum PlayerStates
{
    Idle,
    Walk,
    Run,
    Fall,
    Jump,
    Grounded,
    Dashing,
}
