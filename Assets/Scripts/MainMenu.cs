using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public string gameScene;
    [SerializeField] public GameObject howToPlayScreen; 
    [SerializeField] public GameObject settingsScreen;
    [SerializeField] public AudioClip backgroundAudio;

    // Start is called before the first frame update
    void Start()
    {
        this.howToPlayScreen.SetActive(false);
        this.settingsScreen.SetActive(false);
        while(AudioManager.Instance != null);
        AudioManager.Instance.PlayMusic(this.backgroundAudio);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() {
        SceneManager.LoadScene(gameScene);
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
}
