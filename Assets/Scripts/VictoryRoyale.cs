using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryRoyale : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Canvas VictoryCanvas;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Win();
        }
    }

    void Win()
    {
        if (VictoryCanvas)
        {
            VictoryCanvas.gameObject.SetActive(true);
        }
    }
}
