using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PenguinDead : MonoBehaviour {
    [SerializeField] public Sprite[] gameOverScreens;  // 0 -> Foxy; 1 -> Sassy
    [SerializeField] public string mainMenu;
    [SerializeField] public SceneTransition sceneTransition;
    [SerializeField] public GameObject gameOverScreen;
    [SerializeField] public Image gameOverImage;

    // Start is called before the first frame update
    void Start() {
        this.gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void GameOver(string murdererName) {
        Debug.Log("GAME OVER !!");
        DisplayMurderer(murdererName);
        Time.timeScale = 0f;
        this.gameOverScreen.SetActive(true);
    }

    private void DisplayMurderer(string murdererName) {
        Debug.Log("The " + murdererName + " killed Jimbo :(");
        if (murdererName == "Fox") {
            // this.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = this.gameOverScreens[0];
            this.gameOverImage.sprite = this.gameOverScreens[0];
        } else {
            // this.gameObject.GetComponent<SpriteRenderer>().sprite = this.gameOverScreens[1];
            this.gameOverImage.sprite = this.gameOverScreens[1];
        }
    }

    public void Restart() {
        Scene currScene = SceneManager.GetActiveScene();
        Time.timeScale = 1f;
        this.sceneTransition.LoadNewScene(currScene.name);
        AudioSource[] sources = Object.FindObjectsOfType<AudioSource>();
        foreach (AudioSource s in sources)
        {
            s.Stop();
        }
    }

    public void Home() {
        Time.timeScale = 1f;
        this.sceneTransition.LoadNewScene(this.mainMenu);
    }

    public void Quit() {
        Application.Quit();
    }
}
