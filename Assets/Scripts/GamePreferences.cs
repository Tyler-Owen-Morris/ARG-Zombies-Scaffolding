using UnityEngine;
using System.Collections;

public static class GamePreferences {

	public static string TotalSurvivors = "TotalSurvivors";
	public static string ActiveSurvivors = "ActiveSurvivors";
	public static string DayTimeCharacterCreated = "DayTimeCharacterCreated";
	public static string HomebaseLattitude = "HomebaseLattitude";
	public static string HomebaseLongitude = "HomebaseLongitude";
	public static string PlayerHealthInMemory = "LastPlayerCurrentHealth";

	public static string Supply = "Supply";
	public static string Water = "Water";
	public static string Food = "Food";
	public static string Meals = "Meals";

	public static string ShivCount = "ShivCount";
	public static string ClubCount = "ClubCount";
	public static string GunCount = "GunCount";
	public static string ShivDurability = "ShivDurability";
	public static string ClubDurability = "ClubDurability";


	//this used to be how game data was stored in "preferences" memory between plays. it's commented to find places where it needs to be removed.
	/*
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

	public static void SetShivCount (int count) {
		PlayerPrefs.SetInt(ShivCount, count);
	}

	public static int GetShivCount () {
		return PlayerPrefs.GetInt(ShivCount);
	}

	public static void SetClubCount (int count) {
		PlayerPrefs.SetInt(ClubCount, count);
	}

	public static int GetClubCount () {
		return PlayerPrefs.GetInt(ClubCount);
	}

	public static void SetGunCount (int count) {
		PlayerPrefs.SetInt(GunCount, count);
	}

	public static int GetGunCount () {
		return PlayerPrefs.GetInt(GunCount);
	}

	public static void SetShivDurability (int durabil) {
		PlayerPrefs.SetInt(ShivDurability, durabil);
	}

	public static int GetShivDurability () {
		return PlayerPrefs.GetInt(ShivDurability);
	}

	public static void SetClubDurability (int durabil) {
		PlayerPrefs.SetInt(ClubDurability, durabil);
	}

	public static int GetClubDurability () {
		return PlayerPrefs.GetInt(ClubDurability);
	}

	public static void SetWaterCount (int count) {
		PlayerPrefs.SetInt(Water, count);
	}

	public static int GetWaterCount () {
		return PlayerPrefs.GetInt(Water);
	}

	public static void SetFoodCount (int count) {
		PlayerPrefs.SetInt(Food, count);
	}

	public static int GetFoodCount () {
		return PlayerPrefs.GetInt(Food);
	}

	public static void SetMealsCount (int count) {
		PlayerPrefs.SetInt(Meals, count);
	}

	public static int GetMealsCount () {
		return PlayerPrefs.GetInt(Meals);
	}
	*/

}//GamePreferences - This handles all the data that is stored in local memory. AKA offline.

///this is the script that should probbaly be turned into the web interface so all this small data can be stored and recieved, as well as processed offline.
