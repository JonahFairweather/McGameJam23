using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Status")] [SerializeField] private float CurrentHealth;
    [SerializeField] private float MaxHealth = 100;
    [SerializeField] private float InitialHealth = 100;

    [Header("Death")] [SerializeField] private bool DestroyOnDeath = true;
    [SerializeField] private float DestructionDelaay = 2f;


    protected KnockbackTaker _knockback;
    protected bool _isDead;
    protected Animator _animator;
    protected CharacterMovementManager _characterMovementManager;
    private void OnEnable()
    {
        CurrentHealth = InitialHealth;
        _isDead = CurrentHealth < 0;
    }
    
    

    public void TakeDamage(float DmgAmt, GameObject instigator)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - DmgAmt, 0, MaxHealth);
        CheckIfDead();
        
        if (!_isDead)
        {
            
            _knockback?.TakeKnockback(gameObject.transform.position - instigator.transform.position, DmgAmt);
        }

        if (DmgAmt > 0)
        {
            _animator?.SetTrigger("Hit");
            Debug.Log("Sending hit trigger");
        }
        
    }

    private void CheckIfDead()
    {
        if (CurrentHealth <= 0)
        {
            _isDead = true;
            Kill();
        }
    }

    private void Kill()
    {
        if (DestroyOnDeath)
        {
            Destroy(this.gameObject, DestructionDelaay);
            // If this is the player, we can show the loss screen
        }

        if (_characterMovementManager)
        {
            _characterMovementManager.StopAllMovement();
        }

        if (_animator)
        {
            _animator.SetTrigger("Died");
        }
    }

    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }

    private void Awake()
    {
        _knockback = this.gameObject.GetComponent<KnockbackTaker>();
        _animator = this.gameObject.GetComponent<Animator>();
        _characterMovementManager = this.gameObject.GetComponent<CharacterMovementManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
