using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip[] levelMusicChangeArray;
	
	private AudioSource audioSource;
	
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (gameObject);
		Debug.Log ("Don't destroy on load" + name);
	}
	
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	void OnLevelWasLoaded (int level) {
		AudioClip lastLevelMusic = GetComponent<AudioSource>().clip;
		AudioClip thislevelMusic = levelMusicChangeArray[level];
		
		if ( thislevelMusic != lastLevelMusic) {
			Debug.Log ("playing clip:" + thislevelMusic);
			if (thislevelMusic){
				audioSource.clip = thislevelMusic;
				audioSource.loop = true;
				audioSource.Play ();
			}
		} else {
			Debug.Log ("resuming same music");
			audioSource.loop = true;
			//audioSource.Play ();
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
	
	// Update is called once per frame
	void Update () {
	
	}
}
