using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [Tooltip("Fuerza de impulso del dash")]
    [SerializeField, Rename("Force")] private float _dashingForce = 24f;
    [Tooltip("Duracion del dash en segundos (tambien inhabilita los demas controles)")]
    [SerializeField, Rename("Duration (s)")] private float _dashingTime = 0.2f;
    [Tooltip("Tiempo de enfriamiento para volver a usar el dash en segundos")]
    [SerializeField, Rename("Cooldown (s)")] private float _dashingCooldown = 1f;

    private bool _canDash = true;

    private PlayerInput _input;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;


    private InputAction _dashAction;

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _input = gameObject.GetComponent<PlayerInput>();

        _dashAction = _input.actions["Dash"];
    }
    private void OnEnable()
    {
        _dashAction.performed += StartDash;
    }
    private void OnDisable()
    {
        _dashAction.performed -= StartDash;
    }

    private void StartDash(InputAction.CallbackContext obj)
    {
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Aiming) return;
        if (_canDash)
            StartCoroutine(Dash());
    }
    private IEnumerator Dash()
    {
        _canDash = false;
        PlayerStateManager.Instance.ChangeState(PlayerState.Dashing);
        var originalGravity = _rigidbody2D.gravityScale;
        _rigidbody2D.gravityScale = 0f;
        var dashDir = _spriteRenderer.flipX
            ? Vector2.right.x
            : Vector2.left.x;
        _rigidbody2D.velocity = new Vector2(dashDir * _dashingForce, 0f);

        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(_dashingTime);
        _trailRenderer.emitting = false;

        _rigidbody2D.gravityScale = originalGravity;
        PlayerStateManager.Instance.ChangeState(PlayerState.Default);

        yield return new WaitForSeconds(_dashingCooldown);
        _canDash = true;
    }
}
