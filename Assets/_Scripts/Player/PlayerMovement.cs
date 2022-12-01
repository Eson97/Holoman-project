using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _sprintMultiplier = 1.5f;

    private Vector2 _direction = Vector2.zero;
    private float _sprintMult = 1f;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    //private Animator _animator;

    private InputAction _moveAction;
    private InputAction _runAction;
    private PlayerInput _input;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _input = gameObject.GetComponent<PlayerInput>();
        //_animator = GetComponent<Animator>();

        _moveAction = _input.actions["Move"];
        _runAction = _input.actions["Run"];
    }

    private void OnEnable()
    {
        _moveAction.performed += GetDirection;
        _moveAction.canceled += resetDirection;

        _runAction.performed += StartRunning;
        _runAction.canceled += StopRunning;
    }

    private void OnDisable()
    {
        _moveAction.performed -= GetDirection;
        _moveAction.canceled -= resetDirection;

        _runAction.performed -= StartRunning;
        _runAction.canceled -= StopRunning;
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
}
