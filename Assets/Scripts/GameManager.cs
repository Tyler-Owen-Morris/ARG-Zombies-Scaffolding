using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public int survivorsActive, totalSurvivors, daysSurvived, supply, reportedSupply, playerCurrentHealth, zombiesToFight;
	public DateTime timeCharacterStarted;
	public float homebaseLat, homebaseLong;
	public bool[] buildingToggleStatusArray;

	private Scene activeScene;
	private string activeBldg;

	void Awake () {
		MakeSingleton();
		StartCoroutine (StartLocationServices());

		buildingToggleStatusArray = new bool[4];
		ResetAllBuildings();
	}

	void OnLevelWasLoaded () {
		activeScene = SceneManager.GetActiveScene();
		if (activeScene.name.ToString() == "02b Combat Level") {
			CombatManager combatManager = FindObjectOfType<CombatManager>();
			combatManager.zombiesToWin = zombiesToFight;
		} else if (activeScene.name.ToString() == "02a Map Level"){
			Debug.Log ("Time character started set to: " + timeCharacterStarted);
			
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
	
	public void SetDaysSurvived () {
		DateTime now = System.DateTime.Now;
		Double days = (now - timeCharacterStarted).TotalDays;
		daysSurvived = Convert.ToInt32(days);
		Debug.Log ("The SetDaysSurvived function has returned: " + days + " Days since character created");
	}

	public void StartNewCharacter () {
		//Record the date and time the character is created- will be compared to get Days alive later.
		timeCharacterStarted = System.DateTime.Now;
		GamePreferences.SetDayTimeCharacterCreated(timeCharacterStarted.ToString());


		//roll a random number of survivors left alive and set both active and alive to that number.
		int survivors = UnityEngine.Random.Range(2, 8);
		survivorsActive = survivors;
		totalSurvivors = survivors;
		supply = UnityEngine.Random.Range(1, 50);
		GamePreferences.SetSupply(supply);
		GamePreferences.SetTotalSurvivors (totalSurvivors);
		GamePreferences.SetActiveSurvivors (survivorsActive);
		this.SetPublicPlayerHealth (100);
		Debug.Log ("Character started at: " + timeCharacterStarted);
	}

	public void ResumeCharacter () {

		survivorsActive = GamePreferences.GetActiveSurvivors();
		totalSurvivors = GamePreferences.GetTotalSurvivors();
		if (totalSurvivors > survivorsActive) {
			survivorsActive = totalSurvivors;
			// if there are more active than total, make the active = the total. temp idiot check.
		}
		supply = GamePreferences.GetSupply();
		timeCharacterStarted = Convert.ToDateTime(GamePreferences.GetDayTimeCharacterCreated());
		SetDaysSurvived();
		playerCurrentHealth = GamePreferences.GetLastPlayerCurrentHealth();

	}

	public void RestartTheGame () {
		StartNewCharacter ();
		SceneManager.LoadScene("01b Start");// consider storing these in a public static array that can just name locations, and possibly swap backgrounds.
	}

	public void PaidRestartOfTheGame () {
		int survivors = UnityEngine.Random.Range(4, 8);
		survivorsActive = survivors;
		totalSurvivors = survivors;
		GamePreferences.SetTotalSurvivors (totalSurvivors);
		GamePreferences.SetActiveSurvivors (survivorsActive);
		int newSupply = Mathf.RoundToInt(this.supply * 0.75f);
		this.supply = newSupply;
		GamePreferences.SetSupply(newSupply);
		SceneManager.LoadScene("01b Start");
	}

	public void SetPublicPlayerHealth (int playerHealth) {
		playerCurrentHealth = playerHealth;
		GamePreferences.SetLastPlayerCurrentHealth(playerHealth);
		//this is for external setting of the permenant game object GameManager.instance
	}

	public void SetHomebaseLocation (float lat, float lon) {
		homebaseLat = lat;
		homebaseLong = lon;

		GamePreferences.SetHomebaseLattitude(lat);
		GamePreferences.SetHomebaseLongitude(lon);
	}

	public void LoadIntoCombat (int zombies, string bldg) {
		activeBldg = bldg;
		zombiesToFight = zombies;
		SceneManager.LoadScene ("02b Combat level");
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

	public void BuildingIsCleared (int sup) {
		reportedSupply = sup;
		supply += sup;
		GamePreferences.SetSupply(supply); 
		SetClearedBuilding();
		// should possibly also build a list or an array here with the names of buildings cleared and use 01-04 to store states.
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
		if (supply >= 20) {
			supply -= 20;
			GamePreferences.SetSupply(supply);
			SetPublicPlayerHealth(100);

			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
			mapLevelManager.UpdateTheUI();
		} else {
			Debug.Log ("Player does not have enough supply to make the purchase");
		}
	}

	public void PlayerAttemptingPurchaseNewSurvivor () {
		if (supply >= 50) {

			//subtract the cost.
			supply -= 50;
			GamePreferences.SetSupply(supply);

			//add the survivor
			survivorsActive ++;
			totalSurvivors ++;
			GamePreferences.SetActiveSurvivors(survivorsActive);
			GamePreferences.SetTotalSurvivors(totalSurvivors);

			//this can only be called from inventory on map level- so get that lvl manager, and update the UI elements.
			MapLevelManager mapLevelManager = FindObjectOfType<MapLevelManager>();  //only called from map level manager, and passed 
			mapLevelManager.UpdateTheUI();
		} else {
			Debug.Log ("Player does not have enough supply to make the purchase");
		}
	}



	IEnumerator StartLocationServices () {
		Input.location.Start();

		//wait until Service initializes
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
