using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private PlayerInput _input;
    private InputAction _attackAction;
    private InputAction _attackUpAction;
    private InputAction _attackDownAction;

    public Action AttackDelegate;
    public Action AttackUpDelegate;
    public Action AttackDownDelegate;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();

        _attackAction = _input.actions["Attack"];
        _attackUpAction = _input.actions["AttackUp"];
        _attackDownAction = _input.actions["AttackDown"];
    }
    private void OnEnable()
    {
        _attackAction.performed += Attack;
        _attackUpAction.performed += AttackUp;
        _attackDownAction.performed += AttackDown;
    }

    private void OnDisable()
    {
        _attackAction.performed -= Attack;
        _attackUpAction.performed -= AttackUp;
        _attackDownAction.performed -= AttackDown;
    }

    private void Attack(InputAction.CallbackContext ctx)
    {
        if (_attackUpAction.WasPerformedThisFrame() || _attackDownAction.WasPerformedThisFrame()) return;
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Default) return;
        Debug.Log("Basic");
        AttackDelegate?.Invoke();
    }
    private void AttackUp(InputAction.CallbackContext ctx)
    {
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Default) return;
        Debug.Log("Up");
        AttackUpDelegate?.Invoke();
    }
    private void AttackDown(InputAction.CallbackContext ctx)
    {
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Default) return;
        Debug.Log("Down");
        AttackDownDelegate?.Invoke();
    }
}
