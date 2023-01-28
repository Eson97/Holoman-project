using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        var dirX = _rigidbody2D.velocity.normalized.x;

        if (dirX > 0.01f)
            _spriteRenderer.flipX = false;
        else if (dirX < -0.01f)
            _spriteRenderer.flipX = true;


        if (PlayerStateManager.Instance.CurrentState == PlayerState.Default)
            HandleDefaultState(PlayerSubStateManager.Instance.CurrentSubState);
        if (PlayerStateManager.Instance.CurrentState == PlayerState.OnStickyWall)
            HandleOnStickyWall();
        if (PlayerStateManager.Instance.CurrentState == PlayerState.WallJumping)
            HandleWallJump();

    }

    private void HandleWallJump()
    {
        _animator.SetBool("OnStickyWall", false); 
        _animator.SetFloat("SpeedY", 1f);

    }

    private void HandleOnStickyWall()
    {
        _animator.SetBool("OnStickyWall", true);
    }

    private void HandleDefaultState(PlayerSubState playerSubState)
    {
        switch (playerSubState)
        {
            case PlayerSubState.Idle:
                _animator.SetFloat("Speed", 0f);
                _animator.SetFloat("SpeedY", 0f);
                break;
            case PlayerSubState.Running:
                _animator.SetFloat("Speed", 1f);
                _animator.SetFloat("SpeedY", 0f);
                break;
            case PlayerSubState.Jumping:
                _animator.SetFloat("Speed", 0f);
                _animator.SetFloat("SpeedY", 1f);
                break;
            case PlayerSubState.Falling:
                _animator.SetFloat("Speed", 0f);
                _animator.SetFloat("SpeedY", -1f);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(playerSubState), playerSubState, null);
        }
     
        _animator.SetBool("OnStickyWall", false);
    }




}
