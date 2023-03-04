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
    [SerializeField] private LayerMask _jumpableGroundLayer;
    
    [Header("Dash Settings")]
    [Tooltip("Fuerza del dash (afecta la distancia)")]
    [SerializeField,Rename("Force")] private float _dashingForce = 24f;
    [Tooltip("Duracion del dash")]
    [SerializeField,Rename("Duration")] private float _dashingTime = 0.2f;
    [Tooltip("Tiempo de enfriamiento")]
    [SerializeField,Rename("Cooldown")] private float _dashingCooldown = 1f;
    
    [Header("Slide/Crouch Settings")]
    [Tooltip("Afecta el tiempo en que no puedes moverte mientras esta el slide")]
    [SerializeField] private float _slideDuration = 1f;
    [Tooltip("Tiempo corriendo requerido para poder realizar el slide")]
    [SerializeField] private float _timeToSlide = 3f;
    
    [Header("Player General Settings")]
    [Tooltip("Visual en uso del personaje")]
    [SerializeField] private GameObject _playerVisual;


    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _playerVisualSprite;
    private Animator _playerVisualAnimator;

    private bool _isGrounded;
    private bool _canMove;
    private bool _canSlide;
    private bool _canJump;

    private PlayerBaseState _currentState;
    private PlayerStateFactory _stateFactory;

    //Components
    public Rigidbody2D Rigidbody => _rigidbody;
    public Collider2D Collider => _collider;
    public GameObject PlayerVisual => _playerVisual;
    public SpriteRenderer PlayerVisualSprite => _playerVisualSprite;
    public Animator PlayerVisualAnimator => _playerVisualAnimator;
    public bool IsFlipped => PlayerVisualSprite?.flipX ?? false;
    
    //Movement
    public float MovementSpeed => _movementSpeed;
    public float RunSpeedMultiplier => _runSpeedMultiplier;
    public bool CanMove => _canMove;
    public LayerMask GroundLayout => _groundLayer;
    
    //Jump
    public float JumpForce => _jumpForce;
    public bool IsGrounded => _isGrounded;
    public bool CanJump => _canJump;
    public LayerMask JumpableGround => _jumpableGroundLayer;
    
    //Dash
    public float DashingForce => _dashingForce;
    public float DashingTime => _dashingTime;
    public float DashingCooldown => _dashingCooldown;
    public bool CanDash { get; set; } = true;

    //Slide / crouch
    public float SlideDuration => _slideDuration;
    public float TimeToSlide => _timeToSlide;
    public bool CanSlide => _canSlide;
    public bool CanCrouch => _currentState.Type == PlayerStates.Grounded && _currentState.SubState.Type == PlayerStates.Idle;
    public float StartRunningTime { get; set; }

    //States
    public PlayerBaseState CurrentState => _currentState;
    public PlayerBaseState CurrenSubState => _currentState.SubState;

    public void SetCurrentState(PlayerBaseState newState) => _currentState = newState;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        SetVisuals(_playerVisual);
    }
    private void Start()
    {
        _stateFactory = new PlayerStateFactory(this);
        _currentState = _stateFactory.Grounded();
        _currentState.EnterStates();
    }
    private void Update()
    {
        var dir = PlayerInputManager.Instance.CurrentMovementInput.normalized * Vector2.right;
        
        handleBoxCastColliders(dir);
        handleVisualSpriteFlip(dir);
        handleTimeToSlide();


        Debug.Log($"<color=teal>RootState</color> {_currentState.Type} | <color=Red>SubState</color> {_currentState.SubState?.Type ?? PlayerStates.None}");
        _currentState.UpdateStates();
    }
    private void FixedUpdate()
    {
        _currentState.FixedUpdateStates();
    }

    private void SetVisuals(GameObject newVisual)
    {
        _playerVisualSprite = newVisual.GetComponent<SpriteRenderer>();
        _playerVisualAnimator = newVisual.GetComponent<Animator>();
    }

    private void handleBoxCastColliders(Vector2 direction)
    {
        var angle = 0f;
        var distance = .1f;
        var origin = _collider.bounds.center;
        var size = _collider.bounds.size;

        var collisionsOnPlayerDirection = Physics2D.BoxCastAll(origin, size, angle, direction, distance);
        _canMove = collisionsOnPlayerDirection.Length <= 1; //always hitting player collider

        _isGrounded = Physics2D.BoxCast(origin, size, angle, Vector2.down, distance, _groundLayer);
        _canJump = Physics2D.BoxCast(origin, size, angle, Vector2.down, distance, _jumpableGroundLayer);
    }
    private void handleTimeToSlide()
    {
        if(StartRunningTime > _timeToSlide)
            _canSlide = true;
        else
            _canSlide = false;

        if (_currentState.Type != PlayerStates.Grounded)
            _canSlide = false;
    }
    private void handleVisualSpriteFlip(Vector2 dir)
    {
        if (_playerVisualSprite == null) return;

        if (dir.x < -0.01f)
            _playerVisualSprite.flipX = true;
        else if (dir.x > 0.01f)
            _playerVisualSprite.flipX = false;
    }
}
