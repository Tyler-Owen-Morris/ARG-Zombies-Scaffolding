using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//this is just meant to run the constantly updating output from Location.lastdata


public class CurrentLocationUpdater : MonoBehaviour {
	
	[SerializeField]
	private Text latText, lonText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		latText.text = "Current Longitude: " + Input.location.lastData.latitude.ToString();
		lonText.text = "Current Latitude:  " + Input.location.lastData.longitude.ToString();
	}
}
