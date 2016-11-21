using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

	public AudioClip[] levelMusicChangeArray;
	private static MusicManager instance;
	
	private AudioSource audioSource;
	private Scene activeScene;
	
	// Use this for initialization
	void Awake () {
		MakeSingleton();
	}
	
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
		activeScene = SceneManager.GetActiveScene();
		AudioClip lastLevelMusic = GetComponent<AudioSource>().clip;
		AudioClip thislevelMusic = levelMusicChangeArray[activeScene.buildIndex];
		
		if ( thislevelMusic != lastLevelMusic) {
			//Debug.Log ("playing clip:" + thislevelMusic);
			if (thislevelMusic && audioSource != null){
				audioSource.clip = thislevelMusic;
				audioSource.loop = true;
				audioSource.Play ();
			}
		} else {
			//Debug.Log ("resuming same music");
			audioSource.loop = true;
			//audioSource.Play ();
		}
	}

	void MakeSingleton() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}
	
	public void PlayAudioClipOnce (int clipNumber) {
		AudioClip playThisClip = levelMusicChangeArray[clipNumber];
		
		if (playThisClip) {
			audioSource.clip = playThisClip;
			audioSource.loop = false;
			audioSource.Play ();
		}
	}
	
	public void ChangeVolume (float volume) {
		audioSource.volume = volume;
	}
}
