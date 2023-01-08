using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] public AudioClip deathAudio;
    [SerializeField] public AudioClip damageAudio;

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
            _knockback.TakeKnockback(gameObject.transform.position - instigator.transform.position, DmgAmt);
            AudioManager.Instance.PlayEffect(this.damageAudio);
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
    }

    private void Awake()
    {
        _knockback = this.gameObject.GetComponent<KnockbackTaker>();
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
