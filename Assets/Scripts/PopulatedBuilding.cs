using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System;

public class PopulatedBuilding : MonoBehaviour {

	public int zombiePopulation = 0, wood_inside, metal_inside, food_inside, water_inside;
    public bool has_traps, has_barrel, has_greenhouse, has_my_tag=false;
	public DateTime last_cleared, last_looted_supply, last_looted_food, last_looted_water;
	public string buildingName, buildingID, google_type, loot_code, photo_reference;
    public GameObject name_on_wall_image, clear_bldg_image, populated_bldg_image, building_unknown_image, trap_indicator_image, barrel_indicator_image, greenhouse_indicator_image;
    public Button button;
	public bool active = false;
	public float myLat, myLng;


	private MapLevelManager mapLevelManager;

	void Awake () {
		//GenerateZombies(); //now being called from building spawner.
		mapLevelManager = FindObjectOfType<MapLevelManager>();
        name_on_wall_image.SetActive(false);
	}

	public void GenerateZombies () {
		int zombies = UnityEngine.Random.Range ( 1, 20);
		zombiePopulation = zombies;
		//Debug.Log(buildingName+" has woken up and is generating "+zombies+" zombies; active: "+active.ToString());
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

        this.gameObject.tag = "Untagged"; //remove the building tag from the gameobject

	    mapLevelManager.ActivateBuildingInspector(this.gameObject.GetComponent<PopulatedBuilding>());
	}

    public void SetToUnknown()
    {
        clear_bldg_image.SetActive(false);
        populated_bldg_image.SetActive(false);
        //name_on_wall_image.SetActive(false);
        building_unknown_image.SetActive(true);
    }
    
    public void SetToPopulated()
    {
        clear_bldg_image.SetActive(false);
        populated_bldg_image.SetActive(true);
        //name_on_wall_image.SetActive(false);
        building_unknown_image.SetActive(false);
    }

    public void SetToClear ()
    {
        clear_bldg_image.SetActive(true);
        populated_bldg_image.SetActive(false);
        //name_on_wall_image.SetActive(false);
        building_unknown_image.SetActive(false);
    }

    public void Tagged ()
    {
        clear_bldg_image.SetActive(false);
        populated_bldg_image.SetActive(false);
        name_on_wall_image.SetActive(true);
        building_unknown_image.SetActive(false);
    }

    public void UnTagged ()
    {
        clear_bldg_image.SetActive(false);
        populated_bldg_image.SetActive(false);
        name_on_wall_image.SetActive(false);
        building_unknown_image.SetActive(false);
    }

	public void ActivateMe () {
		active = true;
        if (has_my_tag == false)
        {
            name_on_wall_image.SetActive(false);
        }else
        {
            name_on_wall_image.SetActive(true);
        }

		Image myImage = this.gameObject.GetComponent<Image>();
		if (myImage != null) {
			myImage.color = Color.white;
		}
		if (button != null) {
			button.interactable = true;
		}

        //Handle which sprite is activated- based on last clear time- unknown is default from spawner
        if ((DateTime.Now-last_cleared) < TimeSpan.FromHours(4f))
        {
            SetToClear();
        }
        //1201am Jan1 2000 is code for entered, but not cleared.
        else if (last_cleared == DateTime.Parse("12:01am 01/01/2000"))
        {
            SetToPopulated();
        }else if (last_cleared == DateTime.Parse("11:59pm 12/31/1999"))
        {
            SetToUnknown();
        }

        if (has_traps == true)
        {
            trap_indicator_image.SetActive(true);
            SetToClear();
        }else
        {
            trap_indicator_image.SetActive(false);
        }
        if (has_barrel == true)
        {
            barrel_indicator_image.SetActive(true);
        }else
        {
            barrel_indicator_image.SetActive(false);
        }
        if (has_greenhouse == true)
        {
            greenhouse_indicator_image.SetActive(true);
        }
        else
        {
            greenhouse_indicator_image.SetActive(false);
        }

        //Debug.Log("Activation complete on: "+gameObject.name+" active status is: "+active.ToString()+" and zombie population: "+zombiePopulation.ToString());
    }

	public void DeactivateMe () {
		//StartCoroutine(DelayDeactivation());
		zombiePopulation = 0;
		active = false;
		Image myImage = this.gameObject.GetComponent<Image>();
		if (myImage != null) {
			//Debug.Log(buildingName+" is turning it's color to "+myImage.color.ToString());
		}else{
			Debug.Log(buildingName+" unable to find Image component");
		}

		if (button != null) {
			button.interactable = false;
			//Debug.Log(buildingName+" has set Button component interactable to: "+button.interactable.ToString());
		} else {
			Debug.Log(buildingName+" unable to find Button component");
		}
		//Debug.Log ("Deactivation complete on: "+gameObject.name+" active status is: "+active.ToString()+" and zombie population: "+zombiePopulation.ToString());
		//Debug.Log ("Deactivate function has completed for " + this.gameObject.name + " and currently has " + this.zombiePopulation.ToString() + " zombies");
		//still need to write the code to change appearance, turn on transparent panel? indicate that it's clear.
	}

	IEnumerator DelayDeactivation () {
		yield return new WaitForSeconds(0.2f);

	}
}
