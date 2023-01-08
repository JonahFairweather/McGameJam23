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

    [Header("Knockback")] [SerializeField] private bool AppliesKnockback = true;
    [SerializeField] private float KnockbackMultiplier = 2f;
    [SerializeField] private float KnockbackDuration = 1f;
    


    protected Animator _animator;
    protected CharacterController _character;
    protected float _attackCooldown;

    [SerializeField] public AudioClip penguinPeckingAudio;
    [SerializeField] public AudioClip enemyPeckingAudio;

    void Start()
    {
        
    }

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
        _character = gameObject.GetComponent<CharacterController>();
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
            _animator.SetTrigger("Attack");
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
        if (this.gameObject.tag == "Player") {
            AudioManager.Instance.PlayEffect(this.penguinPeckingAudio);
        } else {
            AudioManager.Instance.PlayEffect(this.enemyPeckingAudio);
        }
        Collider2D[] overlapped = Physics2D.OverlapCircleAll(DamageLocation.position, CircleRadius);
        Debug.Log(overlapped.Length);
        foreach(Collider2D c in overlapped)
        {
            if (c.gameObject != gameObject)
            {
                Health h = c.gameObject.GetComponent<Health>();
                if (h != null)
                {
                    h.TakeDamage(DamageAmount, gameObject, AppliesKnockback, KnockbackMultiplier, c.gameObject.transform.position - transform.position, KnockbackDuration);
                }
            }
        }
        
    }

    public void SetLocalPos(Vector2 pos)
    {
        DamageLocation.localPosition = pos;
    }

    private void OnDrawGizmos()
    {
        if (DamageLocation)
        {
            Gizmos.DrawSphere(DamageLocation.position, CircleRadius);
        }
    }
}
