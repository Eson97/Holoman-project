using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Velocidad de movimiento normal")]
    [SerializeField,Rename("Speed")] private float _movementSpeed = 550f;
    [Tooltip("Multiplicador de velocidad al correr")]
    [SerializeField,Rename("Sprint multiplier")] private float _runSpeedMultiplier = 1.2f;
    [Tooltip("Layers de suelo")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("Jump Settings")]
    [Tooltip("Fuerza de salto")]
    [SerializeField,Rename("Force")] private float _jumpForce = 22f;
    [Tooltip("Layers sobre los cuales se puede saltar")]
    [SerializeField] private LayerMask _jumpableGroundlayer;
    
    [Header("Dash Settings")]
    [Tooltip("Fuerza del dash (afecta la distancia)")]
    [SerializeField,Rename("Force")] private float _dashingForce = 24f;
    [Tooltip("Duracion del dash")]
    [SerializeField,Rename("Duration")] private float _dashingTime = 0.2f;
    [Tooltip("Tiempo de enfriamiento")]
    [SerializeField,Rename("Cooldown")] private float _dashingCooldown = 1f;
    
    [Header("Slide/Crouch Settings")]
    [Tooltip("Afecta el tiempo en que no puedes moverte mientras esta el slide")]
    [SerializeField,Rename("Duration")] private float _slideDuration = 1f;
    [Tooltip("Tiempo corriendo requerido para poder realizar el slide")]
    [SerializeField] private float _timeToSlide = 3f;
    
    [Header("Player General Settings")]
    [Tooltip("Visual en uso del personaje")]
    [SerializeField] private GameObject _playerVisual;

    private const float BOX_CAST_ANGLE = 0f;
    private const float BOX_CAST_DISTANCE = .1f;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _playerVisualSprite;

    private bool _isGrounded;
    private bool _canMove;
    private bool _canSlide;
    private bool _canJump;

    PlayerBaseState _currentState;
    PlayerStateFactory _stateFactory;

    //Components
    public Rigidbody2D Rigidbody => _rigidbody;
    public Collider2D Collider => _collider;
    public GameObject PlayerVisual => _playerVisual;
    public SpriteRenderer PlayerVisualSprite => _playerVisualSprite;
    public bool IsFlipped => PlayerVisualSprite.flipX;
    
    //Movement
    public float MovementSpeed => _movementSpeed;
    public float RunSpeedMultiplier => _runSpeedMultiplier;
    public bool CanMove => _canMove;
    public LayerMask GroundLayout => _groundLayer;
    
    //Jump
    public float JumpForce => _jumpForce;
    public bool IsGrounded => _isGrounded;
    public bool CanJump => _canJump;
    public LayerMask JumpableGround => _jumpableGroundlayer;
    
    //Dash
    public float DashingForce => _dashingForce;
    public float DashingTime => _dashingTime;
    public float DashingCooldown => _dashingCooldown;
    public bool CanDash { get; set; } = true;

    //Slide / crouch
    public float SlideDuration => _slideDuration;
    public float TimeToSlide => _timeToSlide;
    public bool CanSlide => _canSlide;
    public bool CanCrouch => CurrentState == _stateFactory.Grounded();
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

        _canMove = !Physics2D.BoxCast(_collider.bounds.center, canMoveBoxSize, BOX_CAST_ANGLE, dir, BOX_CAST_DISTANCE, _jumpableGroundlayer);
        _isGrounded = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, BOX_CAST_ANGLE, Vector2.down, BOX_CAST_DISTANCE, _groundLayer);
        _canJump = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, BOX_CAST_ANGLE, Vector2.down, BOX_CAST_DISTANCE, _jumpableGroundlayer);


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

        if (_currentState != _stateFactory.Grounded())
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
