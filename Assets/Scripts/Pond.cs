using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pond : MonoBehaviour
{

    [SerializeField] private Canvas UICanvas;

    protected TextMeshProUGUI _toopTipText;

    protected Slider _slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        _toopTipText = UICanvas.GetComponentInChildren<TextMeshProUGUI>();
        _slider = UICanvas.GetComponentInChildren<Slider>();
        _slider.enabled = false;
        _toopTipText.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        CharacterController _controller = col.GetComponent<CharacterController>();
        if (_controller != null)
        {
            _controller.EnteredPond();
            if (_toopTipText)
            {
                _toopTipText.enabled = true;
            }
        }
    }
}
