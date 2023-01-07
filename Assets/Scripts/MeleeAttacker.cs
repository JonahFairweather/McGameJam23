using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MeleeAttacker : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("DamageStats")] [SerializeField]
    private float DamageAmount;

    [SerializeField] private Transform DamageLocation;
    [SerializeField] private float CircleRadius = 1f;
    

    [SerializeField] private float AttackCooldown = 2f;
    [SerializeField] private float TimeBeforeEnableDamage = 0f;
    


    protected Animator _animator;
    protected float _attackCooldown;
    void Start()
    {
        
    }

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_attackCooldown > 0)
        {
            _attackCooldown = Mathf.Clamp(_attackCooldown - Time.deltaTime, 0, AttackCooldown);
        }
    }

    public bool CanAttack()
    {
        return _attackCooldown <= 0;
    }

    public void Attack()
    {
        if (_animator)
        {
            _animator.SetTrigger("Attacking");
        }

        if (TimeBeforeEnableDamage > 0)
        {
            StartCoroutine(EnableHitBox());
        }
        else
        {
            CreateDamageBox();
        }
        
        _attackCooldown = AttackCooldown;
    }

    private IEnumerator EnableHitBox()
    {
        yield return new WaitForSeconds(TimeBeforeEnableDamage);
        CreateDamageBox();
    }

    private void CreateDamageBox()
    {
        Collider2D[] overlapped = Physics2D.OverlapCircleAll(DamageLocation.position, CircleRadius);
        
        foreach(Collider2D c in overlapped)
        {
            if (c.gameObject != gameObject)
            {
                Health h = c.gameObject.GetComponent<Health>();
                h?.TakeDamage(DamageAmount, gameObject);
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        if (DamageLocation)
        {
            Gizmos.DrawSphere(DamageLocation.position, CircleRadius);
        }
    }
}
