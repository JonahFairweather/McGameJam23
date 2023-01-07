using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.UIElements;

public class SnowGatherer : MonoBehaviour
{
    [Header("Snowball Stats")] [SerializeField]
    private int StartingSnowballs = 0;

    [SerializeField] private int CurrentSnowballs;
    [SerializeField] private int MaxSwowballs = 5;

    [Header("ThrowingStuffs")] [SerializeField]
    private float CooldownTime;

    [SerializeField] private float TimeBeforeThrow = 0.5f;
    [SerializeField] private float MaxAngle = 45f;
    [SerializeField] private Transform SpawnLocation;
    [SerializeField] private Snowball SnowballToSpawn;
    

    protected CharacterController _character;
    protected float _cooldown;
    protected Animator _animator;
    protected Vector3 ThrowDir;
    private void Awake()
    {
        _character = gameObject.GetComponent<CharacterController>();
        CurrentSnowballs = StartingSnowballs;
        _animator = gameObject.GetComponent<Animator>();
    }

    public bool CanThrow(Vector3 MouseWorldPos)
    {
        Vector3 dir = MouseWorldPos - SpawnLocation.position;
        dir.z = 0;
        float angle = Vector3.Angle(dir.normalized, SpawnLocation.localPosition);
        if (angle > MaxAngle) return false;
        return CurrentSnowballs > 0 && _cooldown <= 0;
    }

    public IEnumerator StartThrow(Vector3 MouseWorldPos)
    {
        _animator?.SetTrigger("ThrowSnowball");
        _character.SetAttacking(true);
        ThrowDir = MouseWorldPos - SpawnLocation.position;
        yield return new WaitForSeconds(TimeBeforeThrow);
        Throw(MouseWorldPos);
    }
    public void Throw(Vector3 MouseWorldPos)
    {
        if(CurrentSnowballs <=0) return;
        if (SnowballToSpawn != null)
        {
            
            Snowball s = Instantiate<Snowball>(SnowballToSpawn, (SpawnLocation.position - this.gameObject.transform.position)/2 + this.gameObject.transform.position, Quaternion.identity);
            s.AddObjectToIgnore(this.gameObject);
            if (s != null)
            {
                
                s.SetVelocity(ThrowDir);
                s.Owner = this.gameObject;
                _cooldown = CooldownTime;
                CurrentSnowballs -= 1;
                _character?.UpdateHUDSnowballs();
                
                StartCoroutine(_character.AttackFinished(0.16f));

            }
        }
    }

    public void GatherSnow()
    {
        if (_animator)
        {
            _animator.SetTrigger("GatheringSnow");
        }

        if (_character)
        {
            _character.SetGatheringSnow(true);
        }

        StartCoroutine(SetCanInterrupt(0.66f));
    }

    private IEnumerator SetCanInterrupt(float time)
    {
        yield return new WaitForSeconds(time);
        IncreaseSnowballs();
        GatherInterrupt();
        
    }
    

    private void Update()
    {
        if (_cooldown > 0)
        {
            _cooldown = Mathf.Clamp(_cooldown - Time.deltaTime, 0, CooldownTime);
        }
    }

    public int NumSnowballs()
    {
        return CurrentSnowballs;
    }
    
    public bool CanGather()
    {
        return true;
    }

    public void GatherInterrupt()
    {
        if (_character)
        {
            _character.SetGatheringSnow(false);
        }
    }

    public void IncreaseSnowballs()
    {
        CurrentSnowballs = Mathf.Clamp(CurrentSnowballs + 1, 0, MaxSwowballs);
        _character?.UpdateHUDSnowballs();
        
    }


}
