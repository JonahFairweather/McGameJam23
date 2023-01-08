using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] public string mainMenu;
    [SerializeField] GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        this.pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPauseMenu() {
        this.pauseScreen.SetActive(true);
    }

    public void Resume() {
        this.pauseScreen.SetActive(false);
    }

    public void Restart() {
        // restart level
    }

    public void Home() {
        SceneManager.LoadScene(this.mainMenu);
    }
}
