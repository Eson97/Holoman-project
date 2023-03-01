using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    private PlayerInputActions _playerInputActions;

    private Vector2 _currentMovementInput;
    private bool _isJumpPressed;
    private bool _isRunPressed;
    private bool _isDashPressed;

    public bool IsJumpPressed => _isJumpPressed;
    public bool IsRunPressed => _isRunPressed;
    public bool IsDashPressed => _isDashPressed;
    public bool IsMoving => _currentMovementInput != Vector2.zero;
    public Vector2 CurrentMovementInput => _currentMovementInput;

    public event Action OnJumpCanceledDelegate;

    protected override void Awake()
    {
        base.Awake();
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Move.started += onMove;
        _playerInputActions.Player.Move.performed += onMove;
        _playerInputActions.Player.Move.canceled += onMove;

        _playerInputActions.Player.Run.started += onRun;
        _playerInputActions.Player.Run.canceled += onRun;

        _playerInputActions.Player.Jump.started += onJumpPressed;
        _playerInputActions.Player.Jump.canceled += onJumpCanceled;

        _playerInputActions.Player.Dash.started += onDash;
        _playerInputActions.Player.Dash.canceled += onDash;
    }

    private void onDash(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => _isDashPressed = ctx.ReadValueAsButton();

    private void onJumpPressed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => _isJumpPressed = ctx.ReadValueAsButton();
    private void onJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        _isJumpPressed = ctx.ReadValueAsButton();
        OnJumpCanceledDelegate?.Invoke();
    }

    private void onRun(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => _isRunPressed = ctx.ReadValueAsButton();

    private void onMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => _currentMovementInput = ctx.ReadValue<Vector2>();
}
