using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForestAnimal : MonoBehaviour
{

    [Header("Movement")] [SerializeField] private float WalkSpeed = 4f;
    [SerializeField] private bool FlipsToMovement = true;
    [SerializeField] private float RunSpeed = 8f;

    [Header("Behavior")] [SerializeField] private bool ChasesPlayer;
    [SerializeField] private bool CanBeObstructed = false;
    [SerializeField] private bool AttacksPlayer;
    [SerializeField] private float AttackDistance = 1f;

    [SerializeField] private bool RunsFromPlayer;

    protected Transform _target;
    protected Health _health;

    protected Animator _animator;

    protected MeleeAttacker _attacker;
    protected SpriteRenderer _renderer;
    protected Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _attacker = this.gameObject.GetComponent<MeleeAttacker>();
        _animator = this.gameObject.GetComponent<Animator>();
        _health = this.gameObject.GetComponent<Health>();
        _renderer = this.gameObject.GetComponent<SpriteRenderer>();
        _rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        
        UpdateChasingDirection();
        _animator?.SetFloat("VerticalVelocity", _rigidbody2D.velocity.y);
        
    }

    private void UpdateChasingDirection()
    {
        if (_target == null) return;
        Vector3 dir = _target.position - transform.position;
        if (_rigidbody2D != null && dir.magnitude > AttackDistance)
        {
            _rigidbody2D.velocity = dir.normalized * RunSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<CharacterController>() && ChasesPlayer)
        {
            _target = col.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform == _target)
        {
            _target = null;
        }
    }
}
