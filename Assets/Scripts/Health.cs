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
    [SerializeField] private bool FlashOnDamage = true;
    [SerializeField] private Gradient DamageFlash;
    [SerializeField] private float FlashDuration = 1f;


    protected KnockbackTaker _knockback;
    protected SpriteRenderer _spriteRenderer;
    protected bool _isDead;
    protected Color _originalColor;

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
        CurrentHealth = Mathf.Clamp(CurrentHealth - DmgAmt * GetDamageMultiplier(this.gameObject), 0, MaxHealth);
        CheckIfDead();
        
        if (!_isDead)
        {
            AudioManager.Instance.PlayEffect(this.damageAudio);
            
        }

        
        if (DmgAmt > 0)
        {
            if (FlashOnDamage)
            {
                StartCoroutine(Flash());
            }
            _animator?.SetTrigger("Hit");
            
        }
        
    }

    private IEnumerator Flash()
    {
        if (!_spriteRenderer)
            yield break;

       
        float TimeElapsed = 0f;
        while (TimeElapsed < FlashDuration)
        {
            TimeElapsed += Time.deltaTime;
            _spriteRenderer.color = DamageFlash.Evaluate(TimeElapsed / FlashDuration);
            yield return null;
        }

        _spriteRenderer.color = _originalColor;
    }

    public void TakeDamage(float DmgAmt, GameObject instigator, bool ApplyKnockback, float KnockbackMultiplier, Vector3 KnockbackDirection, float KnockbackDuration)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - DmgAmt * GetDamageMultiplier(this.gameObject), 0, MaxHealth);
        bool wasDead = _isDead;
        CheckIfDead();
        if (_isDead && !wasDead)
        {
            CharacterController c = instigator.GetComponent<CharacterController>();
            if (c != null)
            {
                c.IncreaseKill(gameObject.tag);
            }
        }
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
            if (FlashOnDamage)
            {
                StartCoroutine(Flash());
            }
        }
    }

    private int GetDamageMultiplier(GameObject damaged)
    {
        ForestAnimal a = damaged.GetComponent<ForestAnimal>();
        if (a == null) return 1;
        return a.GetIsUnprotected() ? 1 : 0 + 1;
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
                Renderer[] r = obj.GetComponentsInChildren<Renderer>();

                foreach (Renderer rend in r)
                {
                    rend.sortingOrder = (int)obj.transform.position.y * -1;
                }
            }
        }
    }
    
    

    private void Awake()
    {
        _knockback = this.gameObject.GetComponent<KnockbackTaker>();
        _animator = this.gameObject.GetComponent<Animator>();
        _characterMovementManager = this.gameObject.GetComponent<CharacterMovementManager>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
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
