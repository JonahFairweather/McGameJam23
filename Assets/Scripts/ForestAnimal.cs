using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class ForestAnimal : MonoBehaviour
{

    [Header("Movement")] [SerializeField] private float WalkSpeed = 4f;
    [SerializeField] private bool FlipsToMovement = true;
    [SerializeField] private float RunSpeed = 8f;
    [SerializeField] private bool LockToFourAxis = true;
    

    [Header("Behavior")] [SerializeField] private bool ChasesPlayer;
    [SerializeField] private bool CanBeObstructed = false;
    [SerializeField] public LayerMask ObstructionLayers;
    [SerializeField] private bool AttacksPlayer;
    
    [SerializeField] private float AttackDistance = 1f;
    [SerializeField] private float DetectionRadius = 2f;
    [SerializeField] private bool HasStandingAnim;
    [SerializeField] private bool RunsFromPlayer;
    [SerializeField] private Gradient StunGradient;
    
    

    protected Transform _target;
    protected Health _health;
    protected bool isSitting;
    protected bool _canMove;
    protected Animator _animator;
    protected bool _disabled;
    protected bool _unprotected;
    protected MeleeAttacker _attacker;
    protected SpriteRenderer _renderer;
    protected Rigidbody2D _rigidbody2D;
    protected Vector2 _lastInputVector;
    

    [SerializeField] public AudioClip alertAudio;

    private void Awake()
    {
        _attacker = this.gameObject.GetComponent<MeleeAttacker>();
        
        _animator = this.gameObject.GetComponent<Animator>();
        _health = this.gameObject.GetComponent<Health>();
        _renderer = this.gameObject.GetComponent<SpriteRenderer>();
        _rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        CircleCollider2D c = gameObject.AddComponent<CircleCollider2D>();
        c.radius = DetectionRadius;
        c.isTrigger = true;
        _lastInputVector = new Vector2(-1, -1);
    }

    // Start is called before the first frame update
    void Start()
    {
        _canMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_renderer)
        {
            _renderer.sortingOrder = (int)transform.position.y * -1;
        }
        
        if (FlipsToMovement)
        {
            if (_rigidbody2D.velocity.x > 0)
            {
                _renderer.flipX = true;
            }else if (_rigidbody2D.velocity.x < 0)
            {
                _renderer.flipX = false;
            }
        }
        
        if (_canMove)
        {
            UpdateChasingDirection();
        }
        if (_attacker)
        {
            _attacker.SetLocalPos(_lastInputVector);
        }

        if (_target != null && CanBeObstructed)
        {
            CheckIfTargetObstructed();
        }
        
        _animator.SetFloat("LastHorizontal", _lastInputVector.x);
        _animator.SetFloat("LastVertical", _lastInputVector.y);
        _animator?.SetFloat("VerticalVelocity", _rigidbody2D.velocity.y);
        _animator.SetFloat("MovementMagnitude", _rigidbody2D.velocity.magnitude);

        if (!_canMove)
        {
            //Cannot attack either
            
            return;
        }

        if (AttacksPlayer && _target != null && (_target.position - transform.position).magnitude < AttackDistance)
        {
            if (_attacker && _attacker.CanAttack())
            {
                _attacker.Attack();
            }
            
        }
        
        
    }

    public bool GetIsUnprotected()
    {
        return _unprotected;
    }

    public void StartUnprotectForDuration(float duration)
    {
        StartCoroutine(UnprotectForDuration(duration));
    }

    public IEnumerator UnprotectForDuration(float duration)
    {
        _unprotected = true;
        float _durationElapsed = 0f;
        while (_durationElapsed < duration)
        {
            _durationElapsed += Time.deltaTime;
            _renderer.color = StunGradient.Evaluate(_durationElapsed / duration);
            
            yield return null;
        }

        _unprotected = false;

    }

    public void DisableAnimalBehavior()
    {
        _canMove = false;
        _target = null;
        _disabled = true;
    }

    private void CheckIfTargetObstructed()
    {
        Vector2 dir = _target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3) dir * 1f , _target.position - transform.position,
            DetectionRadius + 5f, ObstructionLayers
        );
        if (hit.transform != _target)
        {
            
            _target = null;
        }
    }

    public void EnableAnimalBehavior()
    {
        _canMove = true;
        _disabled = false;
        Collider2D[] Colliders = Physics2D.OverlapCircleAll(transform.position, DetectionRadius);
        foreach (Collider2D c in Colliders)
        {
            if (c.gameObject.CompareTag("Player"))
            {
                
                if (CanBeObstructed)
                {
                    Vector2 dir = c.transform.position - transform.position;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position , c.transform.position - transform.position,
                        DetectionRadius + 5f, ObstructionLayers
                    );
                    if (hit.transform != c.transform)
                    {
                        
                    }
                    else
                    {
                        
                        _target = c.transform;
                    }
                }
                else
                {
            
                    _target = c.transform;
                }
            }
        }
        
    }

    public void PauseMovement(bool stopVelo)
    {
       
        _canMove = false;
        foreach (Collider2D c in gameObject.GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
        if (stopVelo) _rigidbody2D.velocity = new Vector2(0, 0);
    }

    public IEnumerator PauseMovementAndResume(float delay)
    {
        _canMove = false;
        _rigidbody2D.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(delay);
        _rigidbody2D.velocity = new Vector2(0, 0);
        _canMove = true;
    }

    public void ResumeMovement()
    {
        _canMove = true;
    }

    private void UpdateChasingDirection()
    {
        if (_target == null)
        {
            _rigidbody2D.velocity = new Vector2(0, 0);
            return;
        }
        Vector3 dir = _target.position - transform.position;
        dir.z = 0;
        if (dir.magnitude < AttackDistance - 0.3f)
        {
            _rigidbody2D.velocity = new Vector2(0, 0);
            return;
        }
        if (_rigidbody2D != null && dir.magnitude > AttackDistance/2f)
        {
            if (LockToFourAxis)
            {
                if(dir.x >= 0 && dir.y >=0)
                {
                    // Go towards the top right
                    dir.x = 1;
                    dir.y = 1;
                }else if (dir.x >= 0 && dir.y < 0)
                {
                    dir.x = 1;
                    dir.y = -1;
                }else if (dir.x < 0 && dir.y >= 0)
                {
                    dir.x = -1;
                    dir.y = 1;
                }else if (dir.x < 0 && dir.y < 0)
                {
                    dir.x = -1;
                    dir.y = -1;
                }

                _lastInputVector = dir;
                _rigidbody2D.velocity = dir * RunSpeed;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_disabled) return;
      
        if (col.gameObject.tag == "Player" && ChasesPlayer)
        {
            _target = col.gameObject.transform;
    
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayEffect(this.alertAudio);
            }
            
            if (HasStandingAnim)
            {
                StartCoroutine(Stand());
            }
            else
            {
                _canMove = true;
            }
            
        }
    }

    private IEnumerator Stand()
    {
        _animator.SetTrigger("Stand");
        yield return new WaitForSeconds(1f);
        
        if (_target != null)
        {
            _canMove = true;
            isSitting = false;
        }
        
    }

    private void Sit()
    {
        if (!_health.IsAlive()) return;
        if (_rigidbody2D)
        {
            _rigidbody2D.velocity = new Vector2(0, 0);
        }
        _canMove = false;
        isSitting = true;
        _animator.SetTrigger("Sit");
    }

    private IEnumerator PermitMovement(float delay)
    {
        yield return new WaitForSeconds(delay);
        _canMove = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == _target)
        {
            _target = null;
            Debug.Log("No more target!");
            Sit();
        }

        
    }
}
