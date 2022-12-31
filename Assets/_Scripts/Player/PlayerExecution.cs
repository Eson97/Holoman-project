using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerExecution : MonoBehaviour
{
    [Header("Aiming Settings")]
    [Tooltip("Velocidad de caida mientras se apunta (min: 0, max: 1")]
    [SerializeField, Rename("Falling Speed")] private float _aimingFallSpeed = 0.1f;
    [Tooltip("Velocidad en X mantenida tras apuntar (min: 0, max: 1)")]
    [SerializeField, Rename("Momentum on X")] private float _Momentum = 0.1f;

    private Vector2 _executionDir = Vector2.zero;
    private bool _canExec = true;
    private float _originalGravity;
    private int _execCount = 0;

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

    private void Start() => _originalGravity = _rigidbody2D.gravityScale;

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

        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x * _Momentum, 0.01f);
        _rigidbody2D.gravityScale = _aimingFallSpeed;
    }
    private void FinishAimMode(InputAction.CallbackContext ctx)
    {
        if (_executionDir != Vector2.zero)
            StartExecution();
        else
        {
            _rigidbody2D.gravityScale = _originalGravity;
            PlayerStateManager.Instance.ChangeState(PlayerState.Default);
        }
    }

    private void StartExecution()
    {
        if (PlayerStateManager.Instance.CurrentState != PlayerState.Aiming) return;
        
        PlayerStateManager.Instance.ChangeState(PlayerState.Executing);
        _execCount++;
        StartCoroutine(Exec());
    }

    //private IEnumerator Exec()
    //{
    //    _rigidbody2D.velocity = _executionDir * 35;

    //    yield return new WaitForSeconds(.15f);

    //    _rigidbody2D.gravityScale = _originalGravity;
    //    _rigidbody2D.velocity = Vector2.zero;

    //    PlayerStateManager.Instance.ChangeState(PlayerState.Default);
    //}

    private IEnumerator Exec()
    {
        _rigidbody2D.velocity = _executionDir * 35;

        yield return new WaitForSeconds(.15f);
        
        _rigidbody2D.velocity = Vector2.zero;

        if (_execCount >= 4)
        {
            _execCount = 0;
            _rigidbody2D.gravityScale = _originalGravity;
            PlayerStateManager.Instance.ChangeState(PlayerState.Default);
        }
        else
            PlayerStateManager.Instance.ChangeState(PlayerState.Aiming);
                
        StartExecution();

    }

}
