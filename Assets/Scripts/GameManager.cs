using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using LitJson;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public int survivorsActive, totalSurvivors, daysSurvived, supply, reportedSupply, reportedWater, reportedFood, reportedTotalSurvivor, reportedActiveSurvivor, playerCurrentHealth, zombiesToFight, shivCount, clubCount, gunCount, foodCount, waterCount, mealCount;
	public DateTime timeCharacterStarted;
	public float homebaseLat, homebaseLong;
	public bool[] buildingToggleStatusArray;
	public string weaponEquipped;

	private Scene activeScene;
	private string activeBldg;
	private int shivDurability, clubDurability;

	public string userId;
	public string userFirstName;
	public string userLastName;

	private string startNewCharURL = "http://localhost/ARGZ_SERVER/StartNewCharacter.php";
	private string resumeCharacterUrl = "http://localhost/ARGZ_SERVER/ResumeCharacter.php";
	private string updateAllStatsURL = "http://localhost/ARGZ_SERVER/UpdateAllPlayerStats.php";

	void Awake () {
		MakeSingleton();
		StartCoroutine (StartLocationServices());


		buildingToggleStatusArray = new bool[4];
		ResetAllBuildings();
		weaponEquipped = "shiv";
	}

	void OnLevelWasLoaded () {
		//this is a catch all to slave the long term memory to the active GameManager.instance object- each load will update long term memory.


		activeScene = SceneManager.GetActiveScene();
		if (activeScene.name.ToString() == "02b Combat Level") {
			CombatManager combatManager = FindObjectOfType<CombatManager>();
			combatManager.zombiesToWin = zombiesToFight;
		} else if (activeScene.name.ToString() == "02a Map Level"){
			Debug.Log ("Time character started set to: " + timeCharacterStarted);
			
		} else if (activeScene.name.ToString() == "01b Start") {
			InvokeRepeating ( "CheckEatingAndDrinking", 1.0f, 30.0f);
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

//	JsonData CurrentPlayerDataIntoJson () {
//		string[] playerData = new string[] () ;
//
//	}

	public void UpdateAllStatsToGameMemory () {

		StartCoroutine(UpdateGameManagerToGameServer());
	/*
		GamePreferences.SetShivCount(shivCount);
		GamePreferences.SetClubCount(clubCount);
		GamePreferences.SetGunCount(gunCount);
		GamePreferences.SetShivDurability(shivDurability);
		GamePreferences.SetClubDurability(clubDurability);
		GamePreferences.SetSupply(supply);
		GamePreferences.SetTotalSurvivors (totalSurvivors);
		GamePreferences.SetActiveSurvivors (survivorsActive);
		GamePreferences.SetWaterCount (waterCount);
		GamePreferences.SetFoodCount (foodCount);
		GamePreferences.SetMealsCount (mealCount);
		SetPublicPlayerHealth (this.playerCurrentHealth);
		GamePreferences.SetHomebaseLattitude (homebaseLat);
		GamePreferences.SetHomebaseLongitude (homebaseLong);
		*/
		//removed to update the server
	}

	IEnumerator UpdateGameManagerToGameServer() {
//		JsonData playerJsonData = CurrentPlayerDataIntoJson();
//		String playerJsonString = File.ReadAllText(Application.dataPath + "/Resources/Player.json");


		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId );
		form.AddField("first_name", GameManager.instance.userFirstName);
		form.AddField("last_name", GameManager.instance.userLastName);
		form.AddField("total_survivors", GameManager.instance.totalSurvivors);
		form.AddField("active_survivors", GameManager.instance.survivorsActive);
		form.AddField("player_current_health", GameManager.instance.playerCurrentHealth);
		form.AddField("supply", GameManager.instance.supply);
		form.AddField("food", GameManager.instance.foodCount);
		form.AddField("water", GameManager.instance.waterCount);
		form.AddField("meals", GameManager.instance.mealCount);
		form.AddField("knife_count", GameManager.instance.shivCount);
		form.AddField("club_count", GameManager.instance.clubCount);
		form.AddField("gun_count", GameManager.instance.gunCount);
		form.AddField("knife_durability", GameManager.instance.shivDurability);
		form.AddField("club_durability", GameManager.instance.clubDurability);
		form.AddField("home_lat", GameManager.instance.homebaseLat.ToString());
		form.AddField("home_lon", GameManager.instance.homebaseLong.ToString());
		form.AddField("char_created_DateTime", GameManager.instance.timeCharacterStarted.ToString());

		WWW www = new WWW(updateAllStatsURL, form);
		yield return www;

		if (www.error == null) {
			
			Debug.Log ("Server successfully updated " + www.text);

			yield break;
		} else {
			Debug.Log("WWW error "+ www.error);
		}
	}

	IEnumerator NewCharacterUpdateServer () {
		
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId );
		form.AddField("first_name", GameManager.instance.userFirstName);
		form.AddField("last_name", GameManager.instance.userLastName);
		form.AddField("total_survivors", GameManager.instance.totalSurvivors);
		form.AddField("active_survivors", GameManager.instance.survivorsActive);
		form.AddField("supply", GameManager.instance.supply);
		form.AddField("food", GameManager.instance.foodCount);
		form.AddField("water", GameManager.instance.waterCount);
		form.AddField("knife_count", GameManager.instance.shivCount);
		form.AddField("club_count", GameManager.instance.clubCount);
		form.AddField("gun_count", GameManager.instance.gunCount);
		form.AddField("knife_durability", GameManager.instance.shivDurability);
		form.AddField("club_durability", GameManager.instance.clubDurability);

		WWW www = new WWW(startNewCharURL, form);
		yield return www;

		if (www.error == null) {
			
			Debug.Log ("New character successfully started on the server" + www.text);
			SceneManager.LoadScene("02a Map Level");
			yield break;
		} else {
			Debug.Log("WWW error "+ www.error);
		}
	}

	IEnumerator FetchResumePlayerData () {

		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);

		WWW www = new WWW(resumeCharacterUrl, form);
		yield return www;

		if (www.error == null) {

			Debug.Log ("resuming character, server returned raw json string of: " + www.text);

			//write the raw WWW return to a .json file 
			File.WriteAllText(Application.dataPath + "/Resources/Player.json", www.text.ToString());

			//read that text out into a string object, and map that to a json object
			string playerJsonString = File.ReadAllText(Application.dataPath + "/Resources/Player.json");
			JsonData playerJson = JsonMapper.ToObject(playerJsonString);


			//update the GameManager.instance with all dataum
			GameManager.instance.userFirstName = playerJson["first_name"].ToString() ;
			GameManager.instance.userLastName = playerJson["last_name"].ToString();
			int totsuv = Convert.ToInt32(playerJson["total_survivors"].ToString());
			GameManager.instance.totalSurvivors = totsuv;
			int suvAct = Convert.ToInt32(playerJson["active_survivors"].ToString());
			GameManager.instance.survivorsActive = suvAct;
			GameManager.instance.timeCharacterStarted = Convert.ToDateTime(playerJson["char_created_DateTime"].ToString());
			int currHealth = Convert.ToInt32(playerJson["last_player_current_health"].ToString());
			GameManager.instance.playerCurrentHealth = currHealth;
			int sup = Convert.ToInt32(playerJson["supply"].ToString());
			GameManager.instance.supply = sup;
			int wat = Convert.ToInt32(playerJson["water"].ToString());
			GameManager.instance.waterCount = wat;
			int fud = Convert.ToInt32(playerJson["food"].ToString());
			GameManager.instance.foodCount = fud;
			int meal = Convert.ToInt32(playerJson["meals"].ToString());
			GameManager.instance.mealCount = meal;
			int knifeC = Convert.ToInt32(playerJson["knife_count"].ToString());
			GameManager.instance.shivCount = knifeC;
			int clubC = Convert.ToInt32(playerJson["club_count"].ToString());
			GameManager.instance.clubCount = clubC;
			int gunC = Convert.ToInt32(playerJson["gun_count"].ToString());
			GameManager.instance.gunCount = gunC;
			int knifeD = Convert.ToInt32(playerJson["knife_durability"].ToString());
			GameManager.instance.shivDurability = knifeD;
			int clubD = Convert.ToInt32(playerJson["club_durability"].ToString());
			GameManager.instance.clubDurability = clubD;
			float homeLat = (float)Convert.ToDouble(playerJson["homebase_lat"].ToString());
			GameManager.instance.homebaseLat = homeLat;
			float homeLon = (float)Convert.ToDouble(playerJson["homebase_lon"].ToString());
			GameManager.instance.homebaseLong = homeLon;

			//once the GameManager.instance is updated- you're clear to load the map level.
			SceneManager.LoadScene("02a Map Level");
			yield break;
		} else {
			Debug.Log ("WWW error" + www.error);
		}

	}


	
	public void SetDaysSurvived () {
		DateTime now = System.DateTime.Now;
		Double days = (now - timeCharacterStarted).TotalDays;
		daysSurvived = Convert.ToInt32(days);
		Debug.Log ("The SetDaysSurvived function has returned: " + days + " Days since character created");
	}

	public void StartNewCharacter () {
		//Record the date and time the character is created- will be compared to get Days alive later.
		timeCharacterStarted = System.DateTime.Now;


		//roll a random number of survivors left alive and set both active and alive to that number.
		shivCount = UnityEngine.Random.Range (2,12);
		clubCount = UnityEngine.Random.Range (4,7);
		gunCount = UnityEngine.Random.Range (10,50);
		shivDurability = 50;
		clubDurability = 25;
		int survivors = UnityEngine.Random.Range(2, 8);
		survivorsActive = UnityEngine.Random.Range(1,survivors);
		totalSurvivors = survivors;
		supply = UnityEngine.Random.Range(20, 70);
		waterCount = UnityEngine.Random.Range(10, 20);
		foodCount = UnityEngine.Random.Range(15, 30);
		mealCount = 0;
		playerCurrentHealth = 100;

		StartCoroutine (NewCharacterUpdateServer());


		//pass all the rolled info to the gamePreferences - aka permenent memory
//		GamePreferences.SetShivCount(shivCount);
//		GamePreferences.SetClubCount(clubCount);
//		GamePreferences.SetGunCount(gunCount);
//		GamePreferences.SetShivDurability(shivDurability);
//		GamePreferences.SetClubDurability(clubDurability);
//		GamePreferences.SetSupply(supply);
//		GamePreferences.SetTotalSurvivors (totalSurvivors);
//		GamePreferences.SetActiveSurvivors (survivorsActive);
//		GamePreferences.SetWaterCount (waterCount);
//		GamePreferences.SetFoodCount (foodCount);
		//this.SetPublicPlayerHealth (100);
		//Debug.Log ("GameManager started a new character- food / water: " + foodCount +" / "+ waterCount );


		Debug.Log ("Character started at: " + timeCharacterStarted);
	}

	public void ResumeCharacter () {

		StartCoroutine (FetchResumePlayerData());

		//The coroutine should handle fetching and loading all the game data from server now.  No data should be stored in preferences anymore.

//		survivorsActive = GamePreferences.GetActiveSurvivors();
//		totalSurvivors = GamePreferences.GetTotalSurvivors();
//		if (totalSurvivors > survivorsActive) {
//			survivorsActive = totalSurvivors;
//			// if there are more active than total, make the active = the total. temp idiot check.
//		}
//
//		waterCount = GamePreferences.GetWaterCount();
//		foodCount = GamePreferences.GetFoodCount();
//		supply = GamePreferences.GetSupply();
//		timeCharacterStarted = Convert.ToDateTime(GamePreferences.GetDayTimeCharacterCreated());
//		SetDaysSurvived();
//		playerCurrentHealth = GamePreferences.GetLastPlayerCurrentHealth();
//
//		shivCount = GamePreferences.GetShivCount();
//		clubCount = GamePreferences.GetClubCount();
//		gunCount = GamePreferences.GetGunCount();
//
//		shivDurability = GamePreferences.GetShivDurability();
//		clubDurability = GamePreferences.GetClubDurability();

	}

	public void RestartTheGame () {
		StartNewCharacter ();
		SceneManager.LoadScene("02a Map Level");// consider storing these in a public static array that can just name locations, and possibly swap backgrounds.
	}

	public void PaidRestartOfTheGame () {
		int survivors = UnityEngine.Random.Range(4, 8);
		survivorsActive = survivors;
		totalSurvivors = survivors;

		int newSupply = Mathf.RoundToInt(this.supply * 0.75f);
		this.supply = newSupply;
		int newFood = Mathf.RoundToInt(this.foodCount / 2);
		foodCount = newFood;
		int newWater = Mathf.RoundToInt(this.waterCount / 2);
		waterCount = newWater;


//		GamePreferences.SetSupply(supply);
//		GamePreferences.SetTotalSurvivors (totalSurvivors);
//		GamePreferences.SetActiveSurvivors (survivorsActive);
//		GamePreferences.SetWaterCount (waterCount);
//		GamePreferences.SetFoodCount (foodCount);
		GameManager.instance.UpdateAllStatsToGameMemory();

		SceneManager.LoadScene("02a Map Level");
	}

	public void SetPublicPlayerHealth (int playerHealth) {
		playerCurrentHealth = playerHealth;

		GameManager.instance.UpdateAllStatsToGameMemory();
//		GamePreferences.SetLastPlayerCurrentHealth(playerHealth);
		//this is for external setting of the permenant game object GameManager.instance
	}

	public void SetHomebaseLocation (float lat, float lon) {
		this.homebaseLat = lat;
		this.homebaseLong = lon;

		UpdateAllStatsToGameMemory();
//		GamePreferences.SetHomebaseLattitude(lat);
//		GamePreferences.SetHomebaseLongitude(lon);
	}

	public void LoadIntoCombat (int zombies, string bldg) {
		activeBldg = bldg;
		zombiesToFight = zombies;
		SceneManager.LoadScene ("02b Combat level");
	}

	public void AddTimePlayed () {
		timeCharacterStarted  = timeCharacterStarted.AddHours(-1.0);
//		GamePreferences.SetDayTimeCharacterCreated (timeCharacterStarted.ToString());

		//right now this will not save to permenant memory- this should be handled in its own function/script- so it's not accidentally changed.
		UpdateAllStatsToGameMemory();
		SetDaysSurvived ();
		Debug.Log ("1 Hour added to time started. New Datetime is: " + timeCharacterStarted.ToString() );
	}


	public void ResetAllBuildings () {
		for (int i = 0 ; i < buildingToggleStatusArray.Length ; i++ ){
			buildingToggleStatusArray[i] = false;
		}
		Building[] arrayOfBuildings = FindObjectsOfType<Building>();
		for (int i = 0; i < arrayOfBuildings.Length; i++) {
			Debug.Log("Sending reactivation message to " + arrayOfBuildings[i].name );
			arrayOfBuildings[i].ReactivateMe();
		}
	}

	public void BuildingIsCleared (int sup, int water, int food, int foundTotalSurvivors, int foundAbleBodiedSurvivors) {
		//local updates for the running game variables
		reportedSupply = sup;
		supply += sup;
		reportedWater = water;
		waterCount += water;
		reportedFood = food;
		foodCount += food;
		reportedTotalSurvivor = foundTotalSurvivors;
		reportedActiveSurvivor = foundAbleBodiedSurvivors;
		totalSurvivors += foundTotalSurvivors;
		survivorsActive += foundTotalSurvivors;

		//this updates the long term memory, and will need to be changed to update the PHP server.  This is essentially saving the check-in.
//		GamePreferences.SetFoodCount(foodCount);
//		GamePreferences.SetWaterCount(waterCount);
//		GamePreferences.SetSupply(supply);
//		GamePreferences.SetTotalSurvivors(totalSurvivors);
//		GamePreferences.SetActiveSurvivors(survivorsActive);
		GameManager.instance.UpdateAllStatsToGameMemory();

		SetClearedBuilding();
	}

	private void SetClearedBuilding () {
		if (activeBldg == "Building01") {
			buildingToggleStatusArray[0]=true;
		} else if (activeBldg == "Building02") {
			buildingToggleStatusArray[1]=true;
		} else if (activeBldg == "Building03") {
			buildingToggleStatusArray[2]=true;
		} else if (activeBldg == "Building04") {
			buildingToggleStatusArray[3]=true;
		}
	}


	public void PlayerAttemptingPurchaseFullHealth () {
		if (playerCurrentHealth < 100) {
			if (supply >= 20) {
				supply -= 20;
				SetPublicPlayerHealth(100);
				//updating server side stats is called in the public player health function.

				MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
				mapLevelManager.UpdateTheUI();
			} else {
				Debug.Log ("Player does not have enough supply to make the purchase");
			}
		} else {
			Debug.Log ("Player Health already full");
		}
	}

	public void PlayerAttemptingPurchaseNewSurvivor () {
		if (supply >= 50) {
			if (survivorsActive < totalSurvivors) {
				//subtract the cost.
				supply -= 50;
//				GamePreferences.SetSupply(supply);

				//add the survivor
				survivorsActive ++;
//				GamePreferences.SetActiveSurvivors(survivorsActive);
//				GamePreferences.SetTotalSurvivors(totalSurvivors);
				GameManager.instance.UpdateAllStatsToGameMemory();

				//this can only be called from inventory on map level- so get that lvl manager, and update the UI elements.
				MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
				mapLevelManager.UpdateTheUI();
			} else {
				Debug.Log("There are no inactive players to train");
			}
		} else {
			Debug.Log ("Player does not have enough supply to make the purchase");
		}
	}

	public void PlayerAttemptingPurchaseShiv () {
		if ( supply >= 5 ) {

			supply -= 5;
//			GamePreferences.SetSupply(this.supply);

			shivCount ++;
//			GamePreferences.SetShivCount(shivCount);

			GameManager.instance.UpdateAllStatsToGameMemory();

			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
			mapLevelManager.UpdateTheUI();
		}
	}

	public void PlayerAttemtpingPurchaseClub () {
		if (supply >= 15) {
			supply -= 15;
//			GamePreferences.SetSupply(supply);

			clubCount ++;
//			GamePreferences.SetClubCount(clubCount);

			GameManager.instance.UpdateAllStatsToGameMemory();

			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
			mapLevelManager.UpdateTheUI();
		}
	}

	public void PlayterAttemtptingPurchaseGun () {
		if (supply >= 25) {
			supply -= 25;
//			GamePreferences.SetSupply(supply);

			gunCount += 10;
//			GamePreferences.SetGunCount(gunCount);

			GameManager.instance.UpdateAllStatsToGameMemory();

			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
			mapLevelManager.UpdateTheUI();
		}
	}

	public void ProcessDurability () {
		if (weaponEquipped == "shiv") {
			shivDurability --;

			if (shivDurability <= 0) {
				shivCount --;
				shivDurability = 50;
//				GamePreferences.SetShivCount (shivCount);
//				GamePreferences.SetShivDurability (shivDurability);
			} else {
//				GamePreferences.SetShivDurability (shivDurability);
			}

			Debug.Log ("Shiv has successfully processed durability, it's now got " + shivDurability + " durability, and total shivs: " +shivCount);
		} else if (weaponEquipped == "club") {
			clubDurability --;

			if (clubDurability <= 0) {
				clubCount --;
				clubDurability = 50;
//				GamePreferences.SetClubCount (clubCount);
//				GamePreferences.SetClubDurability (clubDurability);
			} else {
//				GamePreferences.SetClubDurability (clubDurability);
			}

			Debug.Log ("Club has successfully processed durability, it's now got " + clubDurability + " durability, and total clubs: " + clubCount);
		} else if (weaponEquipped == "gun"){
			gunCount --;

//			GamePreferences.SetGunCount(gunCount);

			Debug.Log ("Gun has successfully processed ammo spent, player now has " + gunCount +" amunition left");
		} else {
			Debug.Log ("Durability function failed to execute");
		}
	}

	public void CheckEatingAndDrinking () {
		StartCoroutine ( UpdatePlayersEatingAndDrinking () );
	}

	// the idea with this coroutine is to invokerepeating on awake, every 15-30, and execute the 'meal' as soon as it's time.
	IEnumerator UpdatePlayersEatingAndDrinking () {
		//Debug.Log ("Checking to update food / Water / Meal counts");
		
		//if we have fewer meals than expected.
		DateTime now = System.DateTime.Now;
		Double days = (now - timeCharacterStarted).TotalDays;
		int expected = (int)Mathf.Floor( (float)days * 2 );

		if (mealCount < expected) {
			mealCount ++;
			foodCount = foodCount - totalSurvivors;
			waterCount = waterCount - totalSurvivors;

			Debug.Log ("a meal has been processed. Total meals eaten: " + mealCount + " Food Count: " + foodCount + " Water Count: "+ waterCount);

//			GamePreferences.SetWaterCount(waterCount);
//			GamePreferences.SetFoodCount(foodCount);
//			GamePreferences.SetMealsCount(mealCount);

			UpdateAllStatsToGameMemory();

			//we need to restart the coroutine to ensure that we don't need to process multiple meals.  
			if (mealCount != expected) {
				StartCoroutine( UpdatePlayersEatingAndDrinking () );
			} else {
				yield break;
			}
		} else {
			Debug.Log ("not time to eat or drink yet... but coroutine is checking @ days played: " + days + " and an expected meal count of " + expected);
			yield break;
		}
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
}
