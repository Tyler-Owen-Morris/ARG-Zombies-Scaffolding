using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using LitJson;
using System;

public class MapLevelManager : MonoBehaviour {

	public GameObject inventoryPanel, buildingPanel, qrPanel, homebasePanel, homebaseConfirmationPanel, outpostConfirmationPanel, missionStartConfirmationPanel, OutpostQRPanel, enterBldgButton, unequippedWeaponsPanel, mapLevelCanvas;

	[SerializeField]
	private Text supplyText, daysAliveText, survivorsAliveText, currentLatText, currentLonText, locationReportText, foodText, waterText, ammoText, playerNameText, bldgNameText, zombieCountText, bldgDistText, homebaseLatText, homebaseLonText, missionConfirmationText;

	[SerializeField]
	private Slider playerHealthSlider, playerHealthSliderDuplicate;

	[SerializeField]
	private BuildingSpawner bldgSpawner;

	public Button confirmHomebaseButton, sendATeamButton;
	public Text homebaseConfirmationText;

	public WeaponListPopulator theWeaponListPopulator;
	public SurvivorListPopulator theSurvivorListPopulator;
	public MissionListPopulator theMissionListPopulator;
	public MissionCombatSimulator myMissionCombatSimulator;

	private float lastUpdateLat = 0.0f, lastUpdateLng = 0.0f;
	private float lastStamUpdateLat = 0.0f, lastStamUpdateLng = 0.0f;

	private int zombieCount;
	public string bldgName, active_bldg_id;

	public int active_gearing_survivor_id, to_equip_weapon_id, to_unequip_weapon_id;

	private static GameObject missionCompletePrefab;

	private string updateHomebaseURL = "http://www.argzombie.com/ARGZ_SERVER/UpdateHomebaseLocation.php";
	private string dropoffAndResupplyURL = "http://www.argzombie.com/ARGZ_SERVER/DropoffAndResupply.php";
	private string equipWeaponURL = "http://www.argzombie.com/ARGZ_SERVER/EquipWeapon.php";
	private string unequipWeaponURL = "http://www.argzombie.com/ARGZ_SERVER/UnequipWeapon.php";
	private string promoteSurvivorURL = "http://www.argzombie.com/ARGZ_SERVER/PromoteSurvivor.php";
	private string staminaUpdateURL = "http://www.argzombie.com/ARGZ_SERVER/StaminaRegen.php";
	private string SendNewOutpostURL = "http://www.argzombie.com/ARGZ_SERVER/CreateOutpost.php";

	public void InventoryButtonPressed () {
		if (inventoryPanel.activeInHierarchy == false ) {
			inventoryPanel.SetActive(true);
		} else if (inventoryPanel.activeInHierarchy == true) {
			inventoryPanel.SetActive(false);
		}
	}

	public void HomebaseButtonPressed () {
		if (homebasePanel.activeInHierarchy == false ) {
			homebasePanel.SetActive(true);
		} else if (homebasePanel.activeInHierarchy == true) {
			homebasePanel.SetActive(false);
		}
	}

	public void CancelHomebaseButtonPressed () {
		homebaseConfirmationPanel.SetActive(false);
	}
	public void OutpostButtonPressed () {
		if (outpostConfirmationPanel.activeInHierarchy == true) {
			outpostConfirmationPanel.SetActive(false);
		} else {
			outpostConfirmationPanel.SetActive(true);
		}
	}

	public void QrScanPressed () {
		if (qrPanel.activeInHierarchy == true) {
			qrPanel.SetActive(false);
		} else {
			qrPanel.SetActive(true);
		}
	}

	public void OutpostPressed (int outpost_id) {
		OutpostPanelController OPController = OutpostQRPanel.GetComponent<OutpostPanelController>();
		OPController.SetQRtextAndEncode(outpost_id);
		OutpostQRPanel.SetActive(true);
	}

//	public void PlayerAttemptingPurchaseFullHealth () {
//		GameManager.instance.PlayerAttemptingPurchaseFullHealth();
//	}
//
//	public void PlayerAttemptingPurchaseNewSurvivor () {
//		GameManager.instance.PlayerAttemptingPurchaseNewSurvivor ();
//	}
//
//	public void PlayerAttemptingPurchaseShiv () {
//		GameManager.instance.PlayerAttemptingPurchaseShiv();
//	}
//
//	public void PlayerAttemptingPurchaseClub () {
//		GameManager.instance.PlayerAttemtpingPurchaseClub();
//	}
//
//	public void PlayerAttemptingPurchaseGun () {
//		GameManager.instance.PlayterAttemtptingPurchaseGun();
//	}
//
//
//	public void ResetBuildingsCalled () {
//		GameManager.instance.ResetAllBuildings();
//	}
    
    void Awake () {
    	missionCompletePrefab = Resources.Load<GameObject>("Prefabs/MissionCompletePanelPrefab");

    	if (FB.IsLoggedIn == true)  {
        	FB.API ("/me?fields=name", HttpMethod.GET, DisplayUsername);
        } else {
        	string name;
        	name = GameManager.instance.userFirstName + " " + GameManager.instance.userLastName;
        	playerNameText.text = name;
        }
        bldgSpawner = GameObject.Find("Building Populator").GetComponent<BuildingSpawner>();
    }

	void OnLevelWasLoaded () {
		//UpdateTheUI();
		InvokeRepeating("CheckAndUpdateMap", 10.0f, 10.0f);
		InvokeRepeating("RegenerateStamina", 30.0f, 60.0f);
	}

	void CheckAndUpdateMap () {
		//check if location services are active
		if (Input.location.status == LocationServiceStatus.Running) {
			//if a last location has NOT been logged
			if (lastUpdateLat != 0 && lastUpdateLng != 0) {
				//if you've moved enough, then do the update, otherwise do nothing
				if (CalculateDistanceToTarget(lastUpdateLat, lastUpdateLng)>= 20.0f) {
					//find and destroy all existing buildings
					GameObject[] oldBldgs = GameObject.FindGameObjectsWithTag("building");
					foreach (GameObject oldBldg in oldBldgs) {
						Destroy(oldBldg.gameObject);
					}
					//if player has traveled 250+meters since google api was last pinged, then change the boolean so that it hits the api for new json data.
					if (CalculateDistanceToTarget(bldgSpawner.lastGoogleLat, bldgSpawner.lastGoogleLng) >= 250.0f) {
						bldgSpawner.googleBldgsNeedUpdate = true;
					}
					//call the building creation function
					bldgSpawner.UpdateBuildings();
					//log the last location updated from
					lastUpdateLat = Input.location.lastData.latitude;
					lastUpdateLng = Input.location.lastData.longitude;
				}

			} else {
				//store current location as last updated location and do the update
				lastUpdateLat = Input.location.lastData.latitude;
				lastUpdateLng = Input.location.lastData.longitude;

				//destroy the existing buildings
				GameObject[] oldBldgs = GameObject.FindGameObjectsWithTag("building");
				foreach (GameObject oldBldg in oldBldgs) {
					Destroy(oldBldg.gameObject);
				}

				//create new buildings 
				bldgSpawner.UpdateBuildings();
			}
		}
	}

	public void RegenerateStamina () {
		int stamRegen = 4; //1 stamina every 15 sec, counted every 1min

		if (Input.location.status == LocationServiceStatus.Running) {
			if (lastStamUpdateLat != 0 && lastStamUpdateLng != 0) {
				float distanceFromLastUpdate = CalculateDistanceToTarget(lastStamUpdateLat, lastStamUpdateLng);
				if (distanceFromLastUpdate > 25) {
					if(distanceFromLastUpdate > 295) {
						distanceFromLastUpdate=295;
					}

					int intervals = Mathf.RoundToInt(distanceFromLastUpdate/25);
					stamRegen += intervals;
					lastStamUpdateLat = Input.location.lastData.latitude;
					lastStamUpdateLng = Input.location.lastData.longitude;
				}
			} else {
				lastStamUpdateLat = Input.location.lastData.latitude;
				lastStamUpdateLng = Input.location.lastData.longitude;
			}

			//if in range of valid outpost or homebase- double the regenerated stamina.
			if (AmIInRangeOfValidOutpost()) {
				stamRegen = stamRegen * 2;
			}
		} else {
			Debug.Log("location services not running, no distance bonus for stamina regen");
		}

		//Add the stamina regen to each player card that's not full.
		foreach(GameObject survivor in GameManager.instance.activeSurvivorCardList) {
			SurvivorPlayCard mySurvivor = survivor.GetComponent<SurvivorPlayCard>();
			if (mySurvivor.survivor.curStamina < mySurvivor.survivor.baseStamina) {
				if (mySurvivor.survivor.curStamina+stamRegen > mySurvivor.survivor.baseStamina) {
					mySurvivor.survivor.curStamina = mySurvivor.survivor.baseStamina;
				}else{
					mySurvivor.survivor.curStamina += stamRegen;
				}
			}
		}

		//send the stamina to the server, so it can match records with the client.
		StartCoroutine(SendStaminaUpdate(stamRegen));
		SetStaminaUIText(stamRegen.ToString());
		UpdateTheUI();
	}

	private void SetStaminaUIText (string dmg) {
		StaminaText stamText = Resources.Load<StaminaText>("Prefabs/StaminaPopupText").GetComponent<StaminaText>();
		StaminaText instance = Instantiate(stamText);
		instance.transform.SetParent(mapLevelCanvas.transform, false);
		instance.SetStaminaText(dmg);
	}

	IEnumerator SendStaminaUpdate (int stam) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("stam_regen", stam);

		WWW www = new WWW(staminaUpdateURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
		} else {
			Debug.Log(www.error);
		}
	}


	public void UpdateTheUI () {
        
        
		//left UI panel update
		supplyText.text = "Supply: " + GameManager.instance.supply.ToString();
        foodText.text = "Food: " + GameManager.instance.foodCount.ToString();
        waterText.text = "Water: " + GameManager.instance.waterCount.ToString();
        ammoText.text = "Ammo: " + GameManager.instance.ammo.ToString();
		daysAliveText.text = GameManager.instance.daysSurvived.ToString();
		//Debug.Log(GameManager.instance.survivorCardList.Count);
		survivorsAliveText.text = "Survivors: " + (GameManager.instance.activeSurvivorCardList.Count);
        

		//inventory panel text updates
//		shivCountText.text = GameManager.instance.shivCount.ToString();
//		clubCountText.text = GameManager.instance.clubCount.ToString();
//		gunCountText.text = GameManager.instance.gunCount.ToString();

		//homebase panel updates
		homebaseLatText.text = "Homebase Latitude: "+GameManager.instance.homebaseLat.ToString();
		homebaseLonText.text = "Homebase Longitude: "+GameManager.instance.homebaseLong.ToString();

		//duplicate health slider updates
		playerHealthSlider.value = (CalculatePlayerHealthSliderValue());
		playerHealthSliderDuplicate.value = (CalculateSurvivorStamina());

		bldgSpawner.PlaceHomebaseGraphic();
        StartCoroutine(SetCurrentLocationText());
	}

	private float distToActiveBldg = 0.0f;
	public void ActivateBuildingInspector(int zombiesInBldg, string buildingName, string bldg_id, float buildingLat, float buildingLng) {

		//this sets text, stores the int/string, and activates the panel.
		bldgName = buildingName;
		bldgNameText.text = buildingName;
		active_bldg_id = bldg_id;
		zombieCount = zombiesInBldg;
		float distToBldg = (int)CalculateDistanceToTarget(buildingLat, buildingLng);
		distToActiveBldg = distToBldg;
		int rand = UnityEngine.Random.Range(1,3);
		zombieCountText.text = (zombiesInBldg-rand)+"-"+(zombiesInBldg+rand);
		bldgDistText.text = distToBldg.ToString();

		//set the color of the text based on the zombie count, and the distance calculated.
		if(distToBldg < 100f) {
			bldgDistText.color = Color.green;
		} else if (distToBldg < 150f) {
			bldgDistText.color = Color.yellow;
		} else {
			bldgDistText.color = Color.red;
		}
		if (zombieCount < 6) {
			zombieCountText.color = Color.green;
		} else if (zombieCount < 13) {
			zombieCountText.color = Color.yellow;
		} else {
			zombieCountText.color = Color.red;
		}


		//if location services are on
		if (Input.location.status == LocationServiceStatus.Running){
			//if player is too far from the building disable the enter option.

			if (distToBldg > 150.0f) {
				//out of range, disable button
				enterBldgButton.GetComponent<Button>().interactable = false;
			} else {
				//in range, enable button
				enterBldgButton.GetComponent<Button>().interactable = true;
			}
		} else {
			//just leave the button on if location services arent running... later this will need to be removed.
			enterBldgButton.GetComponent<Button>().interactable = true;
		}

		//if there are less than 10 survivors active, you may not start a mission.
		if (GameManager.instance.activeSurvivorCardList.Count < 10) {
			sendATeamButton.interactable = false;
		} else {
			sendATeamButton.interactable = true;
		}

		buildingPanel.SetActive(true);

	}

	//opens and closes the mission confirmation panel.
	public void ConfirmMissionStart () {
		if (missionStartConfirmationPanel.activeInHierarchy == false) {
			//construct panel text
			string myConfirmationPanelString = "";
			myConfirmationPanelString += "you pull 5 members of your team aside, and show them\n";
			myConfirmationPanelString += bldgName+"\n\n";
			myConfirmationPanelString += "after discussion, everyone agrees\n";
			float movement_speed = 1.5f; //movement speed in m/s
			float time_in_seconds = distToActiveBldg/movement_speed;
			time_in_seconds = time_in_seconds*2; //doubled for there and back
			time_in_seconds += 120;
			TimeSpan duration = TimeSpan.FromSeconds(time_in_seconds);
			string myClockText = "";
			if (duration.Hours > 0) {
				myClockText += duration.Hours.ToString().PadLeft(2, '0')+"h:";
				duration = duration - TimeSpan.FromHours(duration.Hours);
			}
			if(duration.Minutes > 0){
				myClockText += duration.Minutes.ToString().PadLeft(2, '0')+"m:";
				duration = duration - TimeSpan.FromMinutes(duration.Minutes);
			}
			if(duration.Seconds > 0) {
				myClockText += duration.Seconds.ToString().PadLeft(2, '0')+"s";
			}
			myConfirmationPanelString += "it will take at least "+myClockText+"\n";
			if (zombieCount <= 5) {
				myConfirmationPanelString += "we don't expect much action";
			} else if (zombieCount <= 12) {
				myConfirmationPanelString += "we're sure to run into something";
			} else if (zombieCount > 12) {
				myConfirmationPanelString += "we're expecting a decent fight";
			}

			//set text and activate panel
			missionConfirmationText.text = myConfirmationPanelString;
			missionStartConfirmationPanel.SetActive(true);

		} else {
			missionStartConfirmationPanel.SetActive(false);
		}
	}

	//Send the relevant data into the MissionCombatSimulator, then send the result to the server.
	public void StartMission () {
		//load combat data into simulator
		myMissionCombatSimulator.missionPlayCardList.Add(GameManager.instance.activeSurvivorCardList[5].GetComponent<SurvivorPlayCard>());
		myMissionCombatSimulator.missionPlayCardList.Add(GameManager.instance.activeSurvivorCardList[6].GetComponent<SurvivorPlayCard>());
		myMissionCombatSimulator.missionPlayCardList.Add(GameManager.instance.activeSurvivorCardList[7].GetComponent<SurvivorPlayCard>());
		myMissionCombatSimulator.missionPlayCardList.Add(GameManager.instance.activeSurvivorCardList[8].GetComponent<SurvivorPlayCard>());
		myMissionCombatSimulator.missionPlayCardList.Add(GameManager.instance.activeSurvivorCardList[9].GetComponent<SurvivorPlayCard>());

		//call the funtion to generate combat results
		myMissionCombatSimulator.SimulateCombat(zombieCount, distToActiveBldg, active_bldg_id, bldgName);
	}

	public void DeactivateBuildingInspector () {
		buildingPanel.SetActive(false);
	}

	public void LoadIntoCombatFromBldgInspector () {
		GameManager.instance.LoadIntoCombat(zombieCount, bldgName);
	}
    
    void DisplayUsername (IResult result) {
        
        if (result.Error == null) {
            playerNameText.text = result.ResultDictionary["name"].ToString();
        } else {
            Debug.Log (result.Error);
        }
        
    }

    public void NewMissionComplete (int mission_id) {
    	//declare variables 
    	string bldg_name = "";
    	int dead = 0;
    	int supply = 0;
    	int water = 0;
    	int food = 0;

    	JsonData missionJson = JsonMapper.ToObject(GameManager.instance.missionJsonText);
    	//find the mission in the jsondata object on GameManager
    	for(int i=0; i < missionJson[1].Count; i++) {
    		if (missionJson[1][i]["mission_id"].ToString() == mission_id.ToString()) {
    			//load the matching data into variables with the correct scope
    			bldg_name = missionJson[1][i]["building_name"].ToString();
    			supply = (int)missionJson[1][i]["supply_found"];
    			water = (int)missionJson[1][i]["water_found"];
    			food = (int)missionJson[1][i]["food_found"];

    			if (missionJson[1][i]["survivor1_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[1][i]["survivor2_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[1][i]["survivor3_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[1][i]["survivor4_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[1][i]["survivor5_dead"].ToString() == "1") {
    				dead++;
    			}
    		}
    	}

    	//construct the text for the panel
    	string mission_results_text = "";
    	mission_results_text = "the team has returned from investigating "+bldg_name+"\n";
    	mission_results_text += "they found "+supply+" supply\n";
    	mission_results_text += water+" water\n";
    	mission_results_text += food+" food\n";
    	if (dead > 0) {
    		mission_results_text += "at the cost of "+dead+" survivors lives";
    	} else {
    		mission_results_text += "and everyone returned safely";
    	}

    	//instantiate the panel, set it's ID and text
    	GameObject instance = Instantiate(missionCompletePrefab);
    	GameObject canvas = GameObject.Find("Canvas");
    	instance.gameObject.transform.SetParent(canvas.transform);
    	MissionCompletePanelManager missCompPanelMgr = instance.GetComponent<MissionCompletePanelManager>();
    	missCompPanelMgr.missionInfoText.text = mission_results_text;
    	missCompPanelMgr.mission_id = mission_id;
    }
    
    //this is to get the last location data coroutine, it's part of updating the UI.
    IEnumerator SetCurrentLocationText () {
        if (Input.location.status == LocationServiceStatus.Running) {
            yield return Input.location.lastData;
            currentLatText.text = "Current Latitude: " + Input.location.lastData.latitude.ToString();
            currentLonText.text = "Current Longitude: " + Input.location.lastData.longitude.ToString();
        } else {
            Debug.Log ("Location services not running- can't finish UI update");
        }
    }

	private float CalculatePlayerHealthSliderValue (){
		int currStam =0;
		int baseStam =0;
		foreach (GameObject survivorCard in GameManager.instance.activeSurvivorCardList) {
			SurvivorPlayCard SPC = survivorCard.GetComponent<SurvivorPlayCard>();
			if (SPC.team_pos == 5) {
				baseStam = SPC.survivor.baseStamina;
				currStam = SPC.survivor.curStamina;
				break;
			}
		}
		//Debug.Log(baseStam.ToString()+" "+currStam.ToString());
		float value = (float)currStam/(float)baseStam;
		return value;//the number 100 is a plceholder for total health possible.
	}

	private float CalculateSurvivorStamina () {
		int totalMaxStam = 0;
		int totalCurrStam = 0;
		foreach (GameObject survivor in GameManager.instance.activeSurvivorCardList) {
			SurvivorPlayCard SurvPlayCard = survivor.GetComponent<SurvivorPlayCard>();
			totalMaxStam += SurvPlayCard.survivor.baseStamina;
			totalCurrStam += SurvPlayCard.survivor.curStamina;
			//Debug.Log ("adding "+SurvPlayCard.survivor.baseStamina+" to base of stam counter, and "+SurvPlayCard.survivor.curStamina+" to the current stamina");
		}
		float value = (float)totalCurrStam/(float)totalMaxStam;
		//Debug.Log ("The value of the slider has been calculated to be: "+value);
		return value;
	}

	//This is to attach directly to the drop/resupply button, and verify user is within proximity of their homebase.
	//later it should return a boolean, and if true, start the coroutine to send the transaction signal to the server, and get updated results.
	//for now we're just going to change the button color to confirm it works with current GPS coordinates
	public void AmIHomeTest () {
		if (Input.location.status == LocationServiceStatus.Running) {
			//calculate the distance from current location to stored home location.
			float myLat = Input.location.lastData.latitude;
			float myLon = Input.location.lastData.longitude;
			float homeLat = GameManager.instance.homebaseLat;
			float homeLon = GameManager.instance.homebaseLong;

			float latMid = (myLat + homeLat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			double deltaLatitude = (myLat - homeLat);
			double deltaLongitude = (myLon - homeLon);
			double latDistMeters = deltaLatitude * m_per_deg_lat;
			double lonDistMeters = deltaLongitude * m_per_deg_lon;
			double directDistMeters = Mathf.Sqrt(Mathf.Pow((float)latDistMeters, 2f)+Mathf.Pow((float)lonDistMeters, 2f));

			if (directDistMeters <= 100.0f) {
				//player is within range of home- attempt to drop all supply to homebase client.
				StartCoroutine(DropoffAndResupply());

			} else {
				StartCoroutine(PostTempLocationText("You are more than 100m from home"));
			}

		} else {
			Debug.Log ("location services not running");
			StartCoroutine(PostTempLocationText("location services not running"));

			//remove this for live version- this is for testing only.
			StartCoroutine(DropoffAndResupply());
		}

	}

	public void AttemptToDropAndResupply () {
		if (AmIInRangeOfValidOutpost()) {
			StartCoroutine(DropoffAndResupply());
		} else {
			Debug.Log("Player not in range of homebase or active outpost");
			StartCoroutine(PostTempLocationText("You are not in range of home or outpost"));
		}
	}

	private bool AmIInRangeOfValidOutpost () {
		//ensure location services are running
		if (Input.location.status == LocationServiceStatus.Running) {
			float myLat = Input.location.lastData.latitude;
			float myLng = Input.location.lastData.longitude;

			//first check the homebase
			float dist_to_home = CalculateDistanceToTarget(GameManager.instance.homebaseLat, GameManager.instance.homebaseLong);
			if(dist_to_home <= 100.0f) {
				Debug.Log("player is within 100m of home, returning true to send ressupply");
				return true;
			}

			//check all of the outposts.
			GameObject[] outpostArray = GameObject.FindGameObjectsWithTag("outpost");
			foreach (GameObject outpost in outpostArray) {
				Outpost myOutpost = outpost.GetComponent<Outpost>();
				float myRangeToOutpost = CalculateDistanceToTarget(myOutpost.outpost_lat, myOutpost.outpost_lng);
				if (myRangeToOutpost <= 100.0f) {
					return true;
				} 
			}
			return false;
		} else {
			//I'm assuming this is only false inside the unity client, so i'm returning true for now, otherwise return false.
			Debug.Log("Location services are off- GIVING APPROVAL FOR RANGE DESPITE THIS-- REMOVE BEFORE PUBLISHING!!");
			return true;
		}
	}

	public float CalculateDistanceToTarget (float lat, float lng) {
		if (Input.location.status == LocationServiceStatus.Running) {
			float myLat = Input.location.lastData.latitude;
			float myLon = Input.location.lastData.longitude;
			float targetLat = lat;
			float targetLng = lng;

			float latMid = (myLat + targetLat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			double deltaLatitude = (myLat - targetLat);
			double deltaLongitude = (myLon - targetLng);
			double latDistMeters = deltaLatitude * m_per_deg_lat;
			double lonDistMeters = deltaLongitude * m_per_deg_lon;
			double directDistMeters = Mathf.Sqrt(Mathf.Pow((float)latDistMeters, 2f)+Mathf.Pow((float)lonDistMeters, 2f));

			string myText = "You are "+(float)directDistMeters+" meters from your target";
			Debug.Log (myText);

			return (float)directDistMeters;

		} else {
			Debug.Log ("Location services not running");
			StartCoroutine(PostTempLocationText("Location Services Not Running"));
			return 1000.0f;
		}
	}

	public void CalculateAndPostDistanceToHome () {
		if (Input.location.status == LocationServiceStatus.Running) {
			float myLat = Input.location.lastData.latitude;
			float myLon = Input.location.lastData.longitude;
			float homeLat = GameManager.instance.homebaseLat;
			float homeLon = GameManager.instance.homebaseLong;

			float latMid = (myLat + homeLat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			double deltaLatitude = (myLat - homeLat);
			double deltaLongitude = (myLon - homeLon);
			double latDistMeters = deltaLatitude * m_per_deg_lat;
			double lonDistMeters = deltaLongitude * m_per_deg_lon;
			double directDistMeters = Mathf.Sqrt(Mathf.Pow((float)latDistMeters, 2f)+Mathf.Pow((float)lonDistMeters, 2f));

			string myText = "You are "+(float)directDistMeters+" meters from your homebase";
			Debug.Log (myText);
			StartCoroutine(PostTempLocationText(myText));

		} else {
			Debug.Log ("Location services not running");
			StartCoroutine(PostTempLocationText("Location Services Not Running"));
		}
	}

	public void ButtonPress_SetNewHomebase () {
		//if a homebase has not been set.
		if (GameManager.instance.homebaseLat != 0.0f && GameManager.instance.homebaseLong != 0.0f) {
			//check that it has been more than ## of days since player last set their homebase
			int days = 30;
			System.TimeSpan moveHomebaseAllowedTimer = System.TimeSpan.FromDays(days);
			if (GameManager.instance.lastHomebaseSetTime+moveHomebaseAllowedTimer <= System.DateTime.Now) {
				//open the panel with the ability to send the data.
				homebaseConfirmationText.text = "it has been more than "+days.ToString()+" days since you have set your homebase\n would you like to make your present location home?\n\n 2nd warning: you can only do this 1 time every "+days.ToString()+" days!!";
				confirmHomebaseButton.interactable = true;
				homebaseConfirmationPanel.SetActive(true);
			} else {
				//open the panel without the ability to send the homebase data.
				homebaseConfirmationText.text = "it has not been "+days.ToString()+" days since you have set your homebase\n you must wait, or set up an outpost.";
				confirmHomebaseButton.interactable = false;
				homebaseConfirmationPanel.SetActive(true);
			}
		} else {
			//open the warning panel notifying the player that this is their first time.
			homebaseConfirmationText.text = "You have not yet set a homebase\n \nYour homebase in ARG Zombies is the location you return to in order to drop off supplies, and pick up gear";
			confirmHomebaseButton.interactable = true;
			homebaseConfirmationPanel.SetActive(true);
		}
	}
	private bool sendingNewHomebase = false;
	public void SetNewHomebaseLocation () {
		if (sendingNewHomebase == false) {
			sendingNewHomebase = true;
			StartCoroutine(UpdateHomebaseLocation());
		}
	}

	IEnumerator UpdateHomebaseLocation () {
		float newLat = 0f;
		float newLon = 0f;
		GameManager.instance.lastHomebaseSetTime = System.DateTime.Now;

		//this allows the unity player to run without error or location services running
		if (Input.location.status == LocationServiceStatus.Running) {
			newLat = Input.location.lastData.latitude;
			newLon = Input.location.lastData.longitude;

		} else {
			Debug.Log("Location services not running. Attempting to update the server with Dummy data");
			newLat = 37.80897f;
			newLon = -122.4292f;
		}

		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("lat", newLat.ToString());
		form.AddField("lon", newLon.ToString());
		form.AddField("homebase_set_time", GameManager.instance.lastHomebaseSetTime.ToString());

		WWW www = new WWW(updateHomebaseURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			string jsonString = www.text;
			JsonData replyJson = JsonMapper.ToObject(jsonString);

			if (replyJson[0].ToString() == "Success") {
				Debug.Log (replyJson[1].ToString());
				GameManager.instance.homebaseLat = newLat;
				GameManager.instance.homebaseLong = newLon;
			} else {
				//this will handle any error responses from the server when attempting to set new homebase
				//I expect there will be a time delay, or cost associated with multiple changes, so this  is
				//where I expect the exctpetions to go.
			}
		} else {
			Debug.Log(www.error);
		}
		homebaseConfirmationPanel.SetActive(false);
		sendingNewHomebase = false;
		UpdateTheUI();
	}

	public bool sendingOutpost = false;
	public void CreateOutpost () {
		if (sendingOutpost == false) {
			sendingOutpost = true;
			StartCoroutine(SendNewOutpost());
		}
	}

	IEnumerator SendNewOutpost() {
		float newLat = 0f;
		float newLon = 0f;

		//this allows the unity player to run without error or location services running
		if (Input.location.status == LocationServiceStatus.Running) {
			newLat = Input.location.lastData.latitude;
			newLon = Input.location.lastData.longitude;

		} else {
			Debug.Log("Location services not running. Attempting to update the server with Dummy data");
			newLat = 37.80897f;
			newLon = -122.4292f;
		}

		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("outpost_lat", newLat.ToString());
		form.AddField("outpost_lng", newLon.ToString());
		//duration and capacity are set on server side.

		WWW www = new WWW(SendNewOutpostURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			if (www.text != "") {
				JsonData outpostResult = JsonMapper.ToObject(www.text);

				if(outpostResult[0].ToString() == "Success" ) {
					Debug.Log(outpostResult[1].ToString());
					homebaseConfirmationPanel.SetActive(false);
					homebasePanel.SetActive(false);
					StartCoroutine(GameManager.instance.FetchOutpostData());
				} else if (outpostResult[0].ToString() == "Failed") {
					Debug.Log(outpostResult[1].ToString());
				}
			} else {
				Debug.Log("outpost creation returning blank");
			}
		} else {
			Debug.Log(www.error);
		}
		sendingOutpost = false;
	}

	// this coroutine currently only handles dropoff, but is planned to handle pickup as well in the future.
	IEnumerator DropoffAndResupply () {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);

		WWW www = new WWW(dropoffAndResupplyURL ,form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			string dropoffReturnString = www.text;
			JsonData dropoffJson = JsonMapper.ToObject(dropoffReturnString);

			if (dropoffJson[0].ToString() == "Success") {
				GameManager.instance.supply = 0;

//				if (dropoffJson[2].ToString() != "none") {
//						GameManager.instance.shivCount = GameManager.instance.shivCount + (int)dropoffJson[2]["knife_for_pickup"];
//						GameManager.instance.clubCount = GameManager.instance.clubCount + (int)dropoffJson[2]["club_for_pickup"];
//						GameManager.instance.gunCount = GameManager.instance.gunCount + (int)dropoffJson[2]["ammo_for_pickup"];
//						GameManager.instance.survivorsActive = GameManager.instance.survivorsActive + (int)dropoffJson[2]["active_survivor_for_pickup"];
//				}

				GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
				GameManager.instance.ResumeCharacter();

				StartCoroutine(PostTempLocationText("Dropoff Success!"));
			} else if (dropoffJson[0].ToString() == "Failed") {
				StartCoroutine(PostTempLocationText(dropoffJson[1].ToString()));
				Debug.Log(dropoffJson[1].ToString());
			}

		} else {
			Debug.Log("www error: "+ www.error);
		}
	}

	public void SelectWeaponToEquip (int survivor_id) {
		active_gearing_survivor_id = survivor_id;

		//my calling this we should update the list each time the weapon equip button is pressed.
		theWeaponListPopulator.PopulateWeaponsFromGameManager();

		unequippedWeaponsPanel.SetActive(true);
	}

	public void EquipThisWeapon(int weapon_id) {
		to_equip_weapon_id = weapon_id;

		Debug.Log("Sending survivor id "+ active_gearing_survivor_id.ToString()+" and weapon_id: "+to_equip_weapon_id.ToString());
		StartCoroutine(EquipWeaponToSurvivor(active_gearing_survivor_id, to_equip_weapon_id));
	}

	public void UneqipThisWeapon(int weapon_id) {
		to_unequip_weapon_id = weapon_id;

		StartCoroutine(UnequipThisWeapon(active_gearing_survivor_id, to_unequip_weapon_id));
	}

	public void CloseWeaponSelectPanel () {
		active_gearing_survivor_id = 0;
		to_equip_weapon_id = 0;
		unequippedWeaponsPanel.SetActive(false);
	}

	IEnumerator UnequipThisWeapon (int survivor_equipped, int weapon_id) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("survivor_id", survivor_equipped);
		form.AddField("weapon_id", weapon_id);

		WWW www = new WWW(unequipWeaponURL, form);
		yield return www;
		Debug.Log(www.text);

		if(www.error == null) {
			JsonData unequipJson = JsonMapper.ToObject(www.text);
			if (unequipJson[0].ToString() == "Success") {
				Debug.Log(unequipJson[1].ToString());

				GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
				GameManager.instance.RenewWeaponAndEquippedData();
			} else if (unequipJson[0].ToString() == "Failed") {
				Debug.Log(unequipJson[1].ToString());
				GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
				GameManager.instance.RenewWeaponAndEquippedData();
			}
		} else {
			Debug.Log(www.error);
		}
	}

	IEnumerator EquipWeaponToSurvivor (int survivor_id, int weapon_id) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("survivor_id", survivor_id);
		form.AddField("weapon_id", weapon_id);

		WWW www = new WWW(equipWeaponURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			string serverString = www.text.ToString();
			JsonData weaponEquipJson = JsonMapper.ToObject(serverString);

			if(weaponEquipJson[0].ToString() == "Success") {
				Debug.Log(weaponEquipJson[1].ToString());

				//refresh player data from server, and repopulate it to all the game elements/objects
				GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true; //this boolean activates the map level update from gamemanager, as part of the update loop.
				GameManager.instance.RenewWeaponAndEquippedData();

			} else if (weaponEquipJson[0].ToString() == "Failed") {
				Debug.Log(weaponEquipJson[1].ToString());
			}


		}else {
			Debug.Log(www.error);
		}
	}
	public bool promotionInProgress=false;
	public void PromoteThisSurvivor (int surv_id) {
		if (promotionInProgress==false) {
			promotionInProgress=true;
			StartCoroutine(PromoteSurvivorToTeam (surv_id));
		}
	}

	IEnumerator PromoteSurvivorToTeam (int survivor_id) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("survivor_id", survivor_id);

		WWW www = new WWW(promoteSurvivorURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData survivorPromotionJson = JsonMapper.ToObject(www.text);

			if (survivorPromotionJson[0].ToString() == "Success") {
				Debug.Log(survivorPromotionJson[1].ToString());

				GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
				GameManager.instance.RenewWeaponAndEquippedData();
			}
		} else {
			Debug.Log(www.error);
		}
		promotionInProgress=false;
	}


	private bool textPosted = false;
	public IEnumerator PostTempLocationText (string text) {
		if (textPosted == false) {
			textPosted = true;
			locationReportText.gameObject.SetActive(true);
			locationReportText.text = text;
			yield return new WaitForSeconds (5);
			locationReportText.text = "";
			locationReportText.gameObject.SetActive(false);
			textPosted = false;
		} else {
			yield break;
		}
	}
}
