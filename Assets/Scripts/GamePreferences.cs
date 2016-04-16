using UnityEngine;
using System.Collections;

public static class GamePreferences {

	public static string TotalSurvivors = "TotalSurvivors";
	public static string ActiveSurvivors = "ActiveSurvivors";
	public static string DayTimeCharacterCreated = "DayTimeCharacterCreated";
	public static string Supply = "Supply";
	public static string HomebaseLattitude = "HomebaseLattitude";
	public static string HomebaseLongitude = "HomebaseLongitude";
	public static string PlayerHealthInMemory = "LastPlayerCurrentHealth";


	public static int GetTotalSurvivors () {
		return PlayerPrefs.GetInt(GamePreferences.TotalSurvivors);
	}

	public static void SetTotalSurvivors (int survivors) {
		PlayerPrefs.SetInt (GamePreferences.TotalSurvivors, survivors);
	}

	public static int GetActiveSurvivors () {
		return PlayerPrefs.GetInt(GamePreferences.ActiveSurvivors);
	}

	public static void SetActiveSurvivors (int survivorsActive) {
		PlayerPrefs.SetInt(GamePreferences.ActiveSurvivors, survivorsActive);
	}

	public static string GetDayTimeCharacterCreated () {
		return PlayerPrefs.GetString(GamePreferences.DayTimeCharacterCreated);
	}

	public static void SetDayTimeCharacterCreated (string DayTime) {
		PlayerPrefs.SetString(GamePreferences.DayTimeCharacterCreated, DayTime);
	}

	public static float GetHomebaseLattitude () {
		return PlayerPrefs.GetFloat(HomebaseLattitude);
	}

	public static void SetHomebaseLattitude (float lat) {
		PlayerPrefs.SetFloat(HomebaseLattitude, lat);
	}

	public static float GetHomebaseLongitude () {
		return PlayerPrefs.GetFloat(HomebaseLongitude);
	}

	public static void SetHomebaseLongitude (float lon) {
		PlayerPrefs.SetFloat(HomebaseLongitude, lon);
	}

	public static int GetSupply () {
		return PlayerPrefs.GetInt(Supply);
	}

	public static void SetSupply (int supply) {
		PlayerPrefs.SetInt(Supply, supply);
	}

	public static void SetLastPlayerCurrentHealth (int health){
		PlayerPrefs.SetInt(PlayerHealthInMemory, health);
	}

	public static int GetLastPlayerCurrentHealth () {
		return PlayerPrefs.GetInt(PlayerHealthInMemory);
	}
}//GamePreferences - This handles all the data that is stored in local memory. AKA offline.

///this is the script that should probbaly be turned into the web interface so all this small data can be stored and recieved, as well as processed offline.
