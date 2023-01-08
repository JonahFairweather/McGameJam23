using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
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
    [SerializeField] private bool DamageOnDash = true;
    [SerializeField] private float DashDamageAmount = 30f;
    [Header("HUD")] [SerializeField] private PlayerHUD PlayerHUD;
    

    protected float _currentMoveSpeed;
    protected float _dashCooldown;
    protected List<GameObject> _damagedThisDash;
    protected float _dashRemaining;
    protected Rigidbody2D _rigidbody2D;
    protected float _horizontalInput;
    protected float _verticalInput;
    protected Vector2 _movementVector;
    protected Vector2 _lastMovementVector;
    protected Vector2 _lastInputVector;
    protected bool _canChangeVelocity = true;
    protected bool _canMove;
    protected bool _disabled;
    protected bool _isFacingRight;
    protected bool _isFacingLeft;
    protected bool _isFacingUp;
    protected bool _isFacingDown;
    protected bool _dialogueMode;
    protected Animator _animator;
    private KeyCode _mostRecentlyPressed;
    private Directions _curDirection;
    protected Transform _meleeHitBoxLoc;
    protected Renderer _renderer;
    protected MeleeAttacker _attacker;
    private CharacterState _characterState;
    protected CircleCollider2D _myCollider;
    protected SnowGatherer _gatherer;
    protected bool _polledAudioInstance = false;
    protected Dialogue _currentDialogue;

    protected int _numBearKills;
    protected int _numFoxKills;

        [SerializeField] public AudioClip[] backgroundAudios;
    [SerializeField] public AudioClip slidingAudio;

    private void Awake()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _movementVector = new Vector2();
        _lastMovementVector = new Vector2(-1, -1);
        _lastInputVector = new Vector2(1, 1);
        _currentMoveSpeed = 0;
        _dashCooldown = 0;
        _damagedThisDash = new List<GameObject>();
        _attacker = gameObject.GetComponent<MeleeAttacker>();
        _gatherer = gameObject.GetComponent<SnowGatherer>();
        _isFacingRight = true;
        _isFacingLeft = _isFacingDown = _isFacingUp = false;
        _animator = gameObject.GetComponent<Animator>();
        _meleeHitBoxLoc = gameObject.transform.Find("WeaponHitBox");
        _characterState = CharacterState.Normal;
        _disabled = false;
        _canMove = true;
        _dialogueMode = false;
        _renderer = gameObject.GetComponent<Renderer>();
        _numBearKills = _numFoxKills = 0;
        _myCollider = gameObject.GetComponent<CircleCollider2D>();
    }

    public void DisableInput()
    {
        _disabled = true;
    }

    public void EnableInput()
    {
        _disabled = false;
    }

    private void Start()
    {
        
    }

    public IEnumerator PauseMovementAndActions(float time)
    {
        _canMove = false;
        _characterState = CharacterState.Gathering;
        yield return new WaitForSeconds(time);
        _characterState = CharacterState.Normal;
        _rigidbody2D.velocity = new Vector2(0, 0);
        _canMove = true;
    }


    void PollAudioInstance()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRandomMusic(this.backgroundAudios);
            _polledAudioInstance = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!_polledAudioInstance)
        {
            PollAudioInstance();
        }

        if (AudioManager.Instance != null)
        {
            HandleBackgroundMusic();
        }
        

        if(!_disabled) HandleDiagonalDirection();
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
        if (_movementVector.magnitude == 0 && _lastMovementVector.magnitude != 0 && _characterState != CharacterState.Gathering)
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

        if (_dashRemaining > 0)
        {
            _dashRemaining = Mathf.Clamp(_dashRemaining - Time.deltaTime, 0, DashDuration);
            if (_dashRemaining == 0)
            {
                _characterState = CharacterState.Normal;


                if (_animator)
                {
                    _animator.SetBool("Sliding", false);
                    _animator.SetTrigger("Idle");
                }
                
                _myCollider.radius /= 1.5f;
                _canChangeVelocity = true;
                _rigidbody2D.velocity = _movementVector * _currentMoveSpeed;
                _dashCooldown = DashCooldown;
            }
        }

        if (_characterState != CharacterState.Normal || _disabled) return;
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canMove)
        {
            if (_dashCooldown <= 0)
            {
                AudioManager.Instance.PlayEffect(this.slidingAudio);
                if (_animator)
                {
                    _animator.SetBool("Sliding", true);
                }
                _damagedThisDash.Clear();
                _dashCooldown = DashCooldown + DashDuration;
                _dashRemaining = DashDuration;
                _myCollider.radius *= 1.5f;
                _characterState = CharacterState.Dashing;
                _canChangeVelocity = false;
                _rigidbody2D.velocity = _lastInputVector * DashMagnitude;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (_gatherer && _gatherer.CanThrow(MousePos))
            {
                StartCoroutine(_gatherer.StartThrow(MousePos));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
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
    
    private IEnumerator Dash()
    {
        
        
       
        yield return new WaitForSeconds(DashDuration);
        
    }

    public void BeginDialogue()
    {
        _dialogueMode = false;
        if (PlayerHUD)
        {
            PlayerHUD.panel.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_characterState == CharacterState.Dashing)
        {
            if (!DamageOnDash) return;
            Health h = col.gameObject.GetComponent<Health>();
            if (h != null && !_damagedThisDash.Contains(h.gameObject))
            {
                h.TakeDamage(DashDamageAmount, gameObject, true, 20f, _lastInputVector, 1f);
                _damagedThisDash.Add(h.gameObject);
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            Vector2 dir = pos - transform.position;
            DashInDirection(dir);
            
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_characterState == CharacterState.Dashing)
        {
            if (!DamageOnDash) return;
            Health h = collision.gameObject.GetComponent<Health>();
            if (h != null && !_damagedThisDash.Contains(h.gameObject))
            {
                h.TakeDamage(DashDamageAmount, gameObject, true, 20f, _lastInputVector, 1f);
                _damagedThisDash.Add(h.gameObject);
            }
        }
    }

    private void DashInDirection(Vector2 dir)
    {
        AudioManager.Instance.PlayEffect(this.slidingAudio);
        if (_animator)
        {
            _animator.SetBool("Sliding", true);
        }
        _damagedThisDash.Clear();
        _dashCooldown = DashCooldown + DashDuration;
        _dashRemaining += DashDuration;
        
        _characterState = CharacterState.Dashing;
        _canChangeVelocity = false;
        _lastInputVector = dir.normalized;
        _rigidbody2D.velocity = _lastInputVector * DashMagnitude;
    }

    public void EnteredPond()
    {
        
    }

    public void IncreaseKill(string tag)
    {
        if (tag == "Fox")
        {
            _numFoxKills++;
        }else if (tag == "Bear")
        {
            _numBearKills++;
        }
        UpdateHUDKills();
    }

    public void StopMovement()
    {
        _canMove = false;
        _movementVector = new Vector2(0, 0);
        _rigidbody2D.velocity = _movementVector;
    }
    
    public void SetGatheringSnow(bool isGathering)
    {
        _characterState = isGathering ? CharacterState.Gathering : CharacterState.Normal;
    }

    public IEnumerator AttackFinished(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_characterState == CharacterState.Attacking)
        {
            _characterState = CharacterState.Normal;
        }
    }
    

    public void SetAttacking(bool isAttacking)
    {
        _characterState = isAttacking ? CharacterState.Attacking : CharacterState.Normal;
    }

    private void HandleDiagonalDirection()
    {
        if (_characterState != CharacterState.Normal && _characterState != CharacterState.Dashing)
        {
            return;
            
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _mostRecentlyPressed = KeyCode.Q;
            _movementVector.x = -1;
            _movementVector.y = 1;
        }else if (Input.GetKeyDown(KeyCode.A))
        {
            _mostRecentlyPressed = KeyCode.A;
            _movementVector.x = -1;
            _movementVector.y = -1;
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            _mostRecentlyPressed = KeyCode.E;
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
        if (_renderer)
        {
            _renderer.sortingOrder = (int)transform.position.y * -1;
        }
        Move();    
    }

    void Move()
    {

        if (!_canMove)
        {
            return;
        }

        if (_movementVector != new Vector2(0, 0))
        {
            _lastInputVector = _movementVector;
        }

        
        if (_canChangeVelocity)
        {
            _rigidbody2D.velocity = _movementVector * _currentMoveSpeed;
        }
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

    private void UpdateHUDKills()
    {
        if (PlayerHUD)
        {
            if (PlayerHUD.BearKills)
            {
                PlayerHUD.BearKills.SetText(_numBearKills.ToString());
            }

            if (PlayerHUD.FoxKills)
            {
                PlayerHUD.FoxKills.SetText(_numFoxKills.ToString());
            }
        }
    }

    private void HandleBackgroundMusic() {
        if(!AudioManager.Instance.IsPlayingMusic()) {
            AudioManager.Instance.PlayRandomMusic(backgroundAudios);
        }
    }
}
