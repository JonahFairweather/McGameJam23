using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CinematicController : MonoBehaviour
{

    public List<Sprite> SpriteList;

    public float TimeBetweenFrames = 0.1f;

    public Image ToSet;

    private int i;

    protected SpriteRenderer _spriteRenderer;
    protected bool _polled;

    [SerializeField] private AudioClip CinematicTheme;

    [SerializeField] private String LoadOnceDone;

    [SerializeField] public SceneTransition sceneTransition;
    [SerializeField] public KeyCode skipKeycode = KeyCode.Space;
    
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        i = 0;
        StartCoroutine(PlayNextScene());
        _polled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_polled)
        {
            PollAudioPlayer();
        }

        if (Input.GetKeyDown(skipKeycode))
        {
            StopAllCoroutines();
            sceneTransition.LoadNewScene(LoadOnceDone);
        }
    }

    private void PollAudioPlayer()
    {
        if (AudioManager.Instance != null)
        {
            _polled = true;
            AudioManager.Instance.PlayMusic(CinematicTheme);
        }
    }

    private IEnumerator PlayNextScene()
    {
        
        _spriteRenderer.sprite = SpriteList[i];
        i++;
        float timeperframe = CinematicTheme.length / SpriteList.Count;
        
        yield return new WaitForSeconds(timeperframe);
        while (i < SpriteList.Count)
        {
            _spriteRenderer.sprite = SpriteList[i];
            i++;
            yield return new WaitForSecondsRealtime(timeperframe);
        }

        this.sceneTransition.LoadNewScene(this.LoadOnceDone);
    }
}
