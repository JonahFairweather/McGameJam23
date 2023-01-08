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
    private List< List<Sprite> > Sprites = new List< List<Sprite> >();
    [SerializeField] List<Sprite> Frame1;
    [SerializeField] List<Sprite> Frame2;
    [SerializeField] List<Sprite> Frame3;
    [SerializeField] List<Sprite> Frame4;
    [SerializeField] List<Sprite> Frame5;

    public float TimeBetweenFrames = 10f;

    public Image ToSet;

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
        this.Sprites.Add(Frame1);
        this.Sprites.Add(Frame2);
        this.Sprites.Add(Frame3);
        this.Sprites.Add(Frame4);
        this.Sprites.Add(Frame5);

        PlayCinematic();
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

    private void PlayCinematic()
    {
        
        // _spriteRenderer.sprite = SpriteList[i];
        // i++;
        // float timeperframe = CinematicTheme.length / SpriteList.Count;
        
        // yield return new WaitForSeconds(timeperframe);
        // while (i < SpriteList.Count)
        // {
        //     _spriteRenderer.sprite = SpriteList[i];
        //     i++;
        //     yield return new WaitForSecondsRealtime(timeperframe);
        // }

        // this.sceneTransition.LoadNewScene(this.LoadOnceDone);
        int totalSprites = this.Sprites.Aggregate(0, (i, next) => i += next.Count);
        float timePerCutScene = CinematicTheme.length / 5;
        int i = 0;
        foreach (var frame in Sprites)
        {
            int numReps = (int) (timePerCutScene / (frame.Count * this.TimeBetweenFrames));
            // StartCoroutine(PlayFrame(frame));
            // i = 0;
            // var startTime = Time.time;
            // InvokeRepeating("PlayFrame", 0f, 0.05f * frame.Count);
            // if (Time.time >= startTime + timePerCutScene) CancelInvoke("PlayFrame");
            // currFrameNum ++;
            for (int j = 0; j < numReps; j++)
            {
                StartCoroutine(PlayFrame(frame, timePerCutScene * i + (frame.Count * this.TimeBetweenFrames) * j));
            }
            i ++;
        }
    }

    public IEnumerator PlayFrame(List<Sprite> frame, float delaySeconds) {
        // _spriteRenderer.sprite = SpriteList[i];
        // i++;
        // float timeperframe = 0.05f;
        
        // yield return new WaitForSeconds(timeperframe);
        // while (i < frame.count)
        // {
        //     _spriteRenderer.sprite = frame[i];
        //     i++;
        //     yield return new WaitForSecondsRealtime(timeperframe);
        // }
        yield return new WaitForSeconds(delaySeconds);
        int i = 0;
        foreach (Sprite f in frame)
        {
            StartCoroutine(DisplayFrame(f, this.TimeBetweenFrames * i));
            i++;
        }
    }

    private IEnumerator DisplayFrame(Sprite frame, float delaySeconds) {
        yield return new WaitForSeconds(delaySeconds);
        _spriteRenderer.sprite = frame;
    }
}
