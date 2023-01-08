using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour {
    // singelton instance
    public static AudioManager Instance = null;

    [SerializeField] public AudioSource EffectsSource;
	[SerializeField] public AudioSource MusicSource;

	private float lowPitchRange = .95f;
	private float highPitchRange = 1.05f;
    private float fadeOutDuration = 3f;

    private float volume = 1f;

    private void Awake() { 
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            GameObject.Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    // play the given audio clip through the effects source
    public void PlayEffect(AudioClip clip) {
        EffectsSource.PlayOneShot(clip);
    }

    // play the given audio clip through the music source
    public void PlayMusic(AudioClip clip) {
        MusicSource.clip = clip;
        this.MusicSource.Play();
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
		MusicSource.Play();
    }

    public bool IsPlayingMusic() {
        return MusicSource.isPlaying;
    }

    public void SetVolume(float volumeLevel) {
        this.volume = Mathf.Clamp(volumeLevel, 0f, 1f);
        this.MusicSource.volume = this.volume;
        this.EffectsSource.volume = this.volume;
    }

    public float GetVolume() {
        return this.volume;
    }
}
