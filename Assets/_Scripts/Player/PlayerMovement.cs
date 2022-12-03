using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento sin sprint")]
    [SerializeField] private float _movementSpeed = 5f;
    [Tooltip("Multiplicador de velocidad al correr, formula: \n -> speed * multiplier = total_speed")]
    [SerializeField] private float _sprintMultiplier = 1.5f;

    [Header("Jump Settings")]
    [Tooltip("Fuerza de impulso al saltar")]
    [SerializeField] private float _jumpForce = 10f;
    [Tooltip("Superficie (Layout) sobre la cual se puede saltar")]
    [SerializeField] private LayerMask jumpableGround;

    //-----Movement-----
    private Vector2 _direction = Vector2.zero;
    private float _sprintMult = 1f;

    //-----Components-----
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;
    //private Animator _animator;

    //-----Inputs-----
    private PlayerInput _input;
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _input = gameObject.GetComponent<PlayerInput>();
        _collider = GetComponent<BoxCollider2D>();
        //_animator = GetComponent<Animator>();

        _moveAction = _input.actions["Move"];
        _runAction = _input.actions["Run"];
        _jumpAction = _input.actions["Jump"];
    }

    private void OnEnable()
    {
        _moveAction.performed += GetDirection;
        _moveAction.canceled += resetDirection;

        _runAction.performed += StartRunning;
        _runAction.canceled += StopRunning;

        _jumpAction.performed += StartJump;
        _jumpAction.canceled += StopJump;
    }

    private void OnDisable()
    {
        _moveAction.performed -= GetDirection;
        _moveAction.canceled -= resetDirection;

        _runAction.performed -= StartRunning;
        _runAction.canceled -= StopRunning;

        _jumpAction.performed -= StartJump;
        _jumpAction.canceled -= StopJump;
    }

    private void FixedUpdate()
    {
        var dirX = _direction.x * _movementSpeed * _sprintMult * Time.fixedDeltaTime;

        if (dirX > 0.01f)
            _spriteRenderer.flipX = true;
        else if (dirX < -0.01f)
            _spriteRenderer.flipX = false;

        _rigidbody2D.velocity = new Vector2(dirX, _rigidbody2D.velocity.y);
    }

    private void GetDirection(InputAction.CallbackContext ctx) => _direction = ctx.ReadValue<Vector2>();
    private void resetDirection(InputAction.CallbackContext ctx) => _direction = Vector2.zero;

    private void StartRunning(InputAction.CallbackContext ctx) => _sprintMult = _sprintMultiplier;
    private void StopRunning(InputAction.CallbackContext ctx) => _sprintMult = 1f;

    private void StartJump(InputAction.CallbackContext ctx)
    {
        if (isGrounded())
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
    }
    private void StopJump(InputAction.CallbackContext ctx)
    {
        if (_rigidbody2D.velocity.y > 0.001f)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
    }
    private bool isGrounded()
    {
        //verifica si estamos parados sobre el suelo o no
        return Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
