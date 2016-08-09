using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopulatedBuilding : MonoBehaviour {

	public int zombiePopulation = 0 ;
	public string buildingName;
	public bool active;
	public float myLat, myLng;


	private MapLevelManager mapLevelManager;

	void Awake () {
		GenerateZombies();
		mapLevelManager = FindObjectOfType<MapLevelManager>();
		active = true;
	}

	void GenerateZombies () {
		int zombies = Random.Range ( 1, 20);
		zombiePopulation = zombies;
	}

	public void ClickedOn () {
		//GameManager.instance.LoadIntoCombat(zombiePopulation, buildingName);
		if (this.active == true) {
			mapLevelManager.ActivateBuildingInspector(zombiePopulation, buildingName, myLat, myLng);
		} else {
			Debug.Log (gameObject.name+" reports inactive, and will not launch bldg inspector");
		}
	}

	public void DeactivateMe () {
		//Debug.Log ("calling to deactivate "+gameObject.name);
		this.zombiePopulation = 0;
		this.active = false;
		gameObject.GetComponent<Image>().color = Color.gray;

		//Debug.Log ("Deactivate function has completed for " + this.gameObject.name + " and currently has " + this.zombiePopulation.ToString() + " zombies");
		//still need to write the code to change appearance, turn on transparent panel? indicate that it's clear.
	}
}
