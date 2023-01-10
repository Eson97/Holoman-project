using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento sin sprint")]
    [SerializeField, Rename("Speed")] private float _movementSpeed = 300f;
    [Tooltip("Multiplicador de velocidad al correr, formula: \n -> speed * multiplier = total_speed")]
    [SerializeField] private float _sprintMultiplier = 1.5f;

    [Header("Jump Settings")]
    [Tooltip("Fuerza de impulso al saltar")]
    [SerializeField, Rename("Force")] private float _jumpForce = 10f;
    [Tooltip("Fuerza de impulso al saltar en StickyWall (eje x)")]
    [SerializeField, Rename("Force on wall")] private float _wallJumpForce = 0.5f;
    [Tooltip("Suelo sobre el cual se puede saltar")]
    [SerializeField, Rename("Ground layout")] private LayerMask jumpableGround;
    [Tooltip("Pared sobre la cual se puede saltar")]
    [SerializeField, Rename("Sticky wall layout")] private LayerMask jumpableWall;

    [Header("Dash Settings")]
    [Tooltip("Fuerza de impulso del dash")]
    [SerializeField, Rename("Force")] private float _dashingForce = 24f;
    [Tooltip("Duracion del dash en segundos (tambien inhabilita los demas controles)")]
    [SerializeField, Rename("Duration (s)")] private float _dashingTime = 0.2f;
    [Tooltip("Tiempo de enfriamiento para volver a usar el dash en segundos")]
    [SerializeField,Rename("Cooldown (s)")]private float _dashingCooldown = 1f;


    //-----Movement-----
    private Vector2 _direction = Vector2.zero;
    private float _sprintMult = 1f;


    //-----Dash-----
    private bool _canDash = true;

    //-----Components-----
    private Rigidbody2D _rigidbody2D;
    private TrailRenderer _trailRenderer;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;
    private PlayerInput _input;
    private Animator _animator;

    //-----Inputs-----
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _input = gameObject.GetComponent<PlayerInput>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();

        _moveAction = _input.actions["Move"];
        _runAction = _input.actions["Run"];
        _jumpAction = _input.actions["Jump"];
        _dashAction = _input.actions["Dash"];
    }
    private void OnEnable()
    {
        _moveAction.performed += GetDirection;
        _moveAction.canceled += resetDirection;

        _runAction.performed += StartRunning;
        _runAction.canceled += StopRunning;

        _jumpAction.performed += StartJump;
        _jumpAction.canceled += StopJump;

        _dashAction.performed += StartDash;
    }
    private void OnDisable()
    {
        _moveAction.performed -= GetDirection;
        _moveAction.canceled -= resetDirection;

        _runAction.performed -= StartRunning;
        _runAction.canceled -= StopRunning;

        _jumpAction.performed -= StartJump;
        _jumpAction.canceled -= StopJump;

        _dashAction.performed -= StartDash;
    }

    private void Update()
    {
        var dirX = _rigidbody2D.velocity.x;

        if (dirX > 0.01f)
            _spriteRenderer.flipX = false;
        else if (dirX < -0.01f)
            _spriteRenderer.flipX = true;

        if (PlayerStateManager.Instance.CurrentState == PlayerState.Default)
            _animator.SetFloat("Speed", System.Math.Abs(dirX));
        else
            _animator.SetFloat("Speed", 0.001f);
    }

    private void FixedUpdate()
    {
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Dashing) return;
        if (PlayerStateManager.Instance.CurrentState == PlayerState.WallJumping) return;
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Aiming) return;
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Executing) return;

        var dirX = _direction.x * _movementSpeed * _sprintMult * Time.fixedDeltaTime;

        if (isHittingNormalWall(_direction.normalized))
            _rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
        else
            _rigidbody2D.velocity = new Vector2(dirX, _rigidbody2D.velocity.y);

        if (isHittingStickyWall(_direction.normalized))
            _rigidbody2D.velocity = Vector2.zero;
    }

    private bool isHittingNormalWall(Vector2 direction) => 
        Physics2D.BoxCast(_collider.bounds.center, new Vector2(_collider.bounds.size.x, _collider.bounds.size.y - 0.01f), 0f, direction, .1f, jumpableGround);
    private bool isHittingStickyWall(Vector2 direction) => 
        PlayerStateManager.Instance.CurrentState == PlayerState.OnStickyWall &&
        Physics2D.BoxCast(_collider.bounds.center, new Vector2(_collider.bounds.size.x, _collider.bounds.size.y - 0.01f), 0f, direction, .1f, jumpableWall);

    private void GetDirection(InputAction.CallbackContext ctx) => _direction = ctx.ReadValue<Vector2>();
    private void resetDirection(InputAction.CallbackContext ctx) => _direction = Vector2.zero;

    private void StartRunning(InputAction.CallbackContext ctx)
    {
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Dashing) return;
        _sprintMult = _sprintMultiplier;
    }
    private void StopRunning(InputAction.CallbackContext ctx)
    {
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Dashing) return;
        _sprintMult = 1f;
    }

    private void StartJump(InputAction.CallbackContext ctx)
    {
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Dashing) return;

        if(PlayerStateManager.Instance.CurrentState == PlayerState.OnStickyWall)
            StartCoroutine(WallJump());

        if (isGrounded() && PlayerStateManager.Instance.CurrentState == PlayerState.Default)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
    }
    private void StopJump(InputAction.CallbackContext ctx)
    {
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Dashing) return;


        if (_rigidbody2D.velocity.y > 0.001f)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
    }
    private IEnumerator WallJump()
    {
        PlayerStateManager.Instance.ChangeState(PlayerState.WallJumping);
        _rigidbody2D.velocity = new Vector2(_direction.normalized.x * -(_wallJumpForce), _jumpForce);
        yield return new WaitForSeconds(.15f);
        PlayerStateManager.Instance.ChangeState(PlayerState.Default);
    }
    private bool isGrounded()
    {
        //verifica si estamos parados sobre el suelo o no
        return Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
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
