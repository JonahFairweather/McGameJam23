using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{

    protected CharacterController _characterController;
    protected ForestAnimal _forestAnimal;
    
    private void Awake()
    {
        _forestAnimal = this.gameObject.GetComponent<ForestAnimal>();
        _characterController = this.gameObject.GetComponent<CharacterController>();
    }

    public void PauseIntentionalMovement()
    {
        if (_forestAnimal)
        {
            _forestAnimal.PauseMovement(false);
        }

        if (_characterController)
        {
            _characterController.StopMovement();
        }
    }

    public void PauseMovement(float time)
    {
        if (_forestAnimal)
        {
           StartCoroutine(_forestAnimal.PauseMovementAndResume(time));
        }

        if (_characterController)
        {
            StartCoroutine(_characterController.PauseMovementAndActions(time));
        }
    }

    public void StopAllMovement()
    {
        if (_forestAnimal)
        {
            _forestAnimal.PauseMovement(true);
        }

        if (_characterController)
        {
            _characterController.StopMovement();
        }
    }
}
