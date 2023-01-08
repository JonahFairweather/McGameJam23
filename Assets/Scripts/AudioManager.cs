using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    // singelton instance
    public static AudioManager Instance = null;

    [SerializeField] public AudioSource EffectsSource;
	[SerializeField] public AudioSource MusicSource;

	private float lowPitchRange = .95f;
	private float highPitchRange = 1.05f;
    private float fadeOutDuration = 1.0f;
    private bool musicStopped = true;

    private void Awake() { 
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            GameObject.Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // play the given audio clip through the effects source
    public void PlayEffect(AudioClip clip) {
        EffectsSource.PlayOneShot(clip);
    }

    // play the given audio clip through the music source
    public void PlayMusic(AudioClip clip) {
        // spin until previous music is faded out
        StartCoroutine(FadeOutMusic());
        while (this.musicStopped);

        // play new music
        MusicSource.clip = clip;
        MusicSource.Play();
        this.musicStopped = false;
    }

    // play a random audio clip from an array of audio clips and randomize the pitch slightly 
    public void RandomSoundEffect(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		EffectsSource.pitch = randomPitch;
		EffectsSource.clip = clips[randomIndex];
		EffectsSource.Play();
    }

    IEnumerator FadeOutMusic() {
        // Check Music Volume and Fade Out
        while (MusicSource.volume > 0.01f)
        {
            MusicSource.volume -= Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        // Make sure volume is set to 0
        MusicSource.volume = 0;

        // Stop Music
        MusicSource.Stop();
        this.musicStopped = true;
    }


}
