using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : Singleton<PlayerStateManager>
{
    public PlayerState CurrentState { get; private set; }

    public Action OnBeforeStateChanged;
    public Action OnAfterStateChanged;

    public Action OnDefault;
    public Action OnDashing;
    public Action OnWallJumping;
    public Action OnOnStickyWall;
    public Action OnAiming;
    public Action OnExecuting;


    private void Start() => ChangeState(PlayerState.Default);

    public void ChangeState(PlayerState newPlayerState)
    {
        OnBeforeStateChanged?.Invoke();
        
        CurrentState = newPlayerState;
        
        switch (newPlayerState)
        {
            case PlayerState.Default:
                HandleDefault();
                break;
            case PlayerState.Dashing:
                HandleDashing();
                break;
            case PlayerState.WallJumping:
                HandleWallJumping();
                break;
            case PlayerState.OnStickyWall:
                HandleOnStickyWall();
                break;
            case PlayerState.Aiming:
                HandleAiming();
                break;
            case PlayerState.Executing:
                HandleExecuting();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newPlayerState), newPlayerState, null);
        }
        
        OnAfterStateChanged?.Invoke();
    }


    private void HandleDefault()
    {
        Debug.Log("State is now Default");
        OnDefault?.Invoke();
    }

    private void HandleDashing()
    {
        Debug.Log("State is now Dashing");
        OnDashing?.Invoke();
    }


    private void HandleWallJumping()
    {
        Debug.Log("State is now Wall Jumping");
        OnWallJumping?.Invoke();
    }

    private void HandleOnStickyWall()
    {
        Debug.Log("State is now On Sticky Wall");
        OnOnStickyWall?.Invoke();
    }

    private void HandleAiming()
    {
        Debug.Log("State is now Aiming");
        OnAiming?.Invoke();
    }
    private void HandleExecuting()
    {
        Debug.Log("State is now Executing");
        OnExecuting?.Invoke();
    }
}

public enum PlayerState
{
    Default,
    Dashing,
    WallJumping,
    OnStickyWall,
    Aiming,
    Executing,
}
