using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using LitJson;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

    public ProfileImageManager profileImageManager;

	public bool gameDataInitialized = false, updateWeaponAndSurvivorMapLevelUI = false, survivorFound = false, playerInTutorial = false, weaponHasBeenSelected = false, playerIsZombie = false, blazeOfGloryActive = false;
	public int daysSurvived, /*supply,*/wood, metal, ammo, trap, barrel, greenhouse, reportedWood, reportedMetal, reportedWater, reportedFood, playerCurrentStamina, playerMaxStamina, foodCount, waterCount, mealCount, distanceCoveredThisSession, zombieKill_HighScore, zombieKill_score;
	public DateTime timeCharacterStarted, lastHomebaseSetTime, gameOverTime, activeBldg_lastclear;
	//public PopulatedBuilding active_building;
	public float homebaseLat, homebaseLong;
	public string foundSurvivorName, lastLoginTime;
	public TimeSpan high_score, my_score, serverTimeOffset;
    public Sprite my_profile_pic;
    public Texture2D profile_image_texture;
	public int foundSurvivorCurStam, foundSurvivorMaxStam, foundSurvivorAttack, foundSurvivorEntryID;
	[SerializeField]
	private GameObject[] weaponOptionsArray;

	public List <GameObject> activeSurvivorCardList = new List<GameObject>();
	public List <GameObject> onMissionSurvivorCardList = new List<GameObject>();
	public List <GameObject> injuredSurvivorCardList = new List<GameObject>();
    public List <GameObject> deadSurvivorCardList = new List<GameObject>();
	public List <GameObject> weaponCardList = new List<GameObject>();
	public GameObject survivorCardHolder;
	public GameObject weaponCardHolder;

	private Scene activeScene;
	//made this public while working on the server "cleared list" data retention. it should go back to private
	public string activeBldg_name, zombie_to_kill_id ="", activeBldg_lootcode, activeBldg_id;
	public int activeBldg_zAcross, activeBldg_wood, activeBldg_metal, activeBldg_food, activeBldg_water, activeBldg_zombies;
	public string myWallsJsonText, locationJsonText, googleBldgJsonTextpg1, googleBldgJsonTextpg2, googleBldgJsonTextpg3, survivorJsonText, weaponJsonText, clearedBldgJsonText, outpostJsonText, missionJsonText, starvationHungerJsonText, injuryJsonText, foundSurvivorJsonText, survivorWallJsonText;
	public JsonData missionData;

	public string userId;
	public string userFirstName;
	public string userLastName;
	public string userName;

	public static string serverURL = "http://game.argzombie.com/ARGZ_DEV_SERVER";
	public static string QR_encryption_key = "12345678901234567890123456789012";

	private string startNewCharURL = serverURL+"/StartNewCharacter.php";
	private string resumeCharacterUrl = serverURL+"/ResumeCharacter.php";
	private string buildingClearedURL = serverURL+"/NewBuildingCleared.php";
	//private string clearedBuildingDataURL = serverURL+"/ClearedBuildingData.php";
	private string fetchSurvivorDataURL = serverURL+"/FetchSurvivorData.php";
	private string fetchWeaponDataURL = serverURL+"/FetchWeaponData.php";
	private string fetchMissionDataURL = serverURL+"/FetchMissionData.php";
	//private string clearSurvivorDataURL = serverURL+"/DeleteMySurvivorData.php";
	private string fetchOutpostDataURL = serverURL+"/FetchOutpostData.php";
	private string allGameDataURL = serverURL+"/FetchAllGameData.php";
	private string newBuildingURL = serverURL+"/NewBuildingEntered.php";
	//private string newHighScoreURL = serverURL+"/NewHighScore.php";
	private string gameOverStarvationURL = serverURL+"/GameOver_Starvation.php";
	private string gameOverSuicideURL = serverURL+"/GameOver_Suicide.php";
	private string gameOverBiteURL = serverURL+"/GameOver_Bite.php";
	private string gameOverBlazeOfGloryURL = serverURL+"/GameOver_BlazeOfGlory.php";
	private string zombieKillURL = serverURL+"/KilledZombie.php";
	public string myProfilePicURL = "";

	private bool eatDrikCounterIsOn;

	private static SurvivorPlayCard survivorPlayCardPrefab;
	private static BaseWeapon baseWeaponPrefab;
	public static int DaysUntilOddsFlat = 40;
	public static float FlatOddsToFind = 5.0f;

	/// <summary>
	/// //////////////////
	/// </summary>

	void Awake () {
		MakeSingleton();
		StartCoroutine (StartLocationServices());

		eatDrikCounterIsOn = false;
		survivorPlayCardPrefab = Resources.Load<SurvivorPlayCard>("Prefabs/SurvivorPlayCard");
		baseWeaponPrefab = Resources.Load<BaseWeapon>("Prefabs/BaseWeaponPrefab");

        profileImageManager = FindObjectOfType<ProfileImageManager>();
		//ResetAllBuildings();
	}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
		//this is a catch all to slave the long term memory to the active GameManager.instance object- each load will update long term memory.


		activeScene = SceneManager.GetActiveScene();
		if (activeScene.name.ToString() == "02a Map Level"){
			//Debug.Log ("Time character started set to: " + timeCharacterStarted);
			SetDaysSurvived();
			
			MapLevelManager mapManager = FindObjectOfType<MapLevelManager>();
			//mapManager.UpdateTheUI();
			mapManager.theMissionListPopulator.LoadMissionsFromGameManager();

            ProfileImageManager my_prof_img_mgr = FindObjectOfType<ProfileImageManager>();
            my_prof_img_mgr.LoadProfilePictures();

		} else if (activeScene.name.ToString() == "01a Login") {

			LoginManager loginMgr = FindObjectOfType<LoginManager>();
			if (FB.IsLoggedIn == true) {
				loginMgr.loggedInPanel.SetActive(true);
			}

		} else if (activeScene.name.ToString() == "01b Start") {
			if (eatDrikCounterIsOn == false) {
				InvokeRepeating ( "CheckEatingAndDrinking", 1.0f, 30.0f);
				eatDrikCounterIsOn = true;
			}
			//in the future this will have to be done on the server, and an update pulled periodically.
		}
	}

	void MakeSingleton() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}



	IEnumerator NewCharacterUpdateServer () {

		
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId );
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		form.AddField("first_name", GameManager.instance.userFirstName);
		form.AddField("last_name", GameManager.instance.userLastName);
		form.AddField("name", GameManager.instance.userName);
		form.AddField("wood", GameManager.instance.wood);
        form.AddField("metal", GameManager.instance.metal);
		form.AddField("food", GameManager.instance.foodCount);
		form.AddField("water", GameManager.instance.waterCount);
		form.AddField("ammo", GameManager.instance.ammo);
		form.AddField("profile_pic_url", GameManager.instance.myProfilePicURL);
		form.AddField("char_created_DateTime", GameManager.instance.timeCharacterStarted.ToString());
		form.AddField("homebase_set_time", "");

		WWW www = new WWW(startNewCharURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			
			Debug.Log ("New character successfully started on the server" + www.text);
            //SceneManager.LoadScene("02a Map Level"); //used to load map level before intro/wep select was added.
            SceneManager.LoadScene("04a Weapon Select"); //added when survivor draft removed from new character process.
            yield break;
		} else {
			Debug.Log("WWW error "+ www.error);
		}
	}

	public IEnumerator LoadAllGameData () {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(allGameDataURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData fullGameData = JsonMapper.ToObject(www.text);

			//******* 0-success 1-player 2-survivor 3-weapon 4-cleared buildings 5-outposts 6-missions 7-eat/drink/starve 8-injuries 9-wall tags ******* THIS IS HOW THE INDEX IS ORGANIZED

			if (fullGameData[0].ToString() =="Success") {

				//***************
				//update the GameManager.instance with all player data
				GameManager.instance.userFirstName = fullGameData[1]["first_name"].ToString() ;
				GameManager.instance.userLastName = fullGameData[1]["last_name"].ToString();
				GameManager.instance.playerCurrentStamina = (int)fullGameData[1]["curr_stamina"];
				GameManager.instance.playerMaxStamina = (int)fullGameData[1]["max_stamina"];
				int wod = Convert.ToInt32(fullGameData[1]["wood"].ToString());
				GameManager.instance.wood = wod;
                int met = Convert.ToInt32(fullGameData[1]["metal"].ToString());
                GameManager.instance.metal = met;
				int wat = Convert.ToInt32(fullGameData[1]["water"].ToString());
				GameManager.instance.waterCount = wat;
				int fud = Convert.ToInt32(fullGameData[1]["food"].ToString());
				GameManager.instance.foodCount = fud;
				GameManager.instance.ammo = (int)fullGameData[1]["ammo"];
                GameManager.instance.trap = (int)fullGameData[1]["trap"];
                GameManager.instance.barrel = (int)fullGameData[1]["barrel"];
                GameManager.instance.greenhouse = (int)fullGameData[1]["greenhouse"];
				float homeLat = (float)Convert.ToDouble(fullGameData[1]["homebase_lat"].ToString());
				GameManager.instance.homebaseLat = homeLat;
				float homeLon = (float)Convert.ToDouble(fullGameData[1]["homebase_lon"].ToString());
				GameManager.instance.homebaseLong = homeLon;
				DateTime oDate = Convert.ToDateTime(fullGameData[1]["char_created_DateTime"].ToString());
				GameManager.instance.timeCharacterStarted = oDate;
				if (fullGameData[1]["homebase_set_time"].ToString() != "") {
					DateTime pDate = Convert.ToDateTime(fullGameData[1]["homebase_set_time"].ToString());
					GameManager.instance.lastHomebaseSetTime = pDate;
				}
				if (fullGameData[1]["game_over_datetime"].ToString() != "") {
					DateTime eDate = Convert.ToDateTime(fullGameData[1]["game_over_datetime"].ToString());
					GameManager.instance.gameOverTime = eDate;
				} 
				//player has not gotten their zombie killed and is attempting to play.
				if (fullGameData[1]["isZombie"].ToString() == "1" || fullGameData[1]["isZombie"].ToString() == "2") {
					if (fullGameData[1]["isZombie"].ToString() == "1") {
						GameManager.instance.playerIsZombie = true;
					} else {
						GameManager.instance.playerIsZombie = false;
					}
                    SceneManager.LoadScene("02b Zombie Mode");
//					SceneManager.LoadScene("03b Game Over");
				}
				GameManager.instance.lastLoginTime = fullGameData[1]["mob_login_ts"].ToString();
				if (fullGameData[1]["high_score"].ToString() != "") {
					TimeSpan hi_score = TimeSpan.Parse(fullGameData[1]["high_score"].ToString());
					GameManager.instance.high_score = hi_score;
				}
				GameManager.instance.zombieKill_score = (int)fullGameData[1]["zombies_killed"];
				GameManager.instance.zombieKill_HighScore = (int)fullGameData[1]["zombies_killed_high_score"];
				//***************
				//load the json text into their respective containers on GameManager.instance



				if(fullGameData[2] != null){
					survivorJsonText = JsonMapper.ToJson(fullGameData[2]);
				} else {
					survivorJsonText = "";
				}
				//Debug.Log(survivorJsonText);

				//Debug.Log(JsonMapper.ToJson(fullGameData[3]));
				if (fullGameData[3] != null) {
					weaponJsonText = JsonMapper.ToJson(fullGameData[3]);
				}else {
					weaponJsonText = "";
				}

				if (fullGameData[4] != null) {
					clearedBldgJsonText = JsonMapper.ToJson(fullGameData[4]);
					//Debug.Log(JsonMapper.ToJson(fullGameData[4]));
				} else {
					clearedBldgJsonText = "";
				}


				if (fullGameData[5] != null) {
					outpostJsonText = JsonMapper.ToJson(fullGameData[5]);
				}else {
					outpostJsonText = "";
				}

				if (fullGameData[6] != null) {
					missionJsonText = JsonMapper.ToJson(fullGameData[6]);
				} else {
					missionJsonText = "";
				}

				if (fullGameData[7] != null) {
					starvationHungerJsonText = JsonMapper.ToJson(fullGameData[7]);
				}else {
					starvationHungerJsonText = "";
				}

				if (fullGameData[8] != null) {
					injuryJsonText = JsonMapper.ToJson(fullGameData[8]);
				}else {
					injuryJsonText = "";
				}

                if (fullGameData[9] != null)
                {
                    myWallsJsonText = JsonMapper.ToJson(fullGameData[9]);
                }else
                {
                    myWallsJsonText = "";
                }

				//***************
				//load the survivor/weapon game objects into the GameManager, and then go to map level
				CreateSurvivorsFromGameManagerJson();
				yield return new WaitForSeconds(0.2f);
				GameManager.instance.gameDataInitialized = true;

				if (playerInTutorial == true && weaponHasBeenSelected == true) {
					string tut = "tutorial";
					LoadAltCombat(1, tut);
				}

				//auto-load map level from login(1) , if on map level stay there, if on victory screen, let the user press the button to load map level.
				if (SceneManager.GetActiveScene().buildIndex == 1 && playerInTutorial == false) {
					SceneManager.LoadScene("02a Map Level");
				} else if (SceneManager.GetActiveScene().name == "02a Map Level") {
					//if we are on the map level, and game data update has been called/completed- update UI and mission list populator.
					MapLevelManager mapManager = FindObjectOfType<MapLevelManager>();
					mapManager.UpdateTheUI();
					mapManager.theMissionListPopulator.LoadMissionsFromGameManager();
				} else if (SceneManager.GetActiveScene().name == "02e Combat-15"/*SceneManager.GetActiveScene().name == "02c Combat-5"*/) {
                    
                    //removed a call to the battlestatemachine- It will set itself up as it loads in.
                    //BattleStateMachine BSM = FindObjectOfType<BattleStateMachine>();
					//BSM.ResetAllTurns();
					//game data is only refreshed from combat when a bit player turns zombie mid-combat. Resetting turns should stop zombies from targeting null gameobjects.
				}
			}	
		} else {
			Debug.Log(www.error);
		}

	}

	//this is only called from the BattleStateMachine in the Combat Scene
	public IEnumerator PlayerBit () {
		activeBldg_name = "bite_case";
		DateTime time_dead = DateTime.Now + TimeSpan.FromMinutes(UnityEngine.Random.Range(5, 25));
		TimeSpan final_score = time_dead - (GameManager.instance.timeCharacterStarted + GameManager.instance.serverTimeOffset);
		my_score=final_score;
		Debug.Log("Player bit, and will turn at "+time_dead.ToString()+" giving a final score of: "+final_score.ToString());

		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime);
		form.AddField("client", "mob");
		form.AddField("game_over_datetime", time_dead.ToString());

		if (final_score > GameManager.instance.high_score) {
			form.AddField("high_score",final_score.ToString());		
		} else {
			form.AddField("high_score", "");
		}

		WWW www= new WWW(gameOverBiteURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			BattleStateMachine myBSM = BattleStateMachine.FindObjectOfType<BattleStateMachine>();
			myBSM.GameOverBiteCallback();
		} else {
			Debug.Log(www.error);
		}
	
	}

	public IEnumerator PlayerDiedofStarvation () {
		//calculate the players score 
		DateTime time_dead = (GameManager.instance.timeCharacterStarted + GameManager.instance.serverTimeOffset) + TimeSpan.FromHours(6 * GameManager.instance.mealCount);
		TimeSpan final_score = time_dead - (GameManager.instance.timeCharacterStarted + GameManager.instance.serverTimeOffset);
		my_score = final_score;
		Debug.Log(final_score.ToString());

		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime);
		form.AddField("client", "mob");
		form.AddField("game_over_datetime", time_dead.ToString());

		if (final_score > GameManager.instance.high_score) {
			form.AddField("high_score",final_score.ToString());		
		} else {
			form.AddField("high_score", "");
		}

		WWW www = new WWW(gameOverStarvationURL, form);
		yield return www;
		Debug.Log(www.text);
        SceneManager.LoadScene("02b Zombie Mode");
		//SceneManager.LoadScene("03b Game Over");
	}

	public IEnumerator KillUrself () {
		int courageTimer = UnityEngine.Random.Range(1, 20);
		DateTime game_over_time = DateTime.Now + TimeSpan.FromMinutes(courageTimer);
		GameManager.instance.gameOverTime = game_over_time;
		TimeSpan myScore = game_over_time - (GameManager.instance.timeCharacterStarted + GameManager.instance.serverTimeOffset);
		my_score = myScore;
		Debug.Log("My score: "+myScore.ToString()+" GameOverTime: "+gameOverTime.ToString());

		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime);
		form.AddField("client", "mob");
		form.AddField("score", myScore.ToString());

		if (myScore > GameManager.instance.high_score) {
			form.AddField("high_score", myScore.ToString());
		} else {
			form.AddField("high_score", "");
		}

		WWW www = new WWW(gameOverSuicideURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
            SceneManager.LoadScene("02b Zombie Mode");
            //SceneManager.LoadScene("03b Game Over");
		} else {
			Debug.Log(www.error);
		}
	}

	public void BlazeOfGloryActivate () {
		blazeOfGloryActive = true;
		LoadAltCombat(UnityEngine.Random.Range(50, 150), "blaze_of_glory");
		//form + web call

		
	}


	/// <summary>
	/// Fetchs the resume player data.
	/// </summary>
	/// <returns>The resume player data.</returns>
	public IEnumerator FetchResumePlayerData () {
		WWWForm form = new WWWForm();
		if (FB.IsLoggedIn == true) {
			form.AddField("id", GameManager.instance.userId);
		} else {
            Debug.Log("player is not logged in, cannot resume game");
            StopCoroutine(FetchResumePlayerData());

            //GameManager.instance.userId = "10154194346243928";
			//form.AddField("id", GameManager.instance.userId);
		}
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(resumeCharacterUrl, form);
		yield return www;

		if (www.error == null) {

			Debug.Log ("resuming character, server returned raw json string of: " + www.text);

			//write the raw WWW return to a .json file 
			//File.WriteAllText(Application.dataPath + "/Resources/Player.json", www.text.ToString());

			//read that text out into a string object, and map that to a json object
			string playerJsonString = www.text.ToString();
			JsonData playerJson = JsonMapper.ToObject(playerJsonString);

			if (playerJson[0].ToString() == "Success"){
				//update the GameManager.instance with all dataum
				GameManager.instance.userFirstName = playerJson[1]["first_name"].ToString() ;
				GameManager.instance.userLastName = playerJson[1]["last_name"].ToString();
				GameManager.instance.playerCurrentStamina = (int)playerJson[1]["curr_stamina"];
				GameManager.instance.playerMaxStamina = (int)playerJson[1]["max_stamina"];
				int wod = Convert.ToInt32(playerJson[1]["wood"].ToString());
                GameManager.instance.wood = wod;
                int met = Convert.ToInt32(playerJson[1]["metal"].ToString());
                GameManager.instance.metal = met;
				int wat = Convert.ToInt32(playerJson[1]["water"].ToString());
				GameManager.instance.waterCount = wat;
				int fud = Convert.ToInt32(playerJson[1]["food"].ToString());
				GameManager.instance.foodCount = fud;
				GameManager.instance.ammo = (int)playerJson[1]["ammo"];
				float homeLat = (float)Convert.ToDouble(playerJson[1]["homebase_lat"].ToString());
				GameManager.instance.homebaseLat = homeLat;
				float homeLon = (float)Convert.ToDouble(playerJson[1]["homebase_lon"].ToString());
				GameManager.instance.homebaseLong = homeLon;
				Debug.Log ("server returned a date time string of: " + playerJson[1]["char_created_DateTime"]);
				DateTime oDate = Convert.ToDateTime(playerJson[1]["char_created_DateTime"].ToString());
				GameManager.instance.timeCharacterStarted = oDate;
				if (playerJson[1]["homebase_set_time"].ToString() != "") {
					DateTime pDate = Convert.ToDateTime(playerJson[1]["homebase_set_time"].ToString());
					GameManager.instance.lastHomebaseSetTime = pDate;
				}

				//before any survivor records or player records are created the core data is initialized, and the boolean needs to be flipped to true.
				gameDataInitialized = true;
			} else if (playerJson[0].ToString() == "Failed") {
				Debug.Log(playerJson[1].ToString());
			}

			StartCoroutine (FetchWeaponData());

			yield break;
		} else {
			Debug.Log ("WWW error" + www.error);
		}

	}

	public IEnumerator FetchWeaponData () {
		//wipe all old data clean.
		GameObject[] oldWeapons = GameObject.FindGameObjectsWithTag("weaponcard");
		GameManager.instance.weaponCardList.Clear();
		foreach (GameObject weaponCard in oldWeapons) {
			Destroy(weaponCard.gameObject);
		}

		//get the data from the server
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(fetchWeaponDataURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData weaponJson = JsonMapper.ToObject(www.text);

			if (weaponJson[0].ToString() == "Success") {
				//parse through the entries to create new game objects, and add them to the list, and child them to the weapon card holder
				if(weaponJson[1].ToString() != "none") {
					for (int i=0; i < weaponJson[1].Count; i++) {
						BaseWeapon instance = Instantiate(baseWeaponPrefab);
						instance.transform.SetParent(weaponCardHolder.gameObject.transform);
						instance.weapon_id = (int)weaponJson[1][i]["weapon_id"];
						instance.equipped_id = (int)weaponJson[1][i]["equipped_id"];
						instance.gameObject.name = weaponJson[1][i]["name"].ToString();
						instance.base_dmg = (int)weaponJson[1][i]["base_dmg"];
						instance.modifier = (int)weaponJson[1][i]["modifier"];
						instance.stam_cost = (int)weaponJson[1][i]["stam_cost"];
						instance.durability = (int)weaponJson[1][i]["durability"];
                        instance.max_durability = (int)weaponJson[1][i]["max_durability"];
                        instance.miss_chance = (int)weaponJson[1][i]["miss_chance"];

						if (weaponJson[1][i]["type"].ToString() == "knife") {
							instance.weaponType = BaseWeapon.WeaponType.KNIFE;
						} else if (weaponJson[1][i]["type"].ToString() == "club") {
							instance.weaponType = BaseWeapon.WeaponType.CLUB;
						} else if (weaponJson[1][i]["type"].ToString() == "gun") {
							instance.weaponType = BaseWeapon.WeaponType.GUN;
						}
					}
				}
				weaponCardList.AddRange (GameObject.FindGameObjectsWithTag("weaponcard"));
				Debug.Log("all weapons added to the scene");

			} else if (weaponJson[0].ToString() == "Failed") {
				Debug.Log(weaponJson[1].ToString());
			}


		} else {
			Debug.Log(www.error);
		}
		StartCoroutine (FetchSurvivorData());

	}

	public void CreateSurvivorsFromGameManagerJson () {

		//delete all previous data from the gamemanager
		GameObject[] oldSurvivorCards = GameObject.FindGameObjectsWithTag("survivorcard");
		//Debug.Log(oldSurvivorCards.Length);
		GameManager.instance.activeSurvivorCardList.Clear();
		GameManager.instance.onMissionSurvivorCardList.Clear();
		GameManager.instance.injuredSurvivorCardList.Clear();
		if (oldSurvivorCards.Length > 0) {
			foreach (GameObject survivorCard in oldSurvivorCards) {
				Destroy(survivorCard.gameObject);
			}
		}

		//instantiate survivorcard gameobjects into the scene, load data onto the component, and set the parent to the current instance of GameManager.instance.survivorcardholder
		JsonData survivorJson = JsonMapper.ToObject(GameManager.instance.survivorJsonText);
		for (int i = 0; i < survivorJson.Count; i++) {
			SurvivorPlayCard instance = Instantiate(survivorPlayCardPrefab);
			instance.survivor.name = survivorJson[i]["name"].ToString();
			instance.gameObject.name = survivorJson[i]["name"].ToString();
			//instance.survivor.weaponEquipped.name = survivorJson[i]["weapon_equipped"].ToString();
			instance.survivor.baseAttack = (int)survivorJson[i]["base_attack"];
			instance.survivor.baseStamina = (int)survivorJson[i]["base_stam"];
			instance.survivor.curStamina = (int)survivorJson[i]["curr_stam"];
			instance.survivor.survivor_id = (int)survivorJson[i]["entry_id"];
			instance.entry_id = (int)survivorJson[i]["entry_id"];
			//instance.survivor_id = (int)survivorJson[1][i]["survivor_id"];
			instance.team_pos = (int)survivorJson[i]["team_position"];
			instance.profilePicURL = survivorJson[i]["profile_pic_url"].ToString();



            if (survivorJson[i]["onMission"].ToString() == "1") {
				instance.onMission = true;
                instance.dead = false;
				onMissionSurvivorCardList.Add(instance.gameObject);//add to "away on mission" list
			} else if (survivorJson[i]["injured"].ToString() != "0") { //injuries are stored by injury_id, NOT 0 means injured
                instance.dead = false;
                instance.onMission = false;
				injuredSurvivorCardList.Add(instance.gameObject); //add to injured list
			} else if (survivorJson[i]["dead"].ToString() == "1"){
                instance.dead = true;
                deadSurvivorCardList.Add(instance.gameObject); //add to the dead survivor list
            }else{
                instance.onMission = false;
                instance.dead = false;
                activeSurvivorCardList.Add(instance.gameObject); //add to the active+alive survivor list
            }


			instance.injury = (int)survivorJson[i]["injured"];

			instance.transform.SetParent(GameManager.instance.survivorCardHolder.transform);
		}


		/*
		//check for missing team positions...
		for (int i=0; i < activeSurvivorCardList.Count; i++) {
			int pos_expected = 5-i;
			if (pos_expected <= 0) {
				break;
			}
			bool found = false;
			foreach (GameObject survivor in activeSurvivorCardList) {
				SurvivorPlayCard myPlayCard = survivor.GetComponent<SurvivorPlayCard>();
				if (myPlayCard.team_pos == pos_expected) {
					found = true;
					break;
				}
			}

			if (found == false) {
				//find a survivor at 0 and set them to the missing #
				foreach (GameObject survivor in activeSurvivorCardList) {
					SurvivorPlayCard thisPlayCard = survivor.GetComponent<SurvivorPlayCard>();
					if (thisPlayCard.team_pos == 0) {
						thisPlayCard.team_pos = pos_expected;
						break;
					} else {
						continue;
					}
				}
			} else if (found == true) {
				continue;
			}

		}
		*/

		//continue to the weapons
		CreateWeaponsFromGameManagerJson();
	}

	public void CreateWeaponsFromGameManagerJson () {
		//wipe all old data clean.
		GameObject[] oldWeapons = GameObject.FindGameObjectsWithTag("weaponcard");
		//Debug.Log(oldWeapons.Length+" weapons found to be destoyed");
		//Debug.Log("There are "+GameManager.instance.weaponCardList.Count+" items in the list before clearing");
		GameManager.instance.weaponCardList.Clear();
		//GameManager.instance.weaponCardList = new List<GameObject>();
		//Debug.Log("There are "+GameManager.instance.weaponCardList.Count+" items in the card list after clear");
		//destroy the old weapons
		foreach (GameObject weaponCard in oldWeapons) {
			Destroy(weaponCard.gameObject);
		}


		if (GameManager.instance.weaponJsonText != null) {
			JsonData weaponJson = JsonMapper.ToObject(GameManager.instance.weaponJsonText);
			for (int i=0; i < weaponJson.Count; i++) {
				BaseWeapon instance = Instantiate(baseWeaponPrefab);
				instance.transform.SetParent(weaponCardHolder.gameObject.transform);
				instance.weapon_id = (int)weaponJson[i]["weapon_id"];
				instance.equipped_id = (int)weaponJson[i]["equipped_id"];
				instance.gameObject.name = weaponJson[i]["name"].ToString();
				instance.base_dmg = (int)weaponJson[i]["base_dmg"];
				instance.modifier = (int)weaponJson[i]["modifier"];
				instance.stam_cost = (int)weaponJson[i]["stam_cost"];
				instance.durability = (int)weaponJson[i]["durability"];
                instance.max_durability = (int)weaponJson[i]["max_durability"];
                instance.miss_chance = (int)weaponJson[i]["miss_chance"];

				if (weaponJson[i]["type"].ToString() == "knife") {
					instance.weaponType = BaseWeapon.WeaponType.KNIFE;
				} else if (weaponJson[i]["type"].ToString() == "club") {
					instance.weaponType = BaseWeapon.WeaponType.CLUB;
				} else if (weaponJson[i]["type"].ToString() == "gun") {
					instance.weaponType = BaseWeapon.WeaponType.GUN;
				}
			}
		} else {
			Debug.Log("No weapons found in gamemanager json data");
		}
			
		weaponCardList.AddRange(GameObject.FindGameObjectsWithTag("weaponcard"));

		MergeWeaponAndSurvivorRecords();
	}

	void MergeWeaponAndSurvivorRecords () {
		GameManager.instance.weaponCardList.Clear();
		GameManager.instance.weaponCardList.AddRange(GameObject.FindGameObjectsWithTag("weaponcard"));

		//for each weapon equipped- compare it's equipped ID with active survivor ID's
		foreach (GameObject weapon in weaponCardList) {
			BaseWeapon myWeapon = weapon.GetComponent<BaseWeapon>();

			if (myWeapon.equipped_id !=0) {
				//ensure there is not a duplicate equipped ID
				foreach (GameObject comp_weapon in weaponCardList) {
					BaseWeapon comp_weapon_data = comp_weapon.GetComponent<BaseWeapon>();
					if (comp_weapon_data.equipped_id == myWeapon.equipped_id && comp_weapon_data.weapon_id != myWeapon.weapon_id) {
						//if a duplicate ID is found, that isn't the same weapon_id- unequip this weapon.
						myWeapon.equipped_id = 0;
						break;
					}
				}
			}

			if (myWeapon.equipped_id !=0) {
				//ensure that the survivor exists
				List <GameObject> allSurvivorList = new List<GameObject>();
				allSurvivorList.AddRange(GameObject.FindGameObjectsWithTag("survivorcard"));
				int count = 0;

				foreach (GameObject survivor in allSurvivorList) {
					SurvivorPlayCard surv = survivor.GetComponent<SurvivorPlayCard>();

					if (surv != null && surv.survivor.survivor_id == myWeapon.equipped_id) {
						break;
					} else {
						count ++;
					}
					//if all survivors ID's have been checked, and none match this weapon equipped ID
					if (count >= allSurvivorList.Count) {
						myWeapon.equipped_id = 0;
					}
				}
			}

			//if the weapon is assigned an equipped id- associate the gameobjects
			if (myWeapon.equipped_id != 0) {


				int count = 0;
				//now loop through the player cards, and find a matching player card ID.
				foreach (GameObject survivorCard in activeSurvivorCardList) {
					SurvivorPlayCard SPC = survivorCard.GetComponent<SurvivorPlayCard>();

					if (SPC.entry_id == myWeapon.equipped_id) {
						//This weapon has been previously assigned to this player- add the game object to the card
						SPC.survivor.weaponEquipped = weapon.gameObject;
						break;
					} else {
						count++;
					}

					if (count >= activeSurvivorCardList.Count) {
						SPC.survivor.weaponEquipped = null;
						myWeapon.equipped_id = 0;
					}
				}

				//if you have gone through every survivor, and not found a match


			} else {
				continue;
			}
		}
	}


	/// <summary>
	/// Fetchs the survivor data.
	/// </summary>
	/// <returns>The survivor data.</returns>
	public IEnumerator FetchSurvivorData () {
		//delete all previous data from the gamemanager
		GameObject[] oldSurvivorCards = GameObject.FindGameObjectsWithTag("survivorcard");
		//Debug.Log(oldSurvivorCards.Length);
		GameManager.instance.activeSurvivorCardList.Clear();
		GameManager.instance.onMissionSurvivorCardList.Clear();
		if (oldSurvivorCards.Length > 0) {
			foreach (GameObject survivorCard in oldSurvivorCards) {
				Destroy(survivorCard.gameObject);
			}
		}

		//construct form
		WWWForm form = new WWWForm();
		if (FB.IsLoggedIn == true) {
			form.AddField("id", GameManager.instance.userId);
		} else {
			GameManager.instance.userId = "10154194346243929";
			form.AddField("id", GameManager.instance.userId);
		}
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		//make www call
		WWW www = new WWW(fetchSurvivorDataURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			//encode json return
			string survivorJsonString = www.text;
			JsonData survivorJson = JsonMapper.ToObject(survivorJsonString);

			if (survivorJson[0].ToString() == "Success") {
				//parse through json creating "player cards" within gamemanager for each survivor found on the server.
				for (int i = 0; i < survivorJson[1].Count; i++) {
					SurvivorPlayCard instance = Instantiate(survivorPlayCardPrefab);
					instance.survivor.name = survivorJson[1][i]["name"].ToString();
					instance.gameObject.name = survivorJson[1][i]["name"].ToString();
					//instance.survivor.weaponEquipped.name = survivorJson[i]["weapon_equipped"].ToString();
					instance.survivor.baseAttack = (int)survivorJson[1][i]["base_attack"];
					instance.survivor.baseStamina = (int)survivorJson[1][i]["base_stam"];
					instance.survivor.curStamina = (int)survivorJson[1][i]["curr_stam"];
					instance.survivor.survivor_id = (int)survivorJson[1][i]["entry_id"];
					instance.entry_id = (int)survivorJson[1][i]["entry_id"];
					//instance.survivor_id = (int)survivorJson[1][i]["survivor_id"];
					instance.team_pos = (int)survivorJson[1][i]["team_pos"];
					instance.profilePicURL = survivorJson[1][i]["pic_url"].ToString();
					if (survivorJson[1][i]["onMission"].ToString() == "1") {
						instance.onMission = true;
						onMissionSurvivorCardList.Add(instance.gameObject);
					} else if (survivorJson[1][i]["onMission"].ToString() == "0") {
						instance.onMission = false;
						activeSurvivorCardList.Add(instance.gameObject);
					}


					instance.transform.SetParent(GameManager.instance.survivorCardHolder.transform);
				}
			} else {
				Debug.Log(survivorJson[1].ToString());
			}

			//StartCoroutine (FetchOutpostData());
			//survivorCardList.AddRange (GameObject.FindGameObjectsWithTag("survivorcard"));

			//auto-load map level from login(1) , if on map level stay there, if on victory screen, let the user press the button to load map level.
			if (SceneManager.GetActiveScene().buildIndex != 2 && SceneManager.GetActiveScene().buildIndex != 4 && playerInTutorial==false) {
				SceneManager.LoadScene("02a Map Level");
			}

			//We need a function to match the weapons to the correct players, and equip them- if they are equipped.
			MergeWeaponAndSurvivorRecords ();


			if (updateWeaponAndSurvivorMapLevelUI == true) {
				MapLevelManager mapLvlMgr = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
				//mapLvlMgr.theWeaponListPopulator.PopulateWeaponsFromGameManager();
				//mapLvlMgr.theSurvivorListPopulator.RefreshFromGameManagerList();
				mapLvlMgr.UpdateTheUI();
				updateWeaponAndSurvivorMapLevelUI = false;
			}

			if (playerInTutorial == true && weaponHasBeenSelected == true) {
				string tut = "tutorial";
				LoadAltCombat(1, tut);
			}

		} else {
			Debug.LogWarning(www.error);
		}
	}


	public IEnumerator FetchOutpostData () {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(fetchOutpostDataURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			//GameManager.instance.outpostJsonText = www.text; //need to remove the "Success" from [0]
			JsonData outpostJson = JsonMapper.ToObject(www.text);

			if (outpostJson[0].ToString() == "Success") {
                //store the text for later use
                GameManager.instance.outpostJsonText = JsonMapper.ToJson(outpostJson[1]);//the results should all be in this array position
			} else if (outpostJson[0].ToString() == "Failed") {
				Debug.Log(outpostJson[1].ToString());

			}
		} else {
			Debug.Log(www.error);
		}
		//if we are on the map level- place the graphic
		if(activeScene.buildIndex == 2) {
			BuildingSpawner myBldgSpwnr = BuildingSpawner.FindObjectOfType<BuildingSpawner>();
			myBldgSpwnr.SpawnOutpostsToMap();
		}
		//StartCoroutine (FetchMissionData());
	}

	public IEnumerator FetchMissionData () {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(fetchMissionDataURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			missionJsonText = www.text;
			JsonData wwwReturnJson = JsonMapper.ToObject(www.text);
			GameManager.instance.missionData = wwwReturnJson;
			if(wwwReturnJson[0].ToString() == "Success") {
				//set the public game object
				Debug.Log(wwwReturnJson[1].ToString());
			} else {
				Debug.Log(wwwReturnJson[1].ToString());
			}
		} else {
			Debug.Log(www.error);
		}	
		//if we are on the map screen, update the mission UI
		if(activeScene.buildIndex == 2){
			MapLevelManager myMapLvlMgr = MapLevelManager.FindObjectOfType<MapLevelManager>();
			myMapLvlMgr.theMissionListPopulator.LoadMissionsFromGameManager();
				
		}
	}
	
	public void SetDaysSurvived () {
		DateTime now = System.DateTime.Now;
		Double days = (now - timeCharacterStarted).TotalDays;
		daysSurvived = Convert.ToInt32(days);

		//Debug.Log ("The SetDaysSurvived function has returned: " + days + " Days since character created");
	}

    public TimeSpan GetCurrentTimeAlive ()
    {
        TimeSpan time_alive = (DateTime.Now - GameManager.instance.serverTimeOffset) - GameManager.instance.timeCharacterStarted;
        return time_alive;
    }

	public void StartNewCharacter () {
		//Record the date and time the character is created- will be compared to get Days alive later.
		timeCharacterStarted = System.DateTime.Now;

//		if (FB.IsLoggedIn == false) {
//			GameManager.instance.userId = "10154194346243929";
//			GameManager.instance.userFirstName = "Tanderson";
//			GameManager.instance.userLastName = "Flickinhausen";
//			GameManager.instance.userName = "Tanderson Flickenhausen";
//		}


		//roll a random number of survivors left alive and set both active and alive to that number.

		GameManager.instance.wood = UnityEngine.Random.Range(20, 70);
        GameManager.instance.metal = UnityEngine.Random.Range(20, 70);
		GameManager.instance.waterCount = UnityEngine.Random.Range(10, 20);
		GameManager.instance.foodCount = UnityEngine.Random.Range(15, 30);
		GameManager.instance.ammo = UnityEngine.Random.Range(0,20);
//		GameManager.instance.playerMaxStamina = 100;
//		GameManager.instance.playerCurrentStamina = 100;
		GameManager.instance.homebaseLat = 0.0f;
		GameManager.instance.homebaseLong = 0.0f;
		GameManager.instance.lastLoginTime = "12/31/1999 11:59:59";
        GameManager.instance.activeBldg_zombies = 1; //set up the 1 zombie for the weapon-select combat
        GameManager.instance.activeBldg_zAcross = 1; //set up the building data for weapon-select combat
		playerInTutorial = true;

		StartCoroutine (NewCharacterUpdateServer());

		//Debug.Log ("Character started at: " + timeCharacterStarted);
	}

	public void ResumeCharacter () {
		GameManager.instance.lastLoginTime = "12/31/1999 11:59:59";
		//Debug.Log(GameManager.instance.lastLoginTime.ToString());
		//NOTE: This coroutine also calls FetchSurvivorData and FetchWeaponData- due to order of operations in associating the 2 databases, I placed the StartCoroutine at the end of each other coroutine
		//StartCoroutine (FetchResumePlayerData());
		StartCoroutine (LoadAllGameData());
	}

	public void RenewWeaponAndEquippedData () {
		//Because these 4 coroutines go in order, this starts and executes the last 2 updates- weapon data, survivor data, and merging the 2 records in the client
		StartCoroutine(FetchWeaponData());
	}

	public void RestartTheGame () {
		StartNewCharacter ();
		SceneManager.LoadScene("02a Map Level");// consider storing these in a public static array that can just name locations, and possibly swap backgrounds.
	}

	public void PaidRestartOfTheGame () {
		

		int newWood = Mathf.RoundToInt(this.wood * 0.75f);
        this.wood = newWood;
        int newMetal = Mathf.RoundToInt(this.metal * 0.75f);
		int newFood = Mathf.RoundToInt(this.foodCount / 2);
		foodCount = newFood;
		int newWater = Mathf.RoundToInt(this.waterCount / 2);
		waterCount = newWater;


//		GamePreferences.SetSupply(supply);
//		GamePreferences.SetTotalSurvivors (totalSurvivors);
//		GamePreferences.SetActiveSurvivors (survivorsActive);
//		GamePreferences.SetWaterCount (waterCount);
//		GamePreferences.SetFoodCount (foodCount);
		//GameManager.instance.UpdateAllStatsToGameMemory();

		SceneManager.LoadScene("02a Map Level");
	}

	public void SetPublicPlayerHealth (int playerStamina) {
		playerCurrentStamina = playerStamina;

		//GameManager.instance.UpdateAllStatsToGameMemory();
//		GamePreferences.SetLastPlayerCurrentHealth(playerHealth);
		//this is for external setting of the permenant game object GameManager.instance
	}

	public void SetHomebaseLocation (float lat, float lon) {
		this.homebaseLat = lat;
		this.homebaseLong = lon;

		//UpdateAllStatsToGameMemory();
//		GamePreferences.SetHomebaseLattitude(lat);
//		GamePreferences.SetHomebaseLongitude(lon);
	}

    public void LoadBossCombat (string boss)
    {
        activeBldg_name = boss;

        SceneManager.LoadScene("02c Combat-Boss");
    }

	public void LoadAltCombat (int zomb,string bldg_name) {
		activeBldg_name = bldg_name;
        activeBldg_zombies = zomb;
		//zombiesToFight = zomb;
        
        SceneManager.LoadScene("02e Combat-15");//all zombie combat is handled in this one scene now
        //LoadRandomCombatScene();
    }

	public void LoadBuildingCombat () {
		survivorFound = false;
        Debug.Log("Calling load into building ****************************");

        //store the active building stats for win screen reporting.
        GameManager.instance.reportedWood = GameManager.instance.activeBldg_wood;
        GameManager.instance.reportedMetal = GameManager.instance.activeBldg_metal;
        GameManager.instance.reportedFood = GameManager.instance.activeBldg_food;
        GameManager.instance.reportedWater = GameManager.instance.activeBldg_water;

		//if the building has not been visited before- roll it's contents before entering
		if (GameManager.instance.activeBldg_lastclear == DateTime.Parse("11:59pm 12/31/1999")) {
			StartCoroutine(RollNewBuildingContents(false));
		} else {
            //if player has been here before- the correct data should already be loaded
            //SceneManager.LoadScene ("02c Combat-5");
            SceneManager.LoadScene("02e Combat-15");
            //LoadRandomCombatScene();
		}


	}

	public IEnumerator RollNewBuildingContents (bool mission) {
		WWWForm form = new WWWForm();
		form.AddField ("id", GameManager.instance.userId);
		form.AddField ("login_ts", GameManager.instance.lastLoginTime);
		form.AddField ("client", "mob");

		#region BUILDING CONTENTS GENERATION ALGORITHIM HERE
		//declare possible stats
		int wood = 0;
        int metal = 0;
		int food = 0;
		int water = 0;

        //determine core odds
        float max_percentage = 0.75f; //players start with 75% chance to find stuff day1min1
        float coreFalloffOdds = CalculateCoreFalloffOdds(max_percentage); //this will return a %value represented 0.0-1.0 so it can be used in the rolls
        float coreInversFalloffOdds = 1.0f-coreFalloffOdds;//this will increase 
        Debug.Log("Calculating core Falloff odds at: " + coreFalloffOdds + " and his inverse at: " + coreInversFalloffOdds);

        #region roll resources based on location type
        //roll based on type
        if (GameManager.instance.activeBldg_lootcode == "S") { //S-for supply. raw materials most likely here
			int wod = UnityEngine.Random.Range(20, 110);
            int met = UnityEngine.Random.Range(5, 90);
			int fud = UnityEngine.Random.Range(0, 10);
			int wat = UnityEngine.Random.Range(0, 10);

			
			float roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll <= coreInversFalloffOdds) {
				fud =0;
			}
			roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll <= coreInversFalloffOdds) {
				wat =0;
			}

            float supply_roll = UnityEngine.Random.Range(0.0f, 1.0f);
            //supply locations will always have a 15% chance of being empty
            if (supply_roll < 0.15f) 
            {
                //wood
                met = 0;
            }else if (supply_roll < 0.15f)
            {
                //metal
                wod = 0;
            } //else let both pass
        
			wood = wod;
            metal = met;
			food = fud;
			water = wat;

		} else if (GameManager.instance.activeBldg_lootcode == "F") { //F-for food.

			int wod = UnityEngine.Random.Range(0, 20);
            int met = UnityEngine.Random.Range(0, 15);
			int fud = UnityEngine.Random.Range(5, 45);
			int wat = UnityEngine.Random.Range(0, 30);

			
			float roll = UnityEngine.Random.Range(0.0f, 1.0f); //players roll
			if (roll <= coreInversFalloffOdds) { //using coreInverseFalloff gives a float that increases with time- approaching 1.0- 100%
				wod =0;
                met = 0;
			}
			roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll <= coreInversFalloffOdds) {
				wat =0;
			}
            if (roll <= coreInversFalloffOdds)
            {
                fud = 0;
            }

			wood = wod;
            metal = met;
			food = fud;
			water = wat;
			
		} else if (GameManager.instance.activeBldg_lootcode == "W") { //W for water.

			int wod = UnityEngine.Random.Range(0, 10);
            int met = UnityEngine.Random.Range(0, 10);
			int fud = UnityEngine.Random.Range(0, 20);
			int wat = UnityEngine.Random.Range(5, 55);

			
			float roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll <= coreInversFalloffOdds) {
				fud =0;
			}
            if(roll<= coreInversFalloffOdds)
            {
                wat = 0;
            }
			roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll <= coreInversFalloffOdds) {
				wod =0;
                met = 0;
			}
			wood = wod;
            metal = met;
			food = fud;
			water = wat;

		} else if (GameManager.instance.activeBldg_lootcode == "G") {//G for general

			int wod = UnityEngine.Random.Range(0, 40);
            int met = UnityEngine.Random.Range(0, 40);
            int fud = UnityEngine.Random.Range(0, 45);
			int wat = UnityEngine.Random.Range(0, 45);

			float roll = UnityEngine.Random.Range(0.0f, 1.0f);
			if (roll <= coreInversFalloffOdds) {
				fud =0;
				wat =0;
                wod = 0;
                met = 0;
            }
			wood = wod;
            metal = met;
			food = fud;
			water = wat;

		}


        #endregion

        #region Zombie population rolls
        //ZOMBIE ROLLS
        //int zombie_pop = UnityEngine.Random.Range(5, 25); //depreciated- rolls should be dynamic now

        int zombie_pop = 0;//initialize count
        //find out how long the player has been alive
        double days_alive = (DateTime.Now - GameManager.instance.timeCharacterStarted).TotalDays;
        if (days_alive < 1)
        {
            //on the first day
            zombie_pop = UnityEngine.Random.Range(0, 5);
        }else if (days_alive < 2)
        {
            //day 2...heating up...
            zombie_pop = UnityEngine.Random.Range(1, 10);
        }else
        {
            //for the rest of the time...
            zombie_pop = UnityEngine.Random.Range(5, 25);
        }

        /*
        //RANDOMIZER- we want to randomly encounter NO zombies, and NO loot.
        int num_one = UnityEngine.Random.Range(1, 4);
        int num_two = UnityEngine.Random.Range(1, 4);
        if (num_one == num_two)
        {
            zombie_pop = 0;
            int num_three = UnityEngine.Random.Range(1, 4);
            if (num_three == num_one)
            {
                food = 0;
                water = 0;
                wood = 0;
                metal = 0;
            }
        }
        */


        //ZOMBIES ACROSS: this generates the integer that sets how many zombie models load in combat AKA how many a player must fight at once at this location
        int zombies_across = 1;
        //this should let us control the odds in 5% chunks
        int across_roll = UnityEngine.Random.Range(0, 20);
        if (across_roll < 10)
        {
            zombies_across = UnityEngine.Random.Range(2, 6);//lowest
        }else if (across_roll < 17)
        {
            zombies_across = UnityEngine.Random.Range(4, 9);//harder
        }else
        {
            zombies_across = UnityEngine.Random.Range(6, 15);//hardest
        }



        #endregion

        //STORE THE ROLLS TO THE GAMEMANAGER
        //DateTime new_bldg_datetime = DateTime.Parse("12:01am 1/1/2000");
        GameManager.instance.activeBldg_zombies = zombie_pop;
		GameManager.instance.activeBldg_food = food;
		GameManager.instance.activeBldg_water = water;
		GameManager.instance.activeBldg_wood = wood;
        GameManager.instance.activeBldg_metal = metal;
        GameManager.instance.activeBldg_zombies = zombie_pop;
		//GameManager.instance.zombiesToFight = zombie_pop;
        GameManager.instance.activeBldg_zAcross = zombies_across;

		Debug.Log("Wood: "+wood.ToString()+" metal: "+metal.ToString()+" food: "+food.ToString()+" water: "+water.ToString()+" zombies: "+zombie_pop.ToString());
        SceneManager.LoadScene("02e Combat-15");

        #endregion //THIS IS WHERE BUILDING LOOT IS DETERMINED IN CODE *****

        form.AddField("wood", wood);
        form.AddField("metal", metal);
		form.AddField("food", food);
		form.AddField("water", water);
		form.AddField("zombies", zombie_pop);
        form.AddField("zombies_across", zombies_across); //this represents how many zombies the player will fight at once
		form.AddField("bldg_name", GameManager.instance.activeBldg_name);
		form.AddField("bldg_id", GameManager.instance.activeBldg_id);

		WWW www = new WWW(newBuildingURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData newBldgJson = JsonMapper.ToObject(www.text);
			if (newBldgJson[0].ToString() == "Success") {
				clearedBldgJsonText = JsonMapper.ToJson(newBldgJson[1]);
				Debug.Log(clearedBldgJsonText);

				//if the mission tab is triggering this, don't load player into combat
				if (mission == false) {
                    Debug.Log("Successfully registered new building on the server");
                    //SceneManager.LoadScene("02d Combat-10");
                    //SceneManager.LoadScene("02e Combat-15"); //now calling earlier in this coroutine
                    //LoadRandomCombatScene();
                    
				}
			} else {
				Debug.Log(newBldgJson[1].ToString());
			}
		} else {
			Debug.Log(www.error);
		}
	}

    void LoadRandomCombatScene () //depreciated
    {
        int roll = UnityEngine.Random.Range(1, 3);
        if ( roll == 1 )
        {
            SceneManager.LoadScene("02c Combat-5");
        }
        else if ( roll == 2 )
        {
            SceneManager.LoadScene("02d Combat-10");
        }
        else
        {
            SceneManager.LoadScene("02e Combat-15");
        }
    }

	public void AddTimePlayed () {
		timeCharacterStarted  = timeCharacterStarted.AddHours(-1.0);
//		GamePreferences.SetDayTimeCharacterCreated (timeCharacterStarted.ToString());

		//right now this will not save to permenant memory- this should be handled in its own function/script- so it's not accidentally changed.
		//UpdateAllStatsToGameMemory();
		SetDaysSurvived ();
		Debug.Log ("1 Hour added to time started. New Datetime is: " + timeCharacterStarted.ToString() );
	}

	/*
	public void DeactivateClearedBuildingsFromJson () {

		JsonData clearedBldgJson = JsonMapper.ToObject(GameManager.instance.clearedBldgJsonText);

		//ensure there are more than 0 buildings returned
    	if (clearedBldgJson.Count > 0) {
			for (int i = 0; i < clearedBldgJson.Count; i++) {
	    		//if the building is still considered inactive by the server
				if (clearedBldgJson[i]["active"].ToString() == "0") {
	    			//Debug.Log ("Coroutine has found "+ clearedJson[i]["bldg_name"].ToString()+" to be inactive");
					GameObject thisBuilding = GameObject.Find(clearedBldgJson[i]["bldg_name"].ToString());
					if (thisBuilding != null) {
	    				PopulatedBuilding populatedBldg = thisBuilding.GetComponent<PopulatedBuilding>();
						//Debug.Log ("GameManager is attempting to deactivate "+populatedBldg.gameObject.name);
						populatedBldg.DeactivateMe();
	    			} else {
	    				continue;
	    			}

				} else if (clearedBldgJson[i]["active"].ToString() == "1") {
					//Debug.Log (clearedJson[i]["bldg_name"].ToString()+" has been reactivated by the server, but remains on player DB. Last cleared DateTime: "+clearedJson[i]["time_cleared"].ToString());
	    		}
	    	}

    	} else {
    		Debug.Log ("Player has not cleared any buildings yet");
    	}
	}

	*/


	public bool clearedBuildingSendInProgress = false;
	public void BuildingIsCleared (bool survFound) {
        GameManager.instance.survivorFound = survFound;
		if (clearedBuildingSendInProgress == false) {
			clearedBuildingSendInProgress = true;

			StartCoroutine(SendClearedBuilding(survFound));
		} 
	}

	IEnumerator SendClearedBuilding (bool survivorFound) {
        //spawn building clear text and SFX
        GameObject canvas = GameObject.Find("Canvas");
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        BuildingClearTextController bldg_clear_text = Resources.Load<BuildingClearTextController>("Prefabs/Building Clear Notification Panel").GetComponent<BuildingClearTextController>();
        BuildingClearTextController instance = Instantiate(bldg_clear_text);
        instance.transform.SetParent(canvas.transform);
        instance.transform.position = center;
        AnimatorClipInfo[] clipinfo = instance.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        float wait_for = clipinfo[0].clip.length;
        Debug.Log("waiting for... " + wait_for.ToString());
        yield return new WaitForSeconds(wait_for);
        Debug.Log("done waiting...");

        //determine the type of clear that's taken place
		if (GameManager.instance.activeBldg_name != "tutorial") {
			if (GameManager.instance.activeBldg_name != "zomb") {
				if(GameManager.instance.activeBldg_name != "blaze_of_glory"){
					if (GameManager.instance.activeBldg_name != "bite_case") {

						string jsonString = GameManager.instance.locationJsonText;
						JsonData bldgJson = JsonMapper.ToObject(jsonString);
						string bldg_id = "";

//						for (int i = 0; i < bldgJson["results"].Count; i++) {
//							if (bldgJson["results"][i]["name"].ToString() == GameManager.instance.activeBldg_name) {
//								bldg_id = bldgJson["results"][i]["id"].ToString();
//								Debug.Log("sending bldg_id: "+bldg_id);
//							}
//						}

						WWWForm wwwForm = new WWWForm();
						wwwForm.AddField("id", GameManager.instance.userId);
						wwwForm.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
						wwwForm.AddField("client", "mob");

						wwwForm.AddField("bldg_name", GameManager.instance.activeBldg_name);
						wwwForm.AddField("bldg_id", GameManager.instance.activeBldg_id);
                        //wwwForm.AddField("supply", GameManager.instance.reportedSupply);
                        //wwwForm.AddField("food" , GameManager.instance.reportedFood);
                        //wwwForm.AddField("water", GameManager.instance.reportedWater);

                        if (survivorFound == true) {

                            //calculate the number of survivors found.
                            int survivor_count = 1;
                            int days = Mathf.FloorToInt((float)(DateTime.Now - GameManager.instance.timeCharacterStarted).TotalDays);
                            if (days < 1)
                            {
                                int group_bonus = UnityEngine.Random.Range(1,5);
                                float odds = 90.0f;
                                float roll = UnityEngine.Random.Range(0.0f, 100.0f);
                                if (roll < odds)
                                {
                                    survivor_count += group_bonus;
                                }
                            }else if (days < 3)
                            {
                                int group_bonus = UnityEngine.Random.Range(1, 4);
                                float odds = 70.0f;
                                float roll = UnityEngine.Random.Range(0.0f, 100.0f);
                                if (roll < odds)
                                {
                                    survivor_count += group_bonus;
                                }
                            }
                            else if (days < 8)
                            {
                                int group_bonus = UnityEngine.Random.Range(1, 2);
                                float odds = 50.0f;
                                float roll = UnityEngine.Random.Range(0.0f, 100.0f);
                                if (roll < odds)
                                {
                                    survivor_count += group_bonus;
                                }
                            }
                            else if (days < 15)
                            {
                                int group_bonus = UnityEngine.Random.Range(1, 2);
                                float odds = 25.0f;
                                float roll = UnityEngine.Random.Range(0.0f, 100.0f);
                                if (roll < odds)
                                {
                                    survivor_count += group_bonus;
                                }
                            }
                            else if (days > 30)
                            {
                                int group_bonus = UnityEngine.Random.Range(0, 1);
                                float odds = 15.0f;
                                float roll = UnityEngine.Random.Range(0.0f, 100.0f);
                                if (roll < odds)
                                {
                                    survivor_count += group_bonus;
                                }
                            }

                            Debug.Log("Found " + survivor_count.ToString() + " survivors to add to the team");
                            wwwForm.AddField("survivor_found", survivor_count);
						} else {
                            Debug.Log("Found no survivors according to GameManager Coroutine");
							wwwForm.AddField("survivor_found", "0");
						}

						Debug.Log ("sending cleared building message to the server- bldg_name: "+GameManager.instance.activeBldg_name+" and id: "+GameManager.instance.activeBldg_id+" Survivor Found status: "+survivorFound);
						WWW www = new WWW(buildingClearedURL, wwwForm);
						yield return www;

						if (www.error == null) {
							Debug.Log(www.text);

							JsonData buildingClearReturn = JsonMapper.ToObject(www.text);

							if (buildingClearReturn[0].ToString() == "Success") {
								Debug.Log(buildingClearReturn[1].ToString());

								//if there has been a survivor added to the players team.
								if ((int)buildingClearReturn[2] > 0) {
                                    foundSurvivorJsonText = JsonMapper.ToJson(buildingClearReturn[3]);
                                    Debug.Log(foundSurvivorJsonText);
                                    /*
									foundSurvivorName = buildingClearReturn[3][0]["name"].ToString();
									foundSurvivorCurStam = (int)buildingClearReturn[3][0]["base_stam"];
									foundSurvivorMaxStam = (int)buildingClearReturn[3][0]["base_stam"];
									foundSurvivorAttack = (int)buildingClearReturn[3][0]["base_attack"];
									foundSurvivorEntryID = (int)buildingClearReturn[3][0]["entry_id"];
								    */
                                }
							}
						} else {
							Debug.Log(www.error);
						}
						SceneManager.LoadScene ("03a Win");
					} else {
						
						//Player has been bit in combat. All end-game data has already been stored, but player played to the end of combat.
						GameManager.instance.playerIsZombie = true;
                        SceneManager.LoadScene("02a Zombie Mode");
						//SceneManager.LoadScene("03b Game Over");

					}
				} else {

					//player has DIED in a BLAZE OF GLORY!!! game must be ended

					DateTime game_over_time = DateTime.Now;
					TimeSpan myScore = game_over_time - GameManager.instance.timeCharacterStarted;
					Debug.Log("calculaing my score to be: "+myScore);
					GameManager.instance.my_score = myScore;


					WWWForm form = new WWWForm();
					form.AddField("id", GameManager.instance.userId);
					form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
					form.AddField("client", "mob");

					form.AddField("game_over_datetime", game_over_time.ToString());
					if (GameManager.instance.high_score < myScore) {
						form.AddField("high_score", myScore.ToString());
					} else {
						form.AddField("high_score", "");
					}

					WWW www = new WWW(gameOverBlazeOfGloryURL, form);
					yield return www;
					Debug.Log(www.text);

					if (www.error == null) {
                        SceneManager.LoadScene("02b Zombie Mode");
                        //SceneManager.LoadScene("03b Game Over");
					} else {
						Debug.Log(www.error);
					}

				}
			} else {
				//player has killed another players zombie.
				WWWForm form = new WWWForm();
				form.AddField("id", GameManager.instance.userId);
				form.AddField("login_ts", GameManager.instance.lastLoginTime);
				form.AddField("client", "mob");

				form.AddField("zombie_id", GameManager.instance.zombie_to_kill_id);

				WWW www = new WWW(zombieKillURL, form);
				yield return www;

				if (www.error == null) {
					Debug.Log(www.text);
					JsonData zombie_json = JsonMapper.ToObject(www.text);

					if (zombie_json[0].ToString() == "Success") {
						if (zombie_json[1].ToString() == "reward") {
							Debug.Log("Player Successfully killed another players zombie");

						} else {
							Debug.Log("This player was no longer a zombie");
							GameManager.instance.zombie_to_kill_id = "";
						}
					}

				} else {
					Debug.Log(www.error);
				}
				//this coroutine is called from Battlemanager. If player has killed a zombie- move to win screen.
				SceneManager.LoadScene("03a Win");
			}

		} else {
			//if player is trying to send the tutorial building for reward- skip it, load game data, and load into map level.
			playerInTutorial = false;
			weaponHasBeenSelected = false;
			StartCoroutine(LoadAllGameData());
			SceneManager.LoadScene("02a Map Level");
			StopCoroutine(SendClearedBuilding(false));
		}
		yield return new WaitForSeconds(2.0f);
		clearedBuildingSendInProgress=false;
		//StartCoroutine(FetchResumePlayerData());  //this is the old train of queries to load game data
		StartCoroutine(LoadAllGameData()); //this is the consolodated cach-all data loader
		//GameManager.instance.DeactivateClearedBuildings();  //this should be called from the map level manager on map level load
	}

	public void PublicStartLocationServices () {
		StartCoroutine(StartLocationServices());
	}


	IEnumerator StartLocationServices () {
		if (!Input.location.isEnabledByUser){
			Debug.Log ("location services not enabled by user");
            yield break;
        }

		Input.location.Start(10f, 10f);

		//wait until Service initializes, or 20 seconds.
		int maxWait = 20;
		while (Input.location.status ==  LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		// Service did not initialize within 20 seconds
		if (maxWait < 1) {
			print ("Location initialization timed out");
			yield break;
		} 

		// connection failed to initialize
		if (Input.location.status == LocationServiceStatus.Failed) {
			print ("Unable to determine location");
			yield break;
		} else if (Input.location.status == LocationServiceStatus.Running) {
			//access granted and location values can be retireved
			Debug.Log ("Location Services report running successfully");
			yield return Input.location.lastData;
			print ("location is: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
		}

	}

    public float CalculateCoreFalloffOdds(float max_odds)
    {
        float odds = 0.0f;

        if (GameManager.instance.daysSurvived < GameManager.DaysUntilOddsFlat)
        {
            DateTime now = System.DateTime.Now;
            double days_alive = (now - GameManager.instance.timeCharacterStarted).TotalDays;

            int exponent = 8;
            //float max_percentage = 0.5f; //this starts us at 50/50 odds.
            double full_value = Mathf.Pow(GameManager.DaysUntilOddsFlat, exponent) / max_odds;

            float inverse_day_value = (float)(GameManager.DaysUntilOddsFlat - days_alive);
            float current_value = (float)(Mathf.Pow(inverse_day_value, exponent) / full_value);
            Debug.Log("calculating players odds to be at " + current_value +" based on a max % of: "+ max_odds+" and the days alive count at: "+days_alive);
            odds = current_value;
        }

        return odds; //value is represeneted 0.0f-1.0f as 0%-100%
    }

}
