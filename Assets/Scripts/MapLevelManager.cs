using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using LitJson;
using System;
using UnityEngine.SceneManagement;

public class MapLevelManager : MonoBehaviour {

	public GameObject exitGamePanel, missionCompletePanel, inventoryPanel, buildingPanel, clearedBuildingPanel, qrPanel, personelPanel, gearPanel, homebasePanel, homebaseConfirmationPanel, outpostSelectionPanel, outpostConfirmationPanel, missionStartConfirmationPanel, OutpostQRPanel, enterBldgButton, unequippedWeaponsPanel, mapLevelCanvas, hungerThirstWarningPanel, endGamePanel, endGameButton, bogConfirmationPanel;

	[SerializeField]
	public Text supplyText, daysAliveText, survivorsAliveText, currentLatText, currentLonText, locationReportText, zombieKillText, foodText, waterText, gearText, expirationText, playerNameText, clearedBuildingNameText, bldgNameText, bldgSupplyText, bldgFoodText, bldgWaterText, clearedBldgSupplyText, clearedBldgFoodText, clearedBldgWaterText, zombieCountText, bldgDistText, homebaseLatText, homebaseLonText, missionConfirmationText;

	[SerializeField]
	private Slider playerHealthSlider, playerHealthSliderDuplicate;

	[SerializeField]
	private BuildingSpawner bldgSpawner;

	public Button confirmHomebaseButton, sendATeamButton;
	public Text homebaseConfirmationText, starvationWarningText;

	public WeaponListPopulator theWeaponListPopulator;
	public SurvivorListPopulator theSurvivorListPopulator;
	public MissionListPopulator theMissionListPopulator;
	public MissionCombatSimulator myMissionCombatSimulator;

	private float lastUpdateLat = 0.0f, lastUpdateLng = 0.0f;
	private float lastStamUpdateLat = 0.0f, lastStamUpdateLng = 0.0f;
    private bool runGameClock = false;

	private int zombieCount, outpost_index;
	public string bldgName, active_bldg_id;
	public PopulatedBuilding activeBuilding;

	public int active_gearing_survivor_id, to_equip_weapon_id, to_unequip_weapon_id;

	private static GameObject missionCompletePrefab;

	private string updateHomebaseURL = GameManager.serverURL+"/UpdateHomebaseLocation.php";
	private string dropoffAndResupplyURL = GameManager.serverURL+"/DropoffAndResupply.php";
	private string equipWeaponURL = GameManager.serverURL+"/EquipWeapon.php";
	private string unequipWeaponURL = GameManager.serverURL+"/UnequipWeapon.php";
	private string promoteSurvivorURL = GameManager.serverURL+"/PromoteSurvivor.php";
	private string staminaUpdateURL = GameManager.serverURL+"/StaminaRegen.php";
	private string SendNewOutpostURL = GameManager.serverURL+"/CreateOutpost.php";
	private string clearDeadSurvivorsURL = GameManager.serverURL+"/ClearDead.php";
	private string reRollZombieURL = GameManager.serverURL+"/ReRollZombieCount.php";
    private string transferLootURL = GameManager.serverURL + "/TransferLootFromBuilding.php";
    //public string placeGearURL = GameManager.serverURL + "/PlaceGear.php";//relocated to cleared bldg panel mgr

	public void InventoryButtonPressed () {
		if (inventoryPanel.activeInHierarchy == false ) {
			inventoryPanel.SetActive(true);
            personelPanel.gameObject.SetActive(true);
            gearPanel.gameObject.SetActive(false);
		} else if (inventoryPanel.activeInHierarchy == true) {
			inventoryPanel.SetActive(false);
		}
	}

    public void PersonelButtonPressed()
    {
        personelPanel.gameObject.SetActive(true);
        gearPanel.gameObject.SetActive(false);
    }

    public void GearButtonPressed()
    {
        personelPanel.gameObject.SetActive(false);
        gearPanel.gameObject.SetActive(true);
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
		InvokeRepeating("CheckAndUpdateMap", 10.0f, 10.0f);
		//opening the panel uses current PopulatedBuilding gameobject, if this repeating remains on, it will delete and replace that gameobject every 10s
		//for this reason, we turn it off when the panel opens, and back on when the panel closes.
	}

	public void OutpostButtonPressed () {
		if (outpostSelectionPanel.activeInHierarchy == true) {
			outpostSelectionPanel.SetActive(false);
		} else {
			outpostSelectionPanel.SetActive(true);
		}
	}

	public void EndGameButtonPressed () {
		if (endGamePanel.activeInHierarchy == true) {
			endGamePanel.SetActive(false);
		} else {
			endGamePanel.SetActive(true);
		}
	}

	public void BlazeOfGloryPressed () {
		if (bogConfirmationPanel.activeInHierarchy == true) {
			bogConfirmationPanel.SetActive(false);
		} else {
			bogConfirmationPanel.SetActive(true);
		}
	}

	public void ConfirmBLAZEOFGLORYgo () {
		Debug.Log("activating blaze of glory! FOR VALHALA!!! WHAT A SHINY DAY!!!");
		GameManager.instance.BlazeOfGloryActivate();
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

    public void ToggleExitPanel ()
    {
        if (exitGamePanel.activeInHierarchy)
        {
            exitGamePanel.SetActive(false);
        }else
        {
            exitGamePanel.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Debug.Log("EXIT GAME CALLED!!");
        SceneManager.LoadScene("01a Login");
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

        SetEndGameButtonText(); //this is going to be handled 1x on map level load for dynamic changes over time.
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void Update ()
    {
        if (runGameClock == true)
        {
            UpdateTimeAliveClock();
            //UpdateButtonSizes();
        }
    }

    /*
    public int pub_x, pub_y;

    void UpdateButtonSizes()
    {
        RectTransform rt = endGameButton.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(pub_x, pub_y);
        }
        else
        {
            Debug.Log("unable to find endgame RT");
        }   
    }
    */

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
		//UpdateTheUI();
		InvokeRepeating("CheckAndUpdateMap", 10.0f, 10.0f);
		InvokeRepeating("RegenerateStamina", 30.0f, 60.0f);

		CheckForStarvationDehydration();
		UpdateTheUI();

        
		/* 
		//if the player is the last one left alive, activate the gameover button.
		JsonData survivorJson = JsonMapper.ToObject(GameManager.instance.survivorJsonText);
		if (survivorJson.Count > 1) {
			endGameButton.SetActive(false);
		}else{
			Debug.Log("Turning end game panel button on");
			endGameButton.SetActive(true);
		}
		*/
	}

    void SetEndGameButtonText ()
    {
        Text button_text = endGameButton.GetComponentInChildren<Text>();
        //Debug.Log(button_text.text);

        if (button_text != null)
        {
            TimeSpan time_alive = (DateTime.Now - GameManager.instance.timeCharacterStarted);
            int days_alive = Mathf.FloorToInt((float)time_alive.TotalDays);

            if (days_alive < 3)
            {
                RectTransform button_rt = endGameButton.GetComponent<RectTransform>();
                button_rt.sizeDelta = new Vector2(100, 62);
                //suicide isn't that tempting yet
                button_text.text = "die intentionally";

            }else if (days_alive < 8)
            {
                RectTransform button_rt = endGameButton.GetComponent<RectTransform>();
                button_rt.sizeDelta = new Vector2(120, 77);
                button_text.text = "stop living";
            }
            else if (days_alive < 13)
            {
                RectTransform button_rt = endGameButton.GetComponent<RectTransform>();
                button_rt.sizeDelta = new Vector2(143, 86);
                button_text.text = "take the easy way out";
            }
            else if (days_alive < 21)
            {
                RectTransform button_rt = endGameButton.GetComponent<RectTransform>();
                button_rt.sizeDelta = new Vector2(162, 95);
                button_text.text = "give up on life";
            }
            else if (days_alive < 34)
            {
                RectTransform button_rt = endGameButton.GetComponent<RectTransform>();
                button_rt.sizeDelta = new Vector2(185, 110);
                button_text.text = "end the pain";
            }
            else
            {
                RectTransform button_rt = endGameButton.GetComponent<RectTransform>();
                button_rt.sizeDelta = new Vector2(200, 120);
                //suicide is very tempting now
                button_text.text = "sweet release";
            }
            //Debug.Log(days_alive.ToString() + ": days alive calculated by the suicide button");

        }else
        {
            Debug.Log("Suicide button text object not found");
        }
    }

	void CheckForStarvationDehydration () {
		//initialize data needed for funciton
		JsonData eatDrinkJson = JsonMapper.ToObject(GameManager.instance.starvationHungerJsonText);
		String starvationWarning = "";
		bool openPanel =false;

		//construct the text string, starting with smallest to largest threat- only displaying last warning found
		if (GameManager.instance.foodCount <= 0 || GameManager.instance.waterCount <= 0) {
			starvationWarning = "Warning! you are out of food and water. Your team will start dying within hours.";
			openPanel = true;
		}

		if (GameManager.instance.starvationHungerJsonText != "") {
			bool ded = false;
			for (int i=0; i<eatDrinkJson.Count; i++) {
				if ((int)eatDrinkJson[i]["team_position"] == 5) {
					//game over condition.
					Debug.Log("GAME OVER CONDITION!! ******PLAYER SUPPOSED TO DIE*******");
					starvationWarning = "You have died of starvation and thirst, you are now a zombie!";
					StartCoroutine(GameManager.instance.PlayerDiedofStarvation());
					ded = true;
					break;
				}

				starvationWarning += eatDrinkJson[i]["name"];
				if (eatDrinkJson.Count > 1) {
					starvationWarning += ",";
					if (i == (eatDrinkJson.Count-2)) {
						starvationWarning += " and ";
					} else {
						starvationWarning += " ";
					}
				} else {
					starvationWarning += " ";
				}

			}
			if (ded == false) starvationWarning += "died of starvation and/or dehydration. You must find food and water soon";
				
		}

		if (openPanel == true) {
			starvationWarningText.text = starvationWarning;
			hungerThirstWarningPanel.SetActive(true);
		}
	}


	void CheckAndUpdateMap () {
		//check if location services are active
		if (Input.location.status == LocationServiceStatus.Running) {
			//if a last location has NOT been logged
			if (lastUpdateLat != 0 && lastUpdateLng != 0) {
				//if you've moved enough, then do the update, otherwise do nothing
				if (CalculateDistanceToTarget(lastUpdateLat, lastUpdateLng)>= 20.0f) {
					
					//if player has traveled 250+meters since google api was last pinged, then change the boolean so that it hits the api for new json data.
					if (CalculateDistanceToTarget(bldgSpawner.lastGoogleLat, bldgSpawner.lastGoogleLng) >= 250.0f) {
						bldgSpawner.googleBldgsNeedUpdate = true;
					}
					//log the last location updated from
					lastUpdateLat = Input.location.lastData.latitude;
					lastUpdateLng = Input.location.lastData.longitude;
				}

                //update the map background
                GoogleMap my_googleMap = FindObjectOfType<GoogleMap>();
                if (my_googleMap != null)
                {
                    my_googleMap.Refresh();
                }
                else
                {
                    Debug.Log("UI updater could not locate Google Map clas to refresh map image");
                }

            } else {
				//store current location as last updated location and do the update
				lastUpdateLat = Input.location.lastData.latitude;
				lastUpdateLng = Input.location.lastData.longitude;
			}
		}
        //update building locations
		bldgSpawner.UpdateBuildings();

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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
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

		theWeaponListPopulator.PopulateWeaponsFromGameManager();
		theSurvivorListPopulator.RefreshFromGameManagerList();
        
		//left UI panel update
        if (GameManager.instance.supply >0)
        {
            supplyText.text = "Supply: " + GameManager.instance.supply.ToString();
        }else
        {
            supplyText.text = "Supply: 0";
        }
		if(GameManager.instance.foodCount > 0)
        {
            foodText.text = "Food: " + GameManager.instance.foodCount.ToString();
        }else
        {
            foodText.text = "Food: 0";
        }
        if (GameManager.instance.waterCount > 0)
        {
            waterText.text = "Water: " + GameManager.instance.waterCount.ToString();
        }
        else
        {
            waterText.text = "Water: 0";
        }

        UpdateInventoryPanelText();

        //this runs the game clock on the map level instead of setting a static float for "time played"
        //daysAliveText.text = ((float)((DateTime.Now-GameManager.instance.timeCharacterStarted).TotalDays)).ToString("F2");
        runGameClock = true;
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
		playerHealthSliderDuplicate.value = (CalculateActiveTeamStamina());

		bldgSpawner.PlaceHomebaseGraphic();
		bldgSpawner.UpdateBuildings();
        StartCoroutine(SetCurrentLocationText());
	}

    void UpdateInventoryPanelText ()
    {
        //set zombie kill text
        zombieKillText.text = "zombies killed: " + GameManager.instance.zombieKill_score.ToString();

        //Construct the string to display equipment status
        string my_gear_string = "";
        my_gear_string += "Ammunition: " + GameManager.instance.ammo.ToString() + "\n";
        if (GameManager.instance.trap > 0)
            my_gear_string += "Traps: " + GameManager.instance.trap.ToString() + "\n";
        if (GameManager.instance.barrel > 0)
            my_gear_string += "Barrels: " + GameManager.instance.barrel.ToString() + "\n";
        if (GameManager.instance.greenhouse > 0)
            my_gear_string += "Greenhouse: " + GameManager.instance.greenhouse.ToString() + "\n";
        my_gear_string += "\n";
        int shiv_count = 0;
        int huntKnife_count = 0;
        int bat_count = 0;
        int hammer_count = 0;
        int twentytwo_count = 0;
        int shotty_count = 0;
        foreach (GameObject weapon in GameManager.instance.weaponCardList)
        {
            BaseWeapon myWep = weapon.GetComponent<BaseWeapon>();
            if (myWep.name == "crude shiv")
                shiv_count++;
            if (myWep.name == "hunting knife")
                huntKnife_count++;
            if (myWep.name == "baseball bat")
                bat_count++;
            if (myWep.name == "sledgehammer")
                hammer_count++;
            if (myWep.name == ".22 revolver")
                twentytwo_count++;
            if (myWep.name == "shotgun")
                shotty_count++;
        }
        if (shiv_count > 0)
            my_gear_string += "Shivs: " + shiv_count.ToString() + "\n";
        if (huntKnife_count > 0)
            my_gear_string += "Hunting Knives: " + huntKnife_count + "\n";
        if (bat_count > 0)
            my_gear_string += "Baseball Bats: " + bat_count + "\n";
        if (hammer_count > 0)
            my_gear_string += "Sledgehammers: " + hammer_count + "\n";
        if (twentytwo_count > 0)
            my_gear_string += ".22 Revolvers: " + twentytwo_count + "\n";
        if (shotty_count > 0)
            my_gear_string += "Shotguns: " + shotty_count + "\n";

        gearText.text = my_gear_string;//located in inventory panel/equipment panel

        //construct the string that calculates starvation and thirst
        string my_rss_expiration_string = "";
        GameObject[] all_survivors = GameObject.FindGameObjectsWithTag("survivorcard");
        int all_surv_count = all_survivors.Length;
        my_rss_expiration_string += all_surv_count.ToString() + " survivors\n";
        int daily_consumption = all_surv_count * 4; //eat&drink 1 unit per 6 hrs
        my_rss_expiration_string += "will consume= " + daily_consumption + " water/day\n";
        my_rss_expiration_string += "and " + daily_consumption + " food/day\n";
        int meals_to_expire_f = Mathf.FloorToInt(GameManager.instance.foodCount / all_surv_count);
        int hrs_food_remain = meals_to_expire_f * 6;
        int days_food_remain = Mathf.FloorToInt(hrs_food_remain / 24);
        my_rss_expiration_string += "Out of FOOD in " + days_food_remain + " days\n";
        int meals_to_expire_w = Mathf.FloorToInt(GameManager.instance.waterCount / all_surv_count);
        int hrs_water_remain = meals_to_expire_w * 6;
        int days_water_remain = Mathf.FloorToInt(hrs_water_remain/24);
        my_rss_expiration_string += "Out of WATER in " + days_water_remain + " days\n";
        if (days_water_remain <= days_food_remain)
        {
            my_rss_expiration_string += "DEAD in " +(days_water_remain+2)+" days";
        }else
        {
            my_rss_expiration_string += "DEAD in " + (days_food_remain + 3) + " days";
        }
        expirationText.text = my_rss_expiration_string;

    }

    private void UpdateTimeAliveClock() {
        TimeSpan time_alive = (DateTime.Now - GameManager.instance.timeCharacterStarted);
        //Debug.Log(time_alive.ToString());
        string my_string = "";

        //days
        if (time_alive > TimeSpan.FromDays(1))
        {
            int total_days = Mathf.FloorToInt((float)time_alive.TotalDays);
            my_string += total_days.ToString().PadLeft(2, '0') + "\n";
            time_alive = time_alive - TimeSpan.FromDays(total_days);
        }
        else
        {
            my_string += "00\n";
        }
        //hours
        if (time_alive > TimeSpan.FromHours(1))
        {
            int tot_hrs = Mathf.FloorToInt((float)time_alive.TotalHours);
            my_string += tot_hrs.ToString().PadLeft(2, '0') + ":";
            time_alive = time_alive - TimeSpan.FromHours((float)tot_hrs);
        }
        else
        {
            my_string += "00:";
        }
        //minutes
        if (time_alive > TimeSpan.FromMinutes(1))
        {
            int tot_min = Mathf.FloorToInt((float)time_alive.TotalMinutes);
            my_string += tot_min.ToString().PadLeft(2, '0') + ":";
            time_alive = time_alive - TimeSpan.FromMinutes((float)tot_min);
        }
        else
        {
            my_string += "00:";
        }
        //seconds
        if (time_alive > TimeSpan.FromSeconds(1))
        {
            int tot_sec = Mathf.FloorToInt((float)time_alive.TotalSeconds);
            my_string += tot_sec.ToString().PadLeft(2, '0');
        }
        else
        {
            my_string += "00";
        }

        daysAliveText.text = my_string;
        //Debug.Log(my_string);
    }

    private float distToActiveBldg = 0.0f;
	public void ActivateBuildingInspector(PopulatedBuilding myBuilding) {
        bool building_clear = false;
		CancelInvoke("CheckAndUpdateMap");

		//load building data into MapLevelManager
		activeBuilding = myBuilding;
		bldgName = myBuilding.buildingName;
		bldgNameText.text = myBuilding.buildingName;
        clearedBuildingNameText.text = myBuilding.buildingName;
		active_bldg_id = myBuilding.buildingID;
		zombieCount = myBuilding.zombiePopulation;
		float distToBldg = (int)CalculateDistanceToTarget(myBuilding.myLat, myBuilding.myLng);
		distToActiveBldg = distToBldg;

        //load building data into GameManager.instance
		GameManager.instance.activeBldg_name = myBuilding.buildingName;
		GameManager.instance.activeBldg_id = myBuilding.buildingID;
		GameManager.instance.activeBldg_supply = myBuilding.supply_inside;
		GameManager.instance.activeBldg_food = myBuilding.food_inside;
		GameManager.instance.activeBldg_water = myBuilding.water_inside;
		GameManager.instance.zombiesToFight = myBuilding.zombiePopulation;
		GameManager.instance.activeBldg_lootcode = myBuilding.loot_code;
		GameManager.instance.activeBldg_lastclear = myBuilding.last_cleared;

        //set the dist & zombie count text pieces
		int rand = UnityEngine.Random.Range(-3,3);
		zombieCountText.text = (zombieCount+rand-2)+"-"+(zombieCount+rand+2);
		bldgDistText.text = distToBldg.ToString();
        bldgSupplyText.text = myBuilding.supply_inside.ToString();
        bldgFoodText.text = myBuilding.food_inside.ToString();
        bldgWaterText.text = myBuilding.water_inside.ToString();

        //check the building state based on timestamp stored on object.
		if (myBuilding.last_cleared == DateTime.Parse("11:59pm 12/31/1999")) {
			//this building has not been visited before
			zombieCountText.text = "??";
			bldgSupplyText.text = "??";
			bldgFoodText.text = "??";
			bldgWaterText.text = "??";

		} else if (myBuilding.last_cleared == DateTime.Parse("12:01am 01/01/2000")) {
			//this building has been entered, but not cleared
			zombieCountText.text = myBuilding.zombiePopulation.ToString();
			bldgSupplyText.text = "??";
			bldgFoodText.text = "??";
			bldgWaterText.text = "??";

		} else {
            //this building has been cleared before
            TimeSpan time_since_cleared = (DateTime.Now - myBuilding.last_cleared);
            if (time_since_cleared > TimeSpan.FromHours(4)) {
				//zombies can appear after 4 hours- roll for zombies
				enterBldgButton.GetComponent<Button>().interactable = true;

                //when a building is cleared it stores zombie pop as -1
				if (myBuilding.zombiePopulation < 0) {

                    //reroll zombie population before updating text
                    float odds = 80f;//start at 80% to find bldg still clear after 4hr
                    int min_zombie = 0;
                    int max_zombie = 5;
                    if (time_since_cleared > TimeSpan.FromHours(6))
                    {
                        odds -= 10;//70%
                        min_zombie += 1;//1
                        max_zombie += 3;//8
                    }
                    if (time_since_cleared > TimeSpan.FromHours(12))
                    {
                        odds -= 10;//60%
                        min_zombie += 2;//3
                        max_zombie += 4;//12
                    }
                    if (time_since_cleared > TimeSpan.FromHours(24))
                    {
                        odds -= 10;//50%
                        min_zombie += 2;//5
                        max_zombie += 6;//18
                    }
                    if (time_since_cleared> TimeSpan.FromHours(48))
                    {
                        odds -= 20;//30%
                        min_zombie += 1;//6
                        max_zombie += 7;//25
                    }
                    int zomb_pop = UnityEngine.Random.Range(min_zombie, max_zombie);
                    

                    float roll = UnityEngine.Random.Range(0.0f,100.0f);
                    if (roll <= odds)
                    {
                        zomb_pop = 0; //there is a chance that no zombies have entered the building.
                    }
                    building_clear = false;//this will load the "not clear" building panel

                    GameManager.instance.zombiesToFight = zomb_pop;
                    zombieCount = zomb_pop;
                    myBuilding.zombiePopulation = zomb_pop;
                    StartCoroutine(ReRollZombiePopulation(zomb_pop)); //set the building to populated on the server, even if it is a 0.
                    zombieCountText.text = "??"; //set up the panel text.
                    Debug.Log("reroll zombie population to: "+zomb_pop.ToString());

                    //^^^^this whole section above allows the player to enter combat with 0 zombies, and roll for a survivor any time 4h after initial clear
                    //the first time they click on it after this time window- it rolls and stores the roll.

                    //this is to set up the un-cleared building text with the correct stats for generated resources
                    ClearedBuildingPanelManager CBPM = clearedBuildingPanel.GetComponent<ClearedBuildingPanelManager>();
                    CBPM.CalculateTrapStatus();
                    CBPM.CalculateBarrelStatus();
                    CBPM.CalculateGreenhouseStatus();
                    bldgSupplyText.text = activeBuilding.supply_inside.ToString();
                    bldgFoodText.text = activeBuilding.food_inside.ToString();
                    bldgWaterText.text = activeBuilding.water_inside.ToString();

				} else {
					//zombie population has already been re-rolled, player has entered, but not cleared.
					zombieCountText.text = "??"; 
                    bldgSupplyText.text = myBuilding.supply_inside.ToString();
                    bldgFoodText.text = myBuilding.food_inside.ToString();
                    bldgWaterText.text = myBuilding.water_inside.ToString();
                    building_clear = false;
				}
			} else {
                //building is clear
                building_clear = true; //this tells which panel to load at the end of this function
                clearedBldgSupplyText.text = myBuilding.supply_inside.ToString();
                clearedBldgFoodText.text = myBuilding.food_inside.ToString();
                clearedBldgWaterText.text = myBuilding.water_inside.ToString();
            }
			
		}


		//set the color of the text based on the zombie count, and the distance calculated.
		if(distToBldg < 150f) {
			bldgDistText.color = Color.green;
		} else if (distToBldg < 300f) {
			bldgDistText.color = Color.yellow;
		} else {
			bldgDistText.color = Color.red;
		}
		if (zombieCount < 8) {
			zombieCountText.color = Color.green;
		} else if (zombieCount < 17) {
			zombieCountText.color = Color.yellow;
		} else {
			zombieCountText.color = Color.red;
		}


		//manage which buttons are available
		if (Input.location.status == LocationServiceStatus.Running){
			//if player is too far from the building disable the enter option.

			if (distToBldg > 300.0f) {
				//out of range, disable button
				//enterBldgButton.GetComponent<Button>().interactable = false;
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

        //activate the correct building panel.
        if (building_clear == false)
        {
            buildingPanel.SetActive(true);
        }else
        {
            ClearedBuildingPanelManager CBPM = clearedBuildingPanel.GetComponent<ClearedBuildingPanelManager>();
            CBPM.InitilizeMyText();
            clearedBuildingPanel.SetActive(true);
        }

    }

	IEnumerator ReRollZombiePopulation(int count) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime);
		form.AddField("client", "mob");

		form.AddField("bldg_name", GameManager.instance.activeBldg_name);
		form.AddField("bldg_id", GameManager.instance.activeBldg_id);
		form.AddField("zombie_count", count);

		WWW www = new WWW(reRollZombieURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData reRollReturn = JsonMapper.ToObject(www.text);
			if (reRollReturn[0].ToString() == "Success") {
				Debug.Log(reRollReturn[1].ToString());
			} else {
				Debug.Log(reRollReturn[1].ToString());
			}
		} else {
			Debug.Log(www.error);
		}
	
	}

	public void LoadIntoCombatFromBldgInspector () {
		GameManager.instance.LoadBuildingCombat();
	}

	public void AcknowledgeStarvationWarning () {
		hungerThirstWarningPanel.SetActive(false);
		StartCoroutine(AcknowledgeStarvation());
	}

	IEnumerator AcknowledgeStarvation () {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(clearDeadSurvivorsURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData clearDeadJSON = JsonMapper.ToObject(www.text);
			if (clearDeadJSON[0].ToString() == "Success") {
				Debug.Log(clearDeadJSON[1].ToString());
			}
		} else {
			Debug.Log(www.error);
		}
	}

	//opens and closes the mission confirmation panel.
	public void ConfirmMissionStart () {
		if (missionStartConfirmationPanel.activeInHierarchy == false) {
			if (activeBuilding.last_cleared == DateTime.Parse("11:59pm 12/31/1999")) {
				StartCoroutine(GameManager.instance.RollNewBuildingContents(true));
			}

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
        InvokeRepeating("CheckAndUpdateMap", 10f, 10f);
		buildingPanel.SetActive(false);
        clearedBuildingPanel.SetActive(false);
	}

    public void LootActiveBuilding() {
        if (activeBuilding.supply_inside > 0 || activeBuilding.water_inside > 0 || activeBuilding.food_inside > 0)
        {
            StartCoroutine(TransferLoot());
        }else
        {
            Debug.Log("There is nothing here to loot");
        }
    }

    IEnumerator TransferLoot()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime);
        form.AddField("client", "mob");

        form.AddField("bldg_name", GameManager.instance.activeBldg_name);
        form.AddField("bldg_id", GameManager.instance.activeBldg_id);

        WWW www = new WWW(transferLootURL, form);
        yield return www;

        if (www.error == null)
        {
            JsonData lootTransferJson = JsonMapper.ToObject(www.text);
            if (lootTransferJson[0].ToString() == "Success")
            {
                //manually update the game-state in order to avoid passing and loading JSON
                GameManager.instance.supply += activeBuilding.supply_inside;
                GameManager.instance.foodCount += activeBuilding.food_inside;
                GameManager.instance.waterCount += activeBuilding.water_inside;
                activeBuilding.supply_inside = 0;
                activeBuilding.food_inside = 0;
                activeBuilding.water_inside = 0;
                GameManager.instance.activeBldg_food = 0;
                GameManager.instance.activeBldg_supply = 0;
                GameManager.instance.activeBldg_water = 0;
                clearedBldgFoodText.text = "0";
                clearedBldgSupplyText.text = "0";
                clearedBldgWaterText.text = "0";
                bldgFoodText.text = "0";
                bldgWaterText.text = "0";
                bldgSupplyText.text = "0";
                

                UpdateTheUI();
                Debug.Log(lootTransferJson[1].ToString());
            }else
            {
                Debug.Log(lootTransferJson[1].ToString());
            }
        }else
        {
            Debug.Log(www.error);
        }
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
    	for(int i=0; i < missionJson.Count; i++) {
    		if (missionJson[i]["mission_id"].ToString() == mission_id.ToString()) {
    			//load the matching data into variables with the correct scope
    			bldg_name = missionJson[i]["building_name"].ToString();
    			supply = (int)missionJson[i]["supply_found"];
    			water = (int)missionJson[i]["water_found"];
    			food = (int)missionJson[i]["food_found"];

    			if (missionJson[i]["survivor1_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[i]["survivor2_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[i]["survivor3_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[i]["survivor4_dead"].ToString() == "1") {
    				dead++;
    			}
				if (missionJson[i]["survivor5_dead"].ToString() == "1") {
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
        //GameObject canvas = GameObject.Find("Canvas");
        //GameObject instance = Instantiate(missionCompletePrefab, canvas.transform) as GameObject;


    	MissionCompletePanelManager missCompPanelMgr = missionCompletePanel.GetComponent<MissionCompletePanelManager>();
    	missCompPanelMgr.missionInfoText.text = mission_results_text;
    	missCompPanelMgr.mission_id = mission_id;
        missionCompletePanel.SetActive(true);
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

	private float CalculateActiveTeamStamina () {
		int totalMaxStam = 0;
		int totalCurrStam = 0;
		foreach (GameObject survivor in GameManager.instance.activeSurvivorCardList) {
			SurvivorPlayCard SurvPlayCard = survivor.GetComponent<SurvivorPlayCard>();
			if (SurvPlayCard.team_pos > 0) {
				totalMaxStam += SurvPlayCard.survivor.baseStamina;
				totalCurrStam += SurvPlayCard.survivor.curStamina;
				//Debug.Log ("adding "+SurvPlayCard.survivor.baseStamina+" to base of stam counter, and "+SurvPlayCard.survivor.curStamina+" to the current stamina");
			}
		}
		float value = (float)totalCurrStam/(float)totalMaxStam;
		//Debug.Log ("The value of the slider has been calculated to be: "+value);
		return value;
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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
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

	public void ConfirmOutpostSelection (int index) {
		outpost_index = index;
		if (outpostConfirmationPanel.activeInHierarchy == false) {
			outpostConfirmationPanel.SetActive(true);
		} else {
			outpostConfirmationPanel.SetActive(false);
		}
	}

	public bool sendingOutpost = false;
	public void CreateOutpost () {
		if (sendingOutpost == false) {
			sendingOutpost = true;
			StartCoroutine(SendNewOutpost(outpost_index));
		}
	}

	public IEnumerator SendNewOutpost(int index) {
		float newLat = 0f;
		float newLon = 0f;
		int duration = 0;//in hours
		int capacity = 0;

		if (index == 1) {
			duration = 72;
			capacity = 5;
		} else if (index == 2) {
			duration = (24*14);
			capacity = 12;
		} else if (index == 3) {
			duration = (24*30);
			capacity = 30;
		}

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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("outpost_lat", newLat.ToString());
		form.AddField("outpost_lng", newLon.ToString());
		form.AddField("capacity", capacity.ToString());
		form.AddField("duration", duration.ToString());
		//duration and capacity are set on server side.

		WWW www = new WWW(SendNewOutpostURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			if (www.text != "") {
				JsonData outpostResult = JsonMapper.ToObject(www.text);

				if(outpostResult[0].ToString() == "Success" ) {
					Debug.Log(outpostResult[1].ToString());
					outpostConfirmationPanel.SetActive(false);
					outpostSelectionPanel.SetActive(false);
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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

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
				StartCoroutine(GameManager.instance.LoadAllGameData());

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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
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
