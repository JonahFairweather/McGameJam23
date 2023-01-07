using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

public class SnowGatherer : MonoBehaviour
{
    [Header("Snowball Stats")] [SerializeField]
    private int StartingSnowballs = 0;

    [SerializeField] private int CurrentSnowballs;
    [SerializeField] private int MaxSwowballs = 5;

    [Header("ThrowingStuffs")] [SerializeField]
    private float CooldownTime;

    protected CharacterController _character;
    protected float _cooldown;
    protected Animator _animator;
    private void Awake()
    {
        _character = gameObject.GetComponent<CharacterController>();
        CurrentSnowballs = StartingSnowballs;
        _animator = gameObject.GetComponent<Animator>();
    }

    public bool CanThrow()
    {
        return CurrentSnowballs > 0 && _cooldown <= 0;
    }

    public void Throw()
    {
        
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
