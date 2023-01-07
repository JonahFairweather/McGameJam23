using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackTaker : MonoBehaviour
{

    protected Rigidbody2D _rigidbody2D;
    private void Awake()
    {
        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    public void TakeKnockback(Vector3 dir, float amt)
    {
        if (_rigidbody2D)
        {
            _rigidbody2D.AddForce(dir * amt * 10000);
        }
    }
}
