using System.Collections;
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



    //-----Movement-----
    public static Vector2 Direction = Vector2.zero;
    private float _sprintMult = 1f;

    //-----Components-----
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _collider;
    private PlayerInput _input;

    //-----Inputs-----
    private InputAction _moveAction;
    private InputAction _runAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        _input = gameObject.GetComponent<PlayerInput>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();

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
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Dashing) return;
        if (PlayerStateManager.Instance.CurrentState == PlayerState.WallJumping) return;
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Aiming) return;
        if (PlayerStateManager.Instance.CurrentState == PlayerState.Executing) return;

        var dirX = Direction.x * _movementSpeed * _sprintMult * Time.fixedDeltaTime;

        if (isHittingNormalWall(Direction.normalized))
            _rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);
        else
            _rigidbody2D.velocity = new Vector2(dirX, _rigidbody2D.velocity.y);

        if (isHittingStickyWall(Direction.normalized))
            _rigidbody2D.velocity = Vector2.zero;
    }

    private bool isHittingNormalWall(Vector2 direction) => 
        Physics2D.BoxCast(_collider.bounds.center, new Vector2(_collider.bounds.size.x, _collider.bounds.size.y - 0.001f), 0f, direction, .1f, jumpableGround);
    private bool isHittingStickyWall(Vector2 direction) => 
        PlayerStateManager.Instance.CurrentState == PlayerState.OnStickyWall &&
        Physics2D.BoxCast(_collider.bounds.center, new Vector2(_collider.bounds.size.x, _collider.bounds.size.y - 0.001f), 0f, direction, .1f, jumpableWall);

    private void GetDirection(InputAction.CallbackContext ctx) => Direction = ctx.ReadValue<Vector2>();
    private void resetDirection(InputAction.CallbackContext ctx) => Direction = Vector2.zero;

    private void StartRunning(InputAction.CallbackContext ctx) => _sprintMult = _sprintMultiplier;
    private void StopRunning(InputAction.CallbackContext ctx) => _sprintMult = 1f;

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
        _rigidbody2D.velocity = new Vector2(Direction.normalized.x * -(_wallJumpForce), _jumpForce);
        yield return new WaitForSeconds(.15f);
        PlayerStateManager.Instance.ChangeState(PlayerState.Default);
    }
    private bool isGrounded() => PlayerSubStateManager.Instance.IsGrounded;

}
