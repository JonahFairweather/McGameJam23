using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackTaker : MonoBehaviour
{

    protected Rigidbody2D _rigidbody2D;

    [SerializeField] private float KnockbackMultiplier;
    [SerializeField] private float VelocityPauseTime = 0.5f;

    private CharacterMovementManager _characterMovementManager;
    private void Awake()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _characterMovementManager = gameObject.GetComponent<CharacterMovementManager>();
    }

    public void TakeKnockback(Vector3 dir, float amt, float KnockbackAmt, float KnockbackDuration)
    {
        if (_characterMovementManager && KnockbackDuration > 0f)
        {
            _characterMovementManager.PauseMovement(KnockbackDuration);
        }
        if (_rigidbody2D)
        {
            Debug.Log("Taking knockback");
            _rigidbody2D.velocity = dir * KnockbackAmt * KnockbackMultiplier;
        }
    }
}
