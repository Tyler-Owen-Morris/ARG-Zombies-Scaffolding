using UnityEngine;
using System.Collections;

public class SetStartVolume : MonoBehaviour {

	private MusicManager musicManager;
	
	// Use this for initialization
	void Start () {
		musicManager = GameObject.FindObjectOfType<MusicManager>();
		if (musicManager) {
			Debug.Log("Music Manager found: " + musicManager+" setting volume to: "+PlayerPrefsManager.GetMasterVolume());
		 	musicManager.ChangeVolume (PlayerPrefsManager.GetMasterVolume());
		} else {
			Debug.LogWarning("no music manager found");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
