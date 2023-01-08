using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class TutorialGUI : MonoBehaviour
{
    [Serializable]
    public struct StuffToSetActive
    {
        
        public List<GameObject> ToSetActive;

        public void SetAllActive()
        {
            foreach (GameObject g in ToSetActive)
            {
                g.SetActive(true);
            }
        }

        public void SetAllInactive()
        {
            foreach (GameObject g in ToSetActive)
            {
                g.SetActive(false);
            }
        }
    }

    private StuffToSetActive CurrentStuff;

    private ForestAnimal[] _animals;

    private CharacterController _character;
    
    private int i = 0;

    public List<StuffToSetActive> OrderOfGUI;
    // Start is called before the first frame update
    void Start()
    {
        StartTutorial();
        _animals = Object.FindObjectsOfType<ForestAnimal>();
        foreach (ForestAnimal f in _animals)
        {
            f.DisableAnimalBehavior();
        }

        _character = Object.FindObjectOfType<CharacterController>();
        if (_character)
        {
            _character.DisableInput();
        }
    }

    public void StartTutorial()
    {
        CurrentStuff = OrderOfGUI[0];
        CurrentStuff.SetAllActive();
        i++;
    }

    public void NextPart()
    {
        if (i >= OrderOfGUI.Count())
        {
            // Tutorial is over, start the game
            this.gameObject.SetActive(false);
            StartGame();
        }
        else
        {
            CurrentStuff.SetAllInactive();
            CurrentStuff = OrderOfGUI[i];
            CurrentStuff.SetAllActive();
            i++;
        }
    } 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextPart();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        _character.EnableInput();
        foreach (ForestAnimal f in _animals)
        {
            f.EnableAnimalBehavior();
        }
        
    }
   
}
