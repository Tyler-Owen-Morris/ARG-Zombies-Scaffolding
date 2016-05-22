using UnityEngine;
using System.Collections;

public class PopulatedBuilding : MonoBehaviour {

	public int zombiePopulation = 0 ;
	public string buildingName;
	public bool active;

	private MapLevelManager mapLevelManager;

	void Awake () {
		GenerateZombies();
		mapLevelManager = FindObjectOfType<MapLevelManager>();
	}

	void GenerateZombies () {
		int zombies = Random.Range ( 1, 20);
		zombiePopulation = zombies;
	}

	public void ClickedOn () {
		//GameManager.instance.LoadIntoCombat(zombiePopulation, buildingName);

		mapLevelManager.ActivateBuildingInspector(zombiePopulation, buildingName);
	}
}
