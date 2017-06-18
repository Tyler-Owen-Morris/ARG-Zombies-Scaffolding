using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainSpawner : MonoBehaviour {

	public WhackABrain brainPrefab;
	public float timeRemaining, minSec, maxSec;

	private bool brainSpwanClockRunning; //this is off while a brain is active on screen- on to count-down to next spawn

	void Start () {
		brainPrefab = Resources.Load<WhackABrain>("Prefabs/Brain Prefab");

		if (brainPrefab==null) {
			Debug.LogWarning("Unable to locate Brain Prefab");
		}

		//Locate all existing brain prefabs, and destroy them **clear the board**
		GameObject[] brains = GameObject.FindGameObjectsWithTag("whackbrain");
		if (brains != null) {
			foreach(GameObject brain in brains){
				Destroy(brain); 
			}
		}

		brainSpwanClockRunning = true; //initially there are no brains, so start the timer
		timeRemaining = Random.Range(minSec, maxSec);
	}

	void Update () {
		if (brainSpwanClockRunning) {
			if (timeRemaining > 0) {
				//continue the timer
				timeRemaining=(timeRemaining-Time.deltaTime);
			}else {
				//trigger the spawn
				brainSpwanClockRunning=false;
				SpawnBrain();
			}
		}
	}

	void SpawnBrain () {
		/**
		RectTransform myRT = this.gameObject.GetComponent<RectTransform>();
		float max_x = myRT.right.x;
		**/

		if (brainPrefab==null) {
			brainPrefab = Resources.Load<WhackABrain>("Prefabs/Brain Prefab");
			if (brainPrefab==null) {
				Debug.LogWarning("Unable to locate Brain Prefab");
			}
		}

		WhackABrain instance = Instantiate(brainPrefab, this.gameObject.transform);
		instance.currWhacks = instance.maxWhacks; //initialize the script
		Vector3 offset = new Vector3(250, 250, 0);
		instance.transform.position = instance.transform.position+offset;
	}

}
