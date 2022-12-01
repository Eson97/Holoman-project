using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private LayerMask jumpableGround;

    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _collider;

    private InputAction _jumpAction;
    private PlayerInput _input;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _input = GetComponent<PlayerInput>();

        _jumpAction = _input.actions["Jump"];
    }

    private void OnEnable()
    {
        _jumpAction.performed += StartJump;
        _jumpAction.canceled += StopJump;
    }

    private void OnDisable()
    {
        _jumpAction.performed -= StartJump;
        _jumpAction.canceled -= StopJump;
    }

    void StartJump(InputAction.CallbackContext ctx)
    {
        if (isGrounded())
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
    }

    void StopJump(InputAction.CallbackContext ctx) 
    {
        if (_rigidbody2D.velocity.y>0.001f)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
    }
    private bool isGrounded()
    {
        //verifica si estamos parados sobre el suelo o no
        return Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
