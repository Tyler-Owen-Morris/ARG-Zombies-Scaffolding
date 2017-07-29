using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainSpawner : MonoBehaviour {

	public WhackABrain brainPrefab;
	public float timeRemaining, minSec, maxSec;
	public GameObject canvas;

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

		canvas = GameObject.Find("Canvas");
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

		/*
		Vector3 center = new Vector3(Screen.width/2, Screen.height/2, 0);
		float x_variance = Screen.width/4;
		float y_variance = Screen.height/4;
		float x_loc = Random.Range((-1*x_variance), x_variance);
		float y_loc = Random.Range((-1*y_variance), y_variance);
		Vector3 pos = new Vector3(center.x+x_loc, center.y+y_loc, 0);
		*/
		float x_variance = Screen.width/4;
		float y_variance = Screen.height/4;
		float x_loc = Random.Range(0.0f, Screen.width-x_variance);
		float y_loc = Random.Range(0.0f, Screen.height-y_variance);
		Vector3 pos = new Vector3(x_loc, y_loc, 0.0f);


		if (brainPrefab==null) {
			brainPrefab = Resources.Load<WhackABrain>("Prefabs/Brain Prefab");
			if (brainPrefab==null) {
				Debug.LogWarning("Unable to locate Brain Prefab");
			}
		}

		WhackABrain instance = Instantiate(brainPrefab, this.gameObject.transform);
		instance.transform.position = pos;
		Debug.Log("Brain located at: "+instance.transform.position.ToString());
		instance.currWhacks = instance.maxWhacks; //initialize the script
		Vector3 offset = new Vector3(250, 250, 0);
		instance.transform.position = instance.transform.position+offset;
	}


	public void ResetBrainSpawner () {
		timeRemaining = Random.Range(minSec, maxSec);
		brainSpwanClockRunning = true;
		Debug.Log("Brain Spawner reset with a time of: " + timeRemaining.ToString());
	}
}
