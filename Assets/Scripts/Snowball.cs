using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Snowball : MonoBehaviour {
    private Vector3 initialPosition;
    protected Rigidbody2D _rigidbody2D;
    [SerializeField] private float Velocity = 10f;
    private List<GameObject> _objectsToIgnore;
    [SerializeField] private LayerMask LayersToIgnore;
    [SerializeField] private float DamageAmount = 5f;
    [SerializeField] private float MaximumLifetime = 5f;

    private CircleCollider2D _collider;
    protected float _lifeTimeElapsed;
    public GameObject Owner; 

    // Start is called before the first frame update
    void Start() {
        initialPosition = transform.position;
        _collider = this.gameObject.GetComponent<CircleCollider2D>();
        _collider.enabled = false;
        StartCoroutine(StartCollisionDealy());
    }

    private IEnumerator StartCollisionDealy()
    {
        yield return null;
        yield return null;
        if (_collider)
        {
            _collider.enabled = true;
        }
    }

    private void Awake()
    {
        _rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        _objectsToIgnore = new List<GameObject>();
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        if (_rigidbody2D != null)
        {
            _rigidbody2D.velocity = newVelocity.normalized * Velocity;
        }
    }

    public void AddObjectToIgnore(GameObject other)
    {
        if (_objectsToIgnore.Contains(other)) return; 
        _objectsToIgnore.Add(other);
    }

    // Update is called once per frame
    void Update() {
        if (_lifeTimeElapsed < MaximumLifetime)
        {
            _lifeTimeElapsed += Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }



    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_objectsToIgnore.Contains(other.gameObject))
        {
            return;
        }

        Health h = other.gameObject.GetComponent<Health>();
        if (h != null)
        {
            h.TakeDamage(DamageAmount, Owner);
        }
        GameObject.Destroy(this.gameObject);
    }
}
