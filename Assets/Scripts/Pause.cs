using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    [SerializeField] public string mainMenu;
    [SerializeField] GameObject pauseScreen;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start() {
        this.pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            this.Pause();
        }
    }

    public void Pause() {
        Time.timeScale = 0f;
        this.pauseScreen.SetActive(true);
    }

    public void Resume() {
        Time.timeScale = 1f;
        this.pauseScreen.SetActive(false);
    }

    public void Restart() {
        Scene currScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currScene.name);
    }

    public void Home() {
        SceneManager.LoadScene(this.mainMenu);
    }
}
