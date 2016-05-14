using UnityEngine;
using System.Collections;

public class PopulatedBuilding : MonoBehaviour {

	public int zombiePopulation = 0 ;
	public string buildingName;
	public bool active;

	void Awake () {
		GenerateZombies();	
	}

	void GenerateZombies () {
		int zombies = Random.Range ( 1, 10);
		zombiePopulation = zombies;
	}

	public void ClickedOn () {
		GameManager.instance.LoadIntoCombat(zombiePopulation, buildingName);
	}
}
