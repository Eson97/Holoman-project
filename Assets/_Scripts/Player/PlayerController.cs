using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    public Vector2 MoveDirection { get; private set; }
    public Vector2 LastMoveDirection { get; private set; }
    
    public Vector2 AimDirection { get; private set; }
    public Vector2 LastAimDirection { get; private set; }

    public bool IsMoving => MoveDirection != Vector2.zero;
    public bool IsAimPressed { get; private set; }
    public bool IsCrouchPressed { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsJumpPressedThisFrame => _inputReader.GameInput?.Player.Jump.WasPerformedThisFrame() ?? false;
    public bool IsRunPressed { get; private set; }
    public bool IsDashPressed { get; private set; }

    private void Awake()
    {
        _inputReader.AimEvent += InputReader_AimEvent;
        _inputReader.AimDirectionEvent += InputReader_AimDirectionEvent;

        _inputReader.AttackEvent += InputReader_AttackEvent;
        _inputReader.AttackUpEvent += InputReader_AttackUpEvent;
        _inputReader.AttackDownEvent += InputReader_AttackDownEvent;

        _inputReader.InteractEvent += InputReader_InteractEvent;

        _inputReader.CrouchEvent += InputReader_CrouchEvent;

        _inputReader.DashEvent += InputReader_DashEvent;

        _inputReader.JumpEvent += InputReader_JumpEvent;

        _inputReader.MoveEvent += InputReader_MoveEvent;
        _inputReader.RunEvent += InputReader_RunEvent;
    }

    private void InputReader_AttackEvent()
    {
        Debug.Log("Attack!");
    }
    private void InputReader_AttackDownEvent()
    {
        Debug.Log("AttackDown!");
    }
    private void InputReader_AttackUpEvent()
    {
        Debug.Log("AttackUp");
    }

    private void InputReader_InteractEvent()
    {
        Debug.Log("Interact!");
    }

    private void InputReader_MoveEvent(Vector2 dir)
    {
        if (dir != Vector2.zero)
            LastMoveDirection = dir;

        MoveDirection = dir;

    }
    private void InputReader_RunEvent(bool isPressed)
    {
        IsRunPressed = isPressed;
    }

    private void InputReader_AimEvent(bool isPressed)
    {
        IsAimPressed = isPressed;
    }
    private void InputReader_AimDirectionEvent(Vector2 dir)
    {
        if(dir != Vector2.zero)
            LastAimDirection = dir;

        AimDirection = dir;
    }

    private void InputReader_JumpEvent(bool isPressed)
    {
        IsJumpPressed = isPressed;
    }
    private void InputReader_DashEvent(bool isPressed)
    {
        IsDashPressed = isPressed;
    }
    private void InputReader_CrouchEvent(bool isPressed)
    {
        IsCrouchPressed = isPressed;
    }

}
