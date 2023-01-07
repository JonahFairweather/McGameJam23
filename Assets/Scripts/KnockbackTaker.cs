using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackTaker : MonoBehaviour
{

    protected Rigidbody2D _rigidbody2D;

    [SerializeField] private float KnockbackMultiplier;
    [SerializeField] private float VelocityPauseTime = 0.5f;
    private void Awake()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void TakeKnockback(Vector3 dir, float amt)
    {
        
        if (_rigidbody2D)
        {
            Debug.Log("Taking knockback");
            _rigidbody2D.AddForce(dir * amt * KnockbackMultiplier, ForceMode2D.Impulse);
        }
    }
}
