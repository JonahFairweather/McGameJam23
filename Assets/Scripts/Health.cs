using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Status")] [SerializeField] private float CurrentHealth;
    [SerializeField] private float MaxHealth = 100;
    [SerializeField] private float InitialHealth = 100;

    [Header("Death")] [SerializeField] private bool DestroyOnDeath = true;
    [SerializeField] private float DestructionDelaay = 2f;
    [SerializeField] private GameObject InstantiateOnDeath;


    protected KnockbackTaker _knockback;
    protected bool _isDead;

    [SerializeField] public AudioClip deathAudio;
    [SerializeField] public AudioClip damageAudio;

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
            AudioManager.Instance.PlayEffect(this.damageAudio);
            _knockback?.TakeKnockback(gameObject.transform.position - instigator.transform.position, DmgAmt, 1f, 0f);
        }

        if (DmgAmt > 0)
        {
            _animator?.SetTrigger("Hit");
            Debug.Log("Sending hit trigger");
        }
        
    }

    public void TakeDamage(float DmgAmt, GameObject instigator, bool ApplyKnockback, float KnockbackMultiplier, Vector3 KnockbackDirection, float KnockbackDuration)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - DmgAmt, 0, MaxHealth);
        CheckIfDead();

        if (ApplyKnockback)
        {
            if (_knockback)
            {
                _knockback.TakeKnockback(KnockbackDirection, DmgAmt, KnockbackMultiplier, KnockbackDuration);
            }
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
            AudioManager.Instance.PlayEffect(this.deathAudio);
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

    private void OnDestroy()
    {
        if (InstantiateOnDeath)
        {
            GameObject obj = Instantiate(InstantiateOnDeath, transform.position, quaternion.identity);
            if (obj != null)
            {
                Renderer r = obj.GetComponent<Renderer>();
                if (r != null)
                {
                    r.sortingOrder = (int) (obj.transform.position.y + 2) * -1;
                }
            }
        }
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
