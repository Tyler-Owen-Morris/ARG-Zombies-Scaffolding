using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public int zombiePopulation;

	private bool buildingClear = false;
	
	// Use this for initialization
	void Start () {
			GenerateZombies();
	}
	
	public void BuildingPressed () {
		Debug.Log ("A building has been triggered");
		if (!buildingClear) { //only load combat if the building is not clear
			//combatManager.SetZombiesEncountered (zombiePopulation);
			GameManager.instance.LoadIntoCombat(zombiePopulation);
		}
	}

	public void ThisBuildingIsClear () {
		buildingClear = true;

		//still need to write the code to change appearance, turn on transparent panel? indicate that it's clear.
	}

	void GenerateZombies () {
		int zombies = Random.Range ( 1, 10);
		zombiePopulation = zombies;
	}

	void Update () {
		
	}
}
