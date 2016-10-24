using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MissionCombatSimulator : MonoBehaviour {

	public List<SurvivorPlayCard> missionPlayCardList;

	public string building_id, building_name;
	public float travel_time_minutes;
	public int ammo_used, zombies_killed, supply_earned, water_earned, food_earned;
	public bool surv1Dead, surv2Dead, surv3Dead, surv4Dead, surv5Dead, survivor_found;

	private string sendMissionURL = GameManager.serverURL+"/StartNewMission.php";

	public void SimulateCombat (int zombieCount, float distanceToBuilding, string bldg_id, string bldg_name) {
		building_id = bldg_id;
		building_name = bldg_name;
		zombies_killed = zombieCount;
		//calculate the travel time and store it for sending to the server.
		float movement_speed = 1.5f;//speed in meters/second
		float time_in_seconds = distanceToBuilding/movement_speed; //distance is in meters
		TimeSpan duration = TimeSpan.FromSeconds(time_in_seconds);
		travel_time_minutes = (float)duration.TotalMinutes*2; //doubled for there AND back 
		float time_on_target = 0.12f * zombieCount; //10sec for each zombie
		travel_time_minutes += time_on_target; //add time at target

		//this is the encapsulating loop that should exit to start our coroutine to send the mission and results to the server.
		while (zombieCount > 0) {
			//execute player attacks, and increment zombies down
			int totalDmg = 0;
			foreach(SurvivorPlayCard survivor in missionPlayCardList) {
				totalDmg += CalculateSurvivorDamage(survivor);
				if (totalDmg >= 30) {
					zombieCount--;
					totalDmg -= 30;
				}
			}
			//execute zombie attacks, and store stamina loss or chance for bites.
			for (int i=0; i < 5; i++) {
				//if there are less than 5 zombies left, only calculate attacks for the amount remaining.
				if (i+1 >zombieCount) {
					break;
				} else {
					//increment stamina down
					missionPlayCardList[i].survivor.curStamina -= 5;

					//check for having bit the player
					float odds = 2.5f;
					if (missionPlayCardList[i].survivor.curStamina < 1) {
						//if player is exhausted, 2.5x the odds to get bitten
						odds = odds*2.5f;
					}
					float roll = UnityEngine.Random.Range(0.0f, 100.0f);
					if (roll < odds) {
						//TARGET HAS BEEN BITTEN
						Debug.Log("Survivor "+missionPlayCardList[i].survivor.name+" has been bitten!!!");
						if (i == 0) {
							surv1Dead = true;
						} else if (i==1) {
							surv2Dead = true;
						} else if (i==2) {
							surv3Dead = true;
						} else if (i==3){
							surv4Dead = true;
						} else if (i==4){
							surv5Dead = true;
						}
					}
				}
			}
		}

		//once the data has been populated, start the coroutine to send the results to the server
		CalculateAndStoreRewards();
		StartCoroutine(SendMissionToServer());
	}

	private IEnumerator SendMissionToServer() {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		form.AddField("building_id", building_id);
		form.AddField("building_name", building_name);
		form.AddField("survivor1_id", missionPlayCardList[0].survivor.survivor_id);
		form.AddField("survivor1_curr_stam", missionPlayCardList[0].survivor.curStamina);
		if (surv1Dead == true ) {
			form.AddField("survivor1_dead", 1);
		} else {
			form.AddField("survivor1_dead", 0);
		}
		form.AddField("survivor2_id", missionPlayCardList[1].survivor.survivor_id);
		form.AddField("survivor2_curr_stam", missionPlayCardList[1].survivor.curStamina);
		if (surv2Dead == true ) {
			form.AddField("survivor2_dead", 1);
		} else {
			form.AddField("survivor2_dead", 0);
		}
		form.AddField("survivor3_id", missionPlayCardList[2].survivor.survivor_id);
		form.AddField("survivor3_curr_stam", missionPlayCardList[2].survivor.curStamina);
		if (surv3Dead == true ) {
			form.AddField("survivor3_dead", 1);
		} else {
			form.AddField("survivor3_dead", 0);
		}
		form.AddField("survivor4_id", missionPlayCardList[3].survivor.survivor_id);
		form.AddField("survivor4_curr_stam", missionPlayCardList[3].survivor.curStamina);
		if (surv4Dead == true ) {
			form.AddField("survivor4_dead", 1);
		} else {
			form.AddField("survivor4_dead", 0);
		}
		form.AddField("survivor5_id", missionPlayCardList[4].survivor.survivor_id);
		form.AddField("survivor5_curr_stam", missionPlayCardList[4].survivor.curStamina);
		if (surv5Dead == true ) {
			form.AddField("survivor5_dead", 1);
		} else {
			form.AddField("survivor5_dead", 0);
		}
		form.AddField("supply_found", supply_earned);
		form.AddField("water_found", water_earned);
		form.AddField("food_found", food_earned);
		form.AddField("ammo_used", ammo_used);
		form.AddField("duration", travel_time_minutes.ToString());
		if (survivor_found) {
			form.AddField("survivor_found", 1);
		} else {
			form.AddField("survivor_found", 0);
		}

		WWW www = new WWW(sendMissionURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			//if the mission has been created- turn off the panels, and call for a UI update.
			MapLevelManager myMapLevelMgr = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
			myMapLevelMgr.missionStartConfirmationPanel.gameObject.SetActive(false);
			myMapLevelMgr.buildingPanel.gameObject.SetActive(false);
			GameManager.instance.updateWeaponAndSurvivorMapLevelUI=true;
			//GameManager.instance.ResumeCharacter();
			StartCoroutine(GameManager.instance.LoadAllGameData());
		} else {
			Debug.Log(www.error);
		}

	}

	private int CalculateSurvivorDamage (SurvivorPlayCard myPlayCard) {
		int myDmg = 0;
		myDmg += myPlayCard.survivor.baseAttack;
		if (myPlayCard.survivor.weaponEquipped != null) {
			//get weapon
			BaseWeapon myWeapon = myPlayCard.survivor.weaponEquipped.GetComponent<BaseWeapon>();
			if (myWeapon.weaponType != BaseWeapon.WeaponType.GUN) {
				//add weapon dmg
				myDmg += (myWeapon.base_dmg+ UnityEngine.Random.Range(0, myWeapon.modifier));
				//subtract stamina cost
				myPlayCard.survivor.curStamina = myPlayCard.survivor.curStamina - myWeapon.stam_cost;
			} else if (GameManager.instance.ammo < 1) {
				//add weapon dmg
				myDmg += (myWeapon.base_dmg+ UnityEngine.Random.Range(0, myWeapon.modifier-5));
				//subtract stamina cost
				myPlayCard.survivor.curStamina = myPlayCard.survivor.curStamina - myWeapon.stam_cost;
			} else {
				//add the weapon dmg, and subtract 1 ammo.
				ammo_used ++;
				GameManager.instance.ammo--;
				//add weapon dmg
				myDmg += (myWeapon.base_dmg+ UnityEngine.Random.Range(0, myWeapon.modifier));
				//subtract stamina cost
				myPlayCard.survivor.curStamina = myPlayCard.survivor.curStamina - myWeapon.stam_cost;
			}
			myWeapon.durability--;
			if (myWeapon.durability < 1) {
				Destroy(myWeapon.gameObject);
			}
		} else {
			//unarmed attack
			int multiplier = UnityEngine.Random.Range(0, 7);
			myPlayCard.survivor.curStamina -= 3;
			myDmg += multiplier;
		}

		//reduce attack for exhausted survivors
		if (myPlayCard.survivor.curStamina < 1) {
			myDmg = Mathf.RoundToInt(myDmg/2);
		}

		return myDmg;
	}

	private void CalculateAndStoreRewards() {
		CalculateSupplyEarned();
		CalculateFoodFound();
		CalculateWaterFound();
		CalculateSurvivorFound();
	}

	void CalculateSupplyEarned () {
		//Debug.Log ("Calculating the sum of supply earned from " + zombiesKilled + " zombies killed.");
		int sum = 0;
		for (int i = 0; i < zombies_killed; i++) {
			int num = UnityEngine.Random.Range(0, 5);
			sum += num;
			//Debug.Log ("adding "+ num +" supply to the list");
		}
		//Debug.Log ("calculating total supply earned yields: " + sum);
		supply_earned = sum;
	}

	void CalculateWaterFound () {
		float oddsToFind = 50.0f;
		int sum = 0;

		float roll = UnityEngine.Random.Range(0.0f, 100.0f);

		for (int i = 0; i < zombies_killed; i++) {
				int amount = (int)UnityEngine.Random.Range( 1 , 4 );
				sum += amount;
		}

		// this is so that you can find nothing
		if (roll <= oddsToFind) {
			sum = 0;
		}

		water_earned = sum;
	}

	void CalculateFoodFound () {
		float oddsToFind = 50.0f;
		int sum = 0;

		float roll = UnityEngine.Random.Range(0.0f, 100.0f);

		for (int i = 0; i < zombies_killed; i++) {
				int amount = (int)UnityEngine.Random.Range( 1 , 4 );
				sum += amount;
		}

		// this is so that you can find nothing
		if (roll <= oddsToFind) {
			sum = 0;
		}

		food_earned = sum;
	}

	void CalculateSurvivorFound () {
		float odds =0.0f;

		if (GameManager.instance.daysSurvived < GameManager.DaysUntilOddsFlat) {
			DateTime now = System.DateTime.Now;
			double days_alive = (now-GameManager.instance.timeCharacterStarted).TotalDays;

			int exponent = 10;
			float max_percentage = 0.5f; //this starts us at 50/50 odds.
			double full_value = Mathf.Pow(GameManager.DaysUntilOddsFlat, exponent)/ max_percentage;

			float inverse_day_value = (float)(GameManager.DaysUntilOddsFlat - days_alive);
			float current_value = (float)(Mathf.Pow(inverse_day_value, exponent)/full_value);
			Debug.Log("calculating players odds to be at "+current_value);
			odds = current_value;
		}

		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
		if (roll < odds) {
			Debug.Log("survivor found!");
			survivor_found = true;
		}else{
			Debug.Log("no survivors found");
			survivor_found = false;
		}
	}
}
