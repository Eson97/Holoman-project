using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 550f;
    [SerializeField] private float _runSpeedMultiplier = 1.2f;
    [SerializeField] private float _jumpForce = 22f;
    [SerializeField] private float _dashingForce = 24f;
    [SerializeField] private float _dashingTime = 0.2f;
    [SerializeField] private float _dashingCooldown = 1f;
    [SerializeField] private float _slideDuration = 1f;
    [SerializeField] private float _timeToSlide = 3f;
    [SerializeField] private LayerMask _jumpableGround;
    [SerializeField] private GameObject _playerVisual;

    private const float BOX_CAST_ANGLE = 0f;
    private const float BOX_CAST_DISTANCE = .1f;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _playerVisualSprite;

    private bool _isGrounded;
    private bool _canMove;
    private bool _canSlide;
    private bool _timeRunning;

    PlayerBaseState _currentState;
    PlayerStateFactory _stateFactory;

    //Components
    public Rigidbody2D Rigidbody => _rigidbody;
    public Collider2D Collider => _collider;
    public GameObject PlayerVisual => _playerVisual;
    public SpriteRenderer PlayerVisualSprite => _playerVisualSprite;
    
    //Movement
    public float MovementSpeed => _movementSpeed;
    public float RunSpeedMultiplier => _runSpeedMultiplier;
    public bool CanMove => _canMove;
    
    //Jump
    public float JumpForce => _jumpForce;
    public bool IsGrounded => _isGrounded;
    public LayerMask JumpableGround => _jumpableGround;
    
    //Dash
    public float DashingForce => _dashingForce;
    public float DashingTime => _dashingTime;
    public float DashingCooldown => _dashingCooldown;
    public bool CanDash { get; set; } = true;

    //Slide
    public float SlideDuration => _slideDuration;
    public float TimeToSlide => _timeToSlide;
    public bool CanSlide => _canSlide;
    public float StartRunningTime { get; set; }

    //Others
    public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _playerVisualSprite = _playerVisual.GetComponent<SpriteRenderer>();

    }
    private void Start()
    {
        _stateFactory = new PlayerStateFactory(this);
        _currentState = _stateFactory.Grounded();
        _currentState.EnterState();
    }
    private void Update()
    {
        var dir = PlayerInputManager.Instance.CurrentMovementInput.normalized * Vector2.right;
        var canMoveBoxSize = new Vector2(_collider.bounds.size.x, _collider.bounds.size.y - 0.001f);

        _canMove = !Physics2D.BoxCast(_collider.bounds.center, canMoveBoxSize, BOX_CAST_ANGLE, dir, BOX_CAST_DISTANCE, _jumpableGround);
        _isGrounded = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, BOX_CAST_ANGLE, Vector2.down, BOX_CAST_DISTANCE, _jumpableGround);

        handleVisualSpriteFlip(dir);
        handleTimeToSlide();


        Debug.Log($"<color=teal>RootState</color> {_currentState} | <color=Red>SubState</color> {_currentState.SubState?.ToString() ?? "None"}");
        _currentState.UpdateStates();
    }
    private void FixedUpdate()
    {
        _currentState.FixedUpdateStates();
    }

    void handleTimeToSlide()
    {
        if(StartRunningTime > _timeToSlide)
            _canSlide = true;
        else
            _canSlide = false;
    }
    void handleVisualSpriteFlip(Vector2 dir)
    {
        if (dir.x < -0.01f)
            _playerVisualSprite.flipX = true;
        else if (dir.x > 0.01f)
            _playerVisualSprite.flipX = false;
    }
}
