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
    private float fadeOutDuration = 3f;

    private void Awake() { 
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            GameObject.Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // play the given audio clip through the effects source
    public void PlayEffect(AudioClip clip) {
        EffectsSource.PlayOneShot(clip);
    }

    // play the given audio clip through the music source
    public void PlayMusic(AudioClip clip) {
        MusicSource.clip = clip;
        StartCoroutine(FadeOutMusic());
    }

    // play a random audio clip from an array of audio clips and randomize the pitch slightly. Plays through the effects source 
    public void PlayRandomEffect(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		EffectsSource.pitch = randomPitch;
		EffectsSource.PlayOneShot(clips[randomIndex]);
    }

    // play a random audio clip from an array of audio clips and randomize the pitch slightly. Plays through the music source
    public void PlayRandomMusic(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
		MusicSource.clip = clips[randomIndex];
		StartCoroutine(FadeOutMusic());
    }

    IEnumerator FadeOutMusic() {
        // Check Music Volume and Fade Out
        while (MusicSource.volume > 0.01f) {
            MusicSource.volume -= Time.fixedDeltaTime / fadeOutDuration;
            yield return null;
        }

        // Stop Music
        MusicSource.Stop();

        // play new music
        MusicSource.volume = 1;
        MusicSource.Play();

        yield break;
    }

    public bool IsPlayingMusic() {
        return MusicSource.isPlaying;
    }
}
