using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSubStateManager : Singleton<PlayerSubStateManager>
{
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask NonJumplableGround;

    public PlayerSubState CurrentSubState { get; private set; }
    public bool IsGrounded { get; private set; }

    private void Start() => ChangueSubState(PlayerSubState.Idle);

    private void Update()
    {
        var dirX = _rigidbody2D.velocity.normalized.x;
        var dirY = _rigidbody2D.velocity.normalized.y;

        if (isGrounded())
        {
            if(dirX == 0)
                ChangueSubState(PlayerSubState.Idle);
            else
                ChangueSubState(PlayerSubState.Running);
        }
        else
        {
            if(dirY > 0.01f)
                ChangueSubState(PlayerSubState.Jumping);
            else if(dirY < 0.01f)
                ChangueSubState(PlayerSubState.Falling);
        }  
    }

    private bool isGrounded()
    {
        IsGrounded = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        var aux = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0f, Vector2.down, .1f, NonJumplableGround);
        return IsGrounded || aux;
    }

    private void ChangueSubState(PlayerSubState newPlayerSubState) => CurrentSubState = newPlayerSubState;
}

public enum PlayerSubState
{
    Idle,
    Running,
    Jumping,
    Falling,
}
