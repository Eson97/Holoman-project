using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerExecution : MonoBehaviour
{
    [Header("Aiming Settings")]
    [Tooltip("Velocidad del juego mientras se apunta (min: 0 -> freeze, max:1 -> nomral)")]
    [SerializeField, Rename("Slow down factor")] private float _slowFactor = 0.05f;


    private Vector2 _executionDir = Vector2.zero;
    private bool _canExec = true;

    private InputAction _execDirAction;
    private InputAction _aimModeAction;
    private PlayerInput _input;

    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();

        _execDirAction = _input.actions["Direction"];
        _aimModeAction = _input.actions["AimMode"];
    }

    private void OnEnable()
    {
        _execDirAction.performed += GetExecutionDir;
        _aimModeAction.performed += StartAimMode;

        _execDirAction.canceled += ResetExecutionDir;
        _aimModeAction.canceled += FinishAimMode;

    }
    private void OnDisable()
    {
        _execDirAction.performed -= GetExecutionDir;
        _aimModeAction.performed -= StartAimMode;

        _execDirAction.canceled -= ResetExecutionDir;
        _aimModeAction.canceled -= FinishAimMode;
    }

    private void Start()
    {
        PlayerStateManager.Instance.OnAiming += StartSlowMotion;
        PlayerStateManager.Instance.OnBeforeStateChanged += EndSlowMotion;
    }

    private void OnDrawGizmos()
    {
        if (PlayerStateManager.Instance == null) return;
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Aiming) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _executionDir * 5);
    }

    private void GetExecutionDir(InputAction.CallbackContext ctx) => _executionDir = ctx.ReadValue<Vector2>();
    private void ResetExecutionDir(InputAction.CallbackContext ctx) => _executionDir = Vector2.zero;

    private void StartAimMode(InputAction.CallbackContext ctx)
    {
        if (!_canExec) return;
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Default) return;
        PlayerStateManager.Instance.ChangeState(PlayerState.Aiming);
    }
    private void FinishAimMode(InputAction.CallbackContext ctx)
    {
        if (_executionDir != Vector2.zero)
            StartExecution();
        else
            PlayerStateManager.Instance.ChangeState(PlayerState.Default);
    }

    private void StartExecution()
    {
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Aiming) return;

        StartCoroutine(Exec());
    }

    private IEnumerator Exec()
    {
        PlayerStateManager.Instance.ChangeState(PlayerState.Executing);

        var originalGravity = _rigidbody2D.gravityScale;

        _rigidbody2D.velocity = _executionDir * 35;
        _rigidbody2D.gravityScale = 0f;

        yield return new WaitForSeconds(.15f);

        _rigidbody2D.gravityScale = originalGravity;
        _rigidbody2D.velocity = Vector2.zero;

        if(PlayerStateManager.Instance.CurrentState == PlayerState.Executing)
            PlayerStateManager.Instance.ChangeState(PlayerState.Default);
    }

    private void StartSlowMotion()
    {
        Time.timeScale = _slowFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    private void EndSlowMotion(PlayerState currentState)
    {
        if(currentState == PlayerState.Aiming)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}
