using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {
    private float transitionTime = 1f;
    public Animator transition;

    public void LoadNextScene() {
        StartCoroutine(LoadNewSceneFromIndex(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadNewScene(string sceneName) {
        StartCoroutine(LoadNewSceneFromName(sceneName));
    }

    IEnumerator LoadNewSceneFromIndex(int sceneIndex) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator LoadNewSceneFromName(string sceneName) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
