using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Building : MonoBehaviour {

	public int zombiePopulation;

	private bool buildingClear;
	
	// Use this for initialization
	void Start () {
			GenerateZombies();
			buildingClear = false;

	}

	void OnLevelWasLoaded () {
		CheckIfThisBuildingIsClear();
	}
	
	public void BuildingPressed () {
		Debug.Log ("Building "+ gameObject.name +" has been triggered");
		if (buildingClear == false) { //only load combat if the building is not clear
			//combatManager.SetZombiesEncountered (zombiePopulation);
			GameManager.instance.LoadIntoCombat(zombiePopulation, this.gameObject.name);
		} else {
			Debug.Log("Building thinks it's already been cleared");
		}
	}

	private void CheckIfThisBuildingIsClear () {
		if (this.gameObject.name == "Building01" && GameManager.instance.buildingToggleStatusArray[0] == true) {
			DeactivateMe();
		} else if (this.gameObject.name == "Building02" && GameManager.instance.buildingToggleStatusArray[1] == true) {
			DeactivateMe();
		} else if (this.gameObject.name == "Building03" && GameManager.instance.buildingToggleStatusArray[2] == true) {
			DeactivateMe();
		} else if (this.gameObject.name == "Building04" && GameManager.instance.buildingToggleStatusArray[3] == true) {
			DeactivateMe();
		}
	}

	public void DeactivateMe () {
		this.buildingClear = true;
		this.GetComponent<BoxCollider2D>().enabled = false;
		this.GetComponent<Image>().color = Color.gray;
		this.GetComponent<Button>().enabled = false;

		//still need to write the code to change appearance, turn on transparent panel? indicate that it's clear.
	}

	void GenerateZombies () {
		int zombies = Random.Range ( 1, 10);
		zombiePopulation = zombies;
	}

	void Update () {
		
	}
}
