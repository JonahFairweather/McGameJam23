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
    
    

    [SerializeField] private float MaxSpeed = 10f;
    [SerializeField] private float Acceleration = 1f;
    [Header("Dash")]
    [SerializeField] private float DashDuration = 1f;
    [SerializeField] private float DashMagnitude = 15f;
    [SerializeField] private float DashCooldown = 3f;
    [Header("Orientation")]
    

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

    private void Awake()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _movementVector = new Vector2();
        _lastMovementVector = new Vector2(-1, -1);
        _currentMoveSpeed = 0;
        _dashCooldown = 0;
        _attacker = gameObject.GetComponent<MeleeAttacker>();
        _isFacingRight = true;
        _isFacingLeft = _isFacingDown = _isFacingUp = false;
        _animator = gameObject.GetComponent<Animator>();
        _meleeHitBoxLoc = gameObject.transform.Find("WeaponHitBox");
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

        if (_animator && !(_dashCooldown > 0))
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
        _animator.SetFloat("MovementMagnitude", _movementVector.magnitude);
        
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
        
        HandleFacingDirection();
        if (_dashCooldown > 0)
        {
            _dashCooldown -= Time.deltaTime;
        }
        
    }

    private void HandleDiagonalDirection()
    {
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
            SetAnimatorParams();
        }
        
        
    }

    private void SetAnimatorParams()
    {
        
        if (_animator != null)
        {
            _animator.SetBool("IsFacingUp", _isFacingUp);
            _animator.SetBool("IsFacingDown", _isFacingDown);
            _animator.SetBool("IsFacingLeft", _isFacingLeft);
            _animator.SetBool("IsFacingRight", _isFacingRight);
            
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
        
        
        if (_canChangeVelocity)
        {
            _rigidbody2D.velocity = _movementVector * _currentMoveSpeed;
        }

        if (_movementVector != new Vector2(0, 0))
        {
            _lastInputVector = _movementVector;
        }
        

    }

    private void HandleFacingDirection()
    {
        //Handles which way the character is facing, will only change if magnitude of movement vector is > 0;
        if (_canChangeVelocity)
        {
            
            if (_movementVector.x == 1 && _movementVector.y == -1)
            {
                
                _isFacingRight = true;
                
                _isFacingDown = _isFacingUp = _isFacingLeft = false;
                if (_curDirection != Directions.Right)
                {
                    _curDirection = Directions.Right;
                    SetAnimatorParams();
                }
            }
            if (_movementVector.x == -1 && _movementVector.y != 1)
            {
                _isFacingLeft = true;
                
                _isFacingDown = _isFacingUp = _isFacingRight = false;
                if (_curDirection != Directions.Left)
                {
                    SetAnimatorParams();
                    _curDirection = Directions.Left;
                }
            }

            if (_movementVector.y == 1 && _movementVector.x != 1)
            {
                _isFacingUp = true;
                
                _isFacingDown = _isFacingRight = _isFacingLeft = false;
                if (_curDirection != Directions.Up)
                {
                    SetAnimatorParams();
                    _curDirection = Directions.Up;
                }
            }
            if (_movementVector.y == -1 && _movementVector.x != -1)
            {
                _isFacingDown = true;
                
                _isFacingRight = _isFacingUp = _isFacingLeft = false;
                if (_curDirection != Directions.Down)
                {
                    _curDirection = Directions.Down;
                    SetAnimatorParams();
                }
            }
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dashing");
        if (_animator)
        {
            _animator.SetBool("Sliding", true);
        }
        _canChangeVelocity = false;
        _rigidbody2D.velocity = _lastInputVector * DashMagnitude;
        yield return new WaitForSeconds(DashDuration);
        if (_animator)
        {
            _animator.SetBool("Sliding", false);
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
}
