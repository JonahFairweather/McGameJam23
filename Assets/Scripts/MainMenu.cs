using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    [SerializeField] public string gameScene;
    [SerializeField] public GameObject howToPlayScreen; 
    [SerializeField] public GameObject settingsScreen;

    [SerializeField] public AudioClip backgroundAudio;

    [SerializeField] public SceneTransition sceneTransition;

    private bool _polledAudioInstance;

    // Start is called before the first frame update
    void Start() {
        this.howToPlayScreen.SetActive(false);
        this.settingsScreen.SetActive(false);
        // AudioManager.Instance.PlayMusic(this.backgroundAudio);
    }

    // Update is called once per frame
    void Update() {
        if (!_polledAudioInstance)
        {
            PollAudioInstance();
        }

        HandleBackgroundMusic();
    }

    void PollAudioInstance()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(this.backgroundAudio);
            _polledAudioInstance = true;
        }
    }

    private void HandleBackgroundMusic() {
        if(!AudioManager.Instance.IsPlayingMusic()) {
            AudioManager.Instance.PlayMusic(backgroundAudio);
        }
    }

    public void StartGame() {
        // SceneManager.LoadScene(gameScene);
        this.sceneTransition.LoadNewScene(this.gameScene);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void OpenSettings() {
        this.settingsScreen.SetActive(true);
    }

    public void CloseSettings() {
        this.settingsScreen.SetActive(false);
    }

    public void EasterJimbo_ed() {
        // do something silly here
    }

    public void OpenHowTo() {
        this.howToPlayScreen.SetActive(true);
    }

    public void CloseHowTo() {
        this.howToPlayScreen.SetActive(false);
    }

    public void VolumeUp()
    {
        //INCREASE VOLUME
    }

    public void VolumeDown()
    {
        //DECREASE VOLUME
    }
}
