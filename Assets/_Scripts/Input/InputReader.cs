using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameInput;
using static UnityEngine.InputSystem.InputAction;

[CreateAssetMenu(menuName = "InputReader")]
public class InputReader : ScriptableObject, IPlayerActions, IUIActions
{
    private GameInput _gameInput;

    public GameInput GameInput => _gameInput;

    #region Player Events
    public event Action<bool> AimEvent;
    public event Action<Vector2> AimDirectionEvent;

    public event Action AttackEvent;
    public event Action AttackUpEvent;
    public event Action AttackDownEvent;

    public event Action<bool> CrouchEvent;

    public event Action<bool> DashEvent;

    public event Action InteractEvent;

    public event Action<bool> JumpEvent;

    public event Action<Vector2> MoveEvent;
    public event Action<bool> RunEvent;

    public event Action PauseEvent;
    #endregion

    #region UI Events
    public event Action UnpauseEvent;
    #endregion


    private void OnEnable()
    {
        if (_gameInput != null) return;
        
        _gameInput = new GameInput();

        _gameInput.Player.SetCallbacks(this);
        _gameInput.UI.SetCallbacks(this);

        SetGameplay();
    }

    public void SetGameplay()
    {
        _gameInput.UI.Disable();
        _gameInput.Player.Enable();
    }
    public void SetUI()
    {
        _gameInput.Player.Disable();
        _gameInput.UI.Enable();
    }

    #region Player Actions
    public void OnAimMode(CallbackContext context)
    {
        AimEvent?.Invoke(context.phase == InputActionPhase.Performed);
    }

    public void OnAttack(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
            AttackEvent?.Invoke();
    }

    public void OnAttackDown(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
            AttackDownEvent?.Invoke();
    }

    public void OnAttackUp(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
            AttackUpEvent?.Invoke();
    }

    public void OnCrouch(CallbackContext context)
    {
        CrouchEvent?.Invoke(context.phase == InputActionPhase.Performed);
    }

    public void OnDash(CallbackContext context)
    {
        DashEvent?.Invoke(context.phase == InputActionPhase.Performed);
    }

    public void OnDirection(CallbackContext context)
    {
        AimDirectionEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnInteract(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
            InteractEvent?.Invoke();
    }

    public void OnJump(CallbackContext context)
    {
        JumpEvent?.Invoke(context.phase == InputActionPhase.Performed);
    }

    public void OnMove(CallbackContext context)
    {
        MoveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPause(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
            PauseEvent?.Invoke();
    }

    public void OnRun(CallbackContext context)
    {
        RunEvent?.Invoke(context.phase == InputActionPhase.Performed);
    }
    #endregion

    #region UI Actions
    public void OnUnpause(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
            UnpauseEvent?.Invoke();
    }
    #endregion
}
