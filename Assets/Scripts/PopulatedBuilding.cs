using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System;

public class PopulatedBuilding : MonoBehaviour {

	public int zombiePopulation = 0 ;
	public string buildingName, buildingID;
	public Button button;
	public bool active = false;
	public float myLat, myLng;


	private MapLevelManager mapLevelManager;

	void Awake () {
		//GenerateZombies(); //now being called from building spawner.
		mapLevelManager = FindObjectOfType<MapLevelManager>();
	}

	public void GenerateZombies () {
		int zombies = UnityEngine.Random.Range ( 1, 20);
		zombiePopulation = zombies;
		Debug.Log(buildingName+" has woken up and is generating "+zombies+" zombies; active: "+active.ToString());
	}

	void Start () {
		//CheckForDeactivation();
	}

	/*
	public void CheckForDeactivation() {
		Debug.Log(gameObject.name+" is checking to see if they're deactivated, ID: "+buildingID);
		if (GameManager.instance.clearedBldgJsonText != "") {
			JsonData cleared_bldg_json = JsonMapper.ToObject(GameManager.instance.clearedBldgJsonText);
			DateTime minus12hr = DateTime.Now - TimeSpan.FromHours(12);

			for (int i = 0; i < cleared_bldg_json.Count; i++) {
				DateTime clear_time = DateTime.Parse(cleared_bldg_json[i]["time_cleared"].ToString());
				if (cleared_bldg_json[i]["bldg_id"].ToString() == buildingID && clear_time<minus12hr) {
					Debug.Log(buildingName+" has found their deactivated");
					DeactivateMe();
					break;
				} else {
					continue;
				}
			}
			Debug.Log(gameObject.name+" has concluded they are active");
		}
	}
	*/

	public void ClickedOn () {
		//GameManager.instance.LoadIntoCombat(zombiePopulation, buildingName);
		if (this.active == true) {
			mapLevelManager.ActivateBuildingInspector(zombiePopulation, buildingName, buildingID, myLat, myLng);
		} else {
			Debug.Log (gameObject.name+" reports inactive, and will not launch bldg inspector");
		}
	}

	public void ActivateMe () {
		active = true;
		Image myImage = this.gameObject.GetComponent<Image>();
		if (myImage != null) {
			myImage.color = Color.white;
		}
		if (button != null) {
			button.interactable = true;
		}

		Debug.Log("Activation complete on: "+gameObject.name+" active status is: "+active.ToString()+" and zombie population: "+zombiePopulation.ToString());
	}

	public void DeactivateMe () {
		//StartCoroutine(DelayDeactivation());
		zombiePopulation = 0;
		active = false;
		Image myImage = this.gameObject.GetComponent<Image>();
		if (myImage != null) {
			myImage.color = Color.gray;
			Debug.Log(buildingName+" is turning it's color to "+myImage.color.ToString());
		}else{
			Debug.Log(buildingName+" unable to find Image component");
		}

		if (button != null) {
			button.interactable = false;
			Debug.Log(buildingName+" has set Button component interactable to: "+button.interactable.ToString());
		} else {
			Debug.Log(buildingName+" unable to find Button component");
		}
		Debug.Log ("Deactivation complete on: "+gameObject.name+" active status is: "+active.ToString()+" and zombie population: "+zombiePopulation.ToString());
		//Debug.Log ("Deactivate function has completed for " + this.gameObject.name + " and currently has " + this.zombiePopulation.ToString() + " zombies");
		//still need to write the code to change appearance, turn on transparent panel? indicate that it's clear.
	}

	IEnumerator DelayDeactivation () {
		yield return new WaitForSeconds(0.2f);

	}
}
