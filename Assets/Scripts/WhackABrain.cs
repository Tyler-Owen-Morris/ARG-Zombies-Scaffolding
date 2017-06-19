using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhackABrain : MonoBehaviour {

	public int maxWhacks, currWhacks;
	public Image displayImage;
	public Sprite[] theSprites;
	public AudioClip bite1;
	public AudioSource myAudioSource;

	private ZombieModeManager myLevelManager;

	void Start () {
		displayImage = gameObject.GetComponent<Image>();
		maxWhacks = 3;
		currWhacks=maxWhacks;
		displayImage.sprite = theSprites[0];//reset image

		myLevelManager = FindObjectOfType<ZombieModeManager>();
		if (myLevelManager==null) {
			Debug.LogWarning("Unable to Locate the Zombie Mode level Manager");
		}

		myAudioSource = this.GetComponent<AudioSource>();
	}
	
	public void Whack () {
		Debug.Log("Brain Whacked");

		if (currWhacks >1) {
			currWhacks --;
			if (currWhacks==2) {
				displayImage.sprite = theSprites[1];
			}else if (currWhacks==1){
				displayImage.sprite = theSprites[2];
			}

			myAudioSource.PlayOneShot(bite1);
		} else {
			//brain is expired.
			//notify the level manager- who will handle updating UI and server.
			if (myLevelManager!=null){
				myAudioSource.PlayOneShot(bite1);
				displayImage.sprite = theSprites[3]; //disable the sprite
				myLevelManager.BrainEaten(this.gameObject, bite1.length);
			}else{
				Debug.LogError("Level manager not found");
			}
		}
	}
}
