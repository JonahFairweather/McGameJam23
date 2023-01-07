using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update
    void Move(InputAction.CallbackContext context)
    {
        Debug.Log("test");
    }
    
    private enum Directions{Up, Down, Left, Right}

    private enum CharacterState
    {
        Normal,
        Dashing,
        Gathering, 
        Attacking
    }
    
    

    [SerializeField] private float MaxSpeed = 10f;
    [SerializeField] private float Acceleration = 1f;
    [Header("Dash")]
    [SerializeField] private float DashDuration = 1f;
    [SerializeField] private float DashMagnitude = 15f;
    [SerializeField] private float DashCooldown = 3f;
    [Header("HUD")] [SerializeField] private PlayerHUD PlayerHUD;
    

    protected float _currentMoveSpeed;
    protected float _dashCooldown;
    protected Rigidbody2D _rigidbody2D;
    protected float _horizontalInput;
    protected float _verticalInput;
    protected Vector2 _movementVector;
    protected Vector2 _lastMovementVector;
    protected Vector2 _lastInputVector;
    protected bool _canChangeVelocity = true;
    
    
    protected bool _isFacingRight;
    protected bool _isFacingLeft;
    protected bool _isFacingUp;
    protected bool _isFacingDown;
    protected Animator _animator;
    private KeyCode _mostRecentlyPressed;
    private Directions _curDirection;
    protected Transform _meleeHitBoxLoc;

    protected MeleeAttacker _attacker;
    private CharacterState _characterState;
    protected SnowGatherer _gatherer;
    

    private void Awake()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _movementVector = new Vector2();
        _lastMovementVector = new Vector2(-1, -1);
        _currentMoveSpeed = 0;
        _dashCooldown = 0;
        _attacker = gameObject.GetComponent<MeleeAttacker>();
        _gatherer = gameObject.GetComponent<SnowGatherer>();
        _isFacingRight = true;
        _isFacingLeft = _isFacingDown = _isFacingUp = false;
        _animator = gameObject.GetComponent<Animator>();
        _meleeHitBoxLoc = gameObject.transform.Find("WeaponHitBox");
        _characterState = CharacterState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        
        HandleDiagonalDirection();
        if (_animator && _canChangeVelocity)
        {
            _animator.SetFloat("Horizontal", _movementVector.x);
            _animator.SetFloat("Vertical", _movementVector.y);
            
        }

        if (_meleeHitBoxLoc)
        {
            _meleeHitBoxLoc.localPosition = _lastInputVector * 0.75f;
        }

        if (_animator && _canChangeVelocity)
        {
            _animator.SetFloat("LastHorizontal", _lastInputVector.x);
            _animator.SetFloat("LastVertical", _lastInputVector.y);
        }
        if (_movementVector.magnitude == 0 && _lastMovementVector.magnitude != 0)
        {
            if (_animator)
            {
                _animator.SetTrigger("Idle");
            }
        }
        _lastMovementVector = _movementVector;
        if (_characterState == CharacterState.Normal)
        {
            _animator.SetFloat("MovementMagnitude", _movementVector.magnitude);
        }
        
        if (_dashCooldown > 0)
        {
            _dashCooldown -= Time.deltaTime;
        }

        if (_characterState != CharacterState.Normal) return;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_dashCooldown <= 0)
            {
                StartCoroutine(Dash());
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_attacker)
            {
                if (_attacker.CanAttack())
                {
                    _attacker.Attack();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_gatherer && _gatherer.CanGather())
            {
                _animator.SetFloat("MovementMagnitude", 0f);
                _movementVector = new Vector2(0,0);
                _gatherer.GatherSnow();
                
            }
        }
    }

    public void SetGatheringSnow(bool isGathering)
    {
        _characterState = isGathering ? CharacterState.Gathering : CharacterState.Normal;
    }

    public void SetAttacking(bool isAttacking)
    {
        _characterState = isAttacking ? CharacterState.Attacking : CharacterState.Normal;
    }

    private void HandleDiagonalDirection()
    {
        if (_characterState != CharacterState.Normal)
        {
            return;
            
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _mostRecentlyPressed = KeyCode.A;
            _movementVector.x = -1;
            _movementVector.y = 1;
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            _mostRecentlyPressed = KeyCode.S;
            _movementVector.x = -1;
            _movementVector.y = -1;
        }else if (Input.GetKeyDown(KeyCode.W))
        {
            _mostRecentlyPressed = KeyCode.W;
            _movementVector.x = 1;
            _movementVector.y = 1;
        }else if (Input.GetKeyDown(KeyCode.D))
        {
            _mostRecentlyPressed = KeyCode.D;
            _movementVector.x = 1;
            _movementVector.y = -1;
        }

        if (Input.GetKeyUp(_mostRecentlyPressed))
        {
            _movementVector.x = _movementVector.y = 0;
           
        }
        
        
    }

    private void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        HandleAcceleration();
        
        Move();    
    }

    void Move()
    {
        
        

        if (_movementVector != new Vector2(0, 0))
        {
            _lastInputVector = _movementVector;
        }

        
        if (_canChangeVelocity)
        {
            _rigidbody2D.velocity = _movementVector * _currentMoveSpeed;
        }
    }

    

    private IEnumerator Dash()
    {
        
        
        if (_animator)
        {
            _animator.SetBool("Sliding", true);
        }

        _characterState = CharacterState.Dashing;
        _canChangeVelocity = false;
        _rigidbody2D.velocity = _lastInputVector * DashMagnitude;
        yield return new WaitForSeconds(DashDuration);
        _characterState = CharacterState.Normal;
        _movementVector = _lastInputVector;
        if (_animator)
        {
            _animator.SetBool("Sliding", false);
            _animator.SetTrigger("Idle");
        }
        _canChangeVelocity = true;
        _dashCooldown = DashCooldown;
    }

    void HandleRotation()
    {
        Vector3 m = Input.mousePosition;
        m.z = 0;
        transform.up = m;
    }

    void HandleAcceleration()
    {
        if (_movementVector.magnitude > 0)
        {
            _currentMoveSpeed += Acceleration * Time.deltaTime * _movementVector.magnitude;
        }
        else
        {
            _currentMoveSpeed += Acceleration * Time.deltaTime * -5;
        }

        _currentMoveSpeed = Mathf.Clamp(_currentMoveSpeed, 0, MaxSpeed);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        
    }

    public void UpdateHUDSnowballs()
    {
        if (PlayerHUD)
        {
            PlayerHUD.SnowballText.text = _gatherer.NumSnowballs().ToString();
        }
    }
}