using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//this is just meant to run the constantly updating output from Location.lastdata


public class CurrentLocationUpdater : MonoBehaviour {
	
	[SerializeField]
	private Text latText, lonText;

	// Use this for initialization
	void Start () {
		InvokeRepeating("SetUpGPSText", 10f, 0.0f);
	}
	
	IEnumerator SetUpGPSText () {
		//ensure services are running
		if (Input.location.status == LocationServiceStatus.Running) {
			//wait for last data to be updated
			yield return Input.location.lastData;
			//set the text
			latText.text = "Current Longitude: " + Input.location.lastData.latitude.ToString();
			lonText.text = "Current Latitude:  " + Input.location.lastData.longitude.ToString();
			yield break;
			
		} else {
			Debug.Log ("can't update text- location services not running");
			CancelInvoke("SetUpGPSText");
		}
	}
}
