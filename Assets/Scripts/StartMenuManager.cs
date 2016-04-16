using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour {

	[SerializeField]
	private Text daysSurvived, activeSurvivors, membersAlive, homebaseLat, homebaseLong, supply;

	//private GameManager gameManager;
	
	void Awake () {
		
		//gameManager = FindObjectOfType<GameManager>();
	}

	void OnLevelWasLoaded () {
		UpdateTheText();
	}

	void UpdateTheText () {
		daysSurvived.text = "Days Survived:      " + GameManager.instance.daysSurvived.ToString();
		activeSurvivors.text = "Active Survivors:      " + GameManager.instance.survivorsActive.ToString();
		membersAlive.text = "Members Alive:               " + GameManager.instance.totalSurvivors.ToString();
		supply.text = "Supply:                 " + GameManager.instance.supply.ToString();
		if (homebaseLat != null && homebaseLong != null) {
			homebaseLat.text = "lat: " + GameManager.instance.homebaseLat.ToString();
			homebaseLong.text = "long: " + GameManager.instance.homebaseLong.ToString();
		}
	}

	public void SetHomeLocation () {
		float lat = Input.location.lastData.latitude;
		float lon = Input.location.lastData.longitude;

		GameManager.instance.SetHomebaseLocation(lat, lon);
		//gameManager.homebaseLat = LocationInfo.latitude;
		//gameManager.homebaseLong = LocationInfo.longitude;
		UpdateTheText();
	}
}
