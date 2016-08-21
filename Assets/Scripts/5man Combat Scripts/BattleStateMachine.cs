using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleStateMachine : MonoBehaviour {

	//this state machine handles computer turns.
	public enum PerformAction 
	{
		WAIT,
		SELECTACTION,
		BITECASE,
		COMPLETED
	}
	public PerformAction battleState;

	//this state machine handles player input
	public enum PlayerInput
	{
		ACTIVATE,
		WAITING,
		WEAPONCHANGE
	}
	public PlayerInput playerGUI;

	public List<GameObject> survivorTurnList = new List<GameObject>();
	private TurnHandler survivorChoice;

	public List <TurnHandler> TurnList = new List <TurnHandler> ();
	public List <GameObject> survivorList = new List<GameObject>();
	public List<GameObject> zombieList = new List<GameObject>();

	public GameObject playerTarget;
	public GameObject autoAttackToggle, survivorBitPanel;
	public GameObject playerPos1, playerPos2, playerPos3, playerPos4, playerPos5;
	[SerializeField]
	public bool autoAttackIsOn= false;

	public int zombiesKilled = 0;
	public Text zombieCounter, ammmoCounter, survivorBitText;

	private SurvivorStateMachine survivorWithBite;
	private string destroySurvivorURL = "http://www.argzombie.com/ARGZ_SERVER/DestroySurvivor.php";
	private string restoreSurvivorURL = "http://www.argzombie.com/ARGZ_SERVER/RestoreSurvivor.php";

	//private int totalSurvivorsFound = 0;
	void Awake () {
		LoadInSurvivorCardData();
	}

	void Start () {
		battleState = PerformAction.WAIT;
		playerGUI = PlayerInput.ACTIVATE;



		survivorList.AddRange (GameObject.FindGameObjectsWithTag("survivor"));
		zombieList.AddRange (GameObject.FindGameObjectsWithTag("zombie"));
		AdjustForLessThan5Zombies ();

		UpdateUINumbers();
	}

	void LoadInSurvivorCardData () {

		//find the survivor play card with position 5, and put that as player in position 1
		foreach(GameObject survivorGameobject in GameManager.instance.survivorCardList) {
			//load in the card data off of current game object
			SurvivorPlayCard myPlayCard = survivorGameobject.GetComponent<SurvivorPlayCard>();
			//match corresponding players to their combat positions
			if (myPlayCard.team_pos == 5) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos1.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
			} else if (myPlayCard.team_pos == 4) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos2.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
			} else if (myPlayCard.team_pos == 3) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos3.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
			} else if (myPlayCard.team_pos == 2) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos4.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
			} else if (myPlayCard.team_pos == 1) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos5.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
			}
		}

		//if there are less than 5 survivors, remove the gameObjects starting with 5 and working up.
		if (GameManager.instance.survivorCardList.Count < 5) {
			int survivorsToDelete = 5 - GameManager.instance.survivorCardList.Count;
			for (int i = 0; i < survivorsToDelete; i++) {
				if (i == 0) {
					Destroy(playerPos5);
				} else if (i == 1) {
					Destroy(playerPos4);
				} else if (i == 2) {
					Destroy(playerPos3);
				} else if (i == 3) {
					Destroy(playerPos2);
				} else if (i == 4) {
					Debug.Log("Major problem! you have no survivor cards to load into combat with");
				}
			}
		}
	}

	void AdjustForLessThan5Zombies () {
		if (GameManager.instance.zombiesToFight < 5) {
			int removeNum = 5 - GameManager.instance.zombiesToFight;
			for (int i = 0; i < removeNum; i++) {
				zombieList[0].SetActive(false);
				zombieList[0].GetComponent<ZombieStateMachine>().myTypeText.text = "";
				zombieList.RemoveAt(0);
			} 
		}
	}
	
	// Update is called once per frame
	void Update () {

		switch (battleState) {
			case (PerformAction.WAIT):
				if (TurnList.Count > 0) {
					battleState = PerformAction.SELECTACTION;
				} else if (zombieList.Count < 1) {
					// end of the building
					Debug.Log ("End building called");
					GameManager.instance.BuildingIsCleared(CalculateSupplyEarned(), CalculateWaterFound(), CalculateFoodFound(), CalculateSurvivorFound());
					SceneManager.LoadScene ("03a Win");
				}else if (autoAttackIsOn && survivorTurnList.Count > 0) {
					//continue auto attack
					AttackButtonPressed();
				} else if (survivorTurnList.Count < 1 && TurnList.Count < 1) {
					//give NPC turns after Player has expended all their turns. 
					ResetAllTurns();
				} 
			break;
			case (PerformAction.SELECTACTION):
				GameObject performer = GameObject.Find(TurnList[0].attacker);
				if (TurnList [0].type == "zombie") {
					ZombieStateMachine ZSM = performer.GetComponent<ZombieStateMachine>();
					ZSM.target = TurnList[0].TargetsGameObject;
					ZSM.currentState = ZombieStateMachine.TurnState.TAKEACTION;

				}
				if (TurnList [0].type == "survivor") {
					SurvivorStateMachine SSM = performer.GetComponent<SurvivorStateMachine>();
					SSM.plyrTarget = TurnList[0].TargetsGameObject;
					SSM.currentState = SurvivorStateMachine.TurnState.ACTION;
				}
				battleState = PerformAction.COMPLETED;

			break;
			case (PerformAction.BITECASE):

			break;
			case (PerformAction.COMPLETED):

			break;
		}

		switch (playerGUI) {

			case (PlayerInput.ACTIVATE):
				if (survivorTurnList.Count > 0) {
					survivorTurnList [0].transform.FindChild("arrow").gameObject.SetActive(true);
					survivorTurnList [0].GetComponent<SurvivorStateMachine>().isSelected = true;
					playerGUI =  PlayerInput.WAITING;
				}
			break;
			case (PlayerInput.WAITING):
				//idle
			break;
			case (PlayerInput.WEAPONCHANGE):

			break;
//			case (PlayerInput.EXECUTETURN):
//				GameObject performer = GameObject.Find(survivorTurnList[0].name);
//				SurvivorStateMachine SSM = performer.GetComponent<SurvivorStateMachine>();
//				if (playerTarget != null) {
//					SSM.plyrTarget = playerTarget.gameObject; 
//				} else {
//					SSM.currentState = SurvivorStateMachine.TurnState.AUTOCHOOSING;
//				}
//
//			break;

		}
	
	}



	public void CollectAction (TurnHandler myTurn) {
		TurnList.Add (myTurn);
	}

	public void AutoAttackTogglePressed () {
		autoAttackIsOn = autoAttackToggle.GetComponent<Toggle>().isOn;
	}

	void ResetAllTurns () {
		foreach (GameObject zombie in zombieList) {
			ZombieStateMachine ZSM = zombie.GetComponent<ZombieStateMachine>();
			ZSM.currentState = ZombieStateMachine.TurnState.CHOOSEACTION;
		}
		foreach (GameObject survivor in survivorList) {
			SurvivorStateMachine SSM = survivor.GetComponent<SurvivorStateMachine>();
			SSM.currentState = SurvivorStateMachine.TurnState.INITIALIZING;
		}
	}
	public void UpdateUINumbers () {
		zombieCounter.text = GameManager.instance.zombiesToFight.ToString();
		ammmoCounter.text = "Ammo: "+GameManager.instance.ammo.ToString();
	}

	int CalculateSupplyEarned () {
		//Debug.Log ("Calculating the sum of supply earned from " + zombiesKilled + " zombies killed.");
		int sum = 0;
		for (int i = 0; i < zombiesKilled; i++) {
			int num = UnityEngine.Random.Range(0, 8);
			sum += num;
			Debug.Log ("adding "+ num +" supply to the list");
		}
		//Debug.Log ("calculating total supply earned yields: " + sum);
		return sum;
	}

	int CalculateWaterFound () {
		float oddsToFind = 50.0f;
		int sum = 0;

		float roll = UnityEngine.Random.Range(0.0f, 100.0f);

		for (int i = 0; i < zombiesKilled; i++) {
				int amount = (int)UnityEngine.Random.Range( 1 , 4 );
				sum += amount;
		}

		// this is so that you can find nothing
		if (roll <= oddsToFind) {
			sum = 0;
		}

		return sum;
	}

	int CalculateFoodFound () {
		float oddsToFind = 50.0f;
		int sum = 0;

		float roll = UnityEngine.Random.Range(0.0f, 100.0f);

		for (int i = 0; i < zombiesKilled; i++) {
				int amount = (int)UnityEngine.Random.Range( 1 , 4 );
				sum += amount;
		}

		// this is so that you can find nothing
		if (roll <= oddsToFind) {
			sum = 0;
		}

		return sum;
	}

	bool CalculateSurvivorFound () {
		float odds =0.0f;
		if (GameManager.instance.daysSurvived < GameManager.DaysUntilOddsFlat) {
			DateTime now = System.DateTime.Now;
			Double days_alive = (now - GameManager.instance.timeCharacterStarted).TotalDays;
			float max_percent = 60.0f;
			float max_roll = GameManager.FlatOddsToFind + Mathf.Pow(GameManager.DaysUntilOddsFlat, 2);
			float full_roll = max_roll / (max_percent/100);
			float curr_roll = (float)(Mathf.Pow((float)((GameManager.DaysUntilOddsFlat+1) - days_alive), 2) + GameManager.FlatOddsToFind);
			odds = curr_roll/full_roll;
			Debug.Log("Players odds to find a survivor calculated at: "+odds.ToString());

//			float maxValue = Mathf.Pow(GameManager.DaysUntilOddsFlat, 2);
//			float maximumOdds = (maxValue*100)/60; //assuming 60% odds on day0 and near 0 on day29
//			odds = (maxValue- Mathf.Pow(GameManager.instance.daysSurvived, 2))/maximumOdds;
//			Debug.Log("odds of finding a survivor calculated to be: "+ odds);
		} else {
			odds = GameManager.FlatOddsToFind;
		}

		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
		if (roll < odds) {
			Debug.Log("survivor found!");
			return true;
		}else{
			Debug.Log("no survivors found");
			return false;
		}
	}

	/*
	int CalculateFoundTotalSurvivors () {

		float odds = 50.0f;
		int sum = 0;

		float roll = UnityEngine.Random.Range (0.0f, 100.0f);

		if ( roll >= odds) {
			sum += UnityEngine.Random.Range (0, 4);
		}
		totalSurvivorsFound = sum;
		return sum;

	}

	int CalculateActiveSurvivorsFound () {
		
		float oddsOfBeingActive = 55.0f;
		int activeSurvivors = 0;

		if (totalSurvivorsFound <= 0 ) {
			return activeSurvivors;
		} else {
			for (int i = 0; i < totalSurvivorsFound; i++ ) {
				float roll = UnityEngine.Random.Range (0.0f, 100.0f);

				if (roll <= oddsOfBeingActive) {
					activeSurvivors++;
				}
			}
			return activeSurvivors;
		}

	}
	*/

	//this is activated on the GUI button press.
	public void AttackButtonPressed () {
		TurnHandler myAttack = new TurnHandler();
		myAttack.attacker = survivorTurnList[0].name;
		myAttack.type = "survivor";
		myAttack.AttackersGameObject = survivorTurnList[0];
		if (playerTarget != null) {
			myAttack.TargetsGameObject = playerTarget;
		} else {
			//pick a random zombie
			//myAttack.TargetsGameObject = zombieList[Random.Range(0, zombieList.Count)];

			//based on player weapon type find the most vulnerable type zombie to attack
			SurvivorStateMachine attackingSSM = survivorTurnList[0].GetComponent<SurvivorStateMachine>();
//			Debug.Log(attackingSSM.survivor.weaponEquipped.name);
			if (attackingSSM.survivor.weaponEquipped != null) {
				if (attackingSSM.survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.KNIFE) {
					foreach (GameObject zombie in zombieList) {
						//Debug.Log(zombie.GetComponent<ZombieStateMachine>().zombie.zombieType.ToString());
						if (zombie.GetComponent<ZombieStateMachine>().zombie.zombieType.ToString() == "SKINNY") {
							myAttack.TargetsGameObject = zombie;
							break;
						}
						Debug.Log("did not find a matching zombie to select for auto-attack");
						//if the foreach completes without finding a match- pick randomly
						myAttack.TargetsGameObject = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
					}

				} else if (attackingSSM.survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.CLUB) {
					foreach (GameObject zombie in zombieList) {
						//Debug.Log(zombie.GetComponent<ZombieStateMachine>().zombie.zombieType.ToString());
						if (zombie.GetComponent<ZombieStateMachine>().zombie.zombieType.ToString() == "NORMAL") {
							myAttack.TargetsGameObject = zombie;
							break;
						}
						Debug.Log("did not find a matching zombie to select for auto-attack");
						//if the foreach completes without finding a match- pick randomly
						myAttack.TargetsGameObject = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
					}

				} else if (attackingSSM.survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.GUN) {
					foreach (GameObject zombie in zombieList) {
						//Debug.Log(zombie.GetComponent<ZombieStateMachine>().zombie.zombieType.ToString());
						if (zombie.GetComponent<ZombieStateMachine>().zombie.zombieType.ToString() == "FAT") {
							myAttack.TargetsGameObject = zombie;
							break;
						}
						Debug.Log("did not find a matching zombie to select for auto-attack");
						//if the foreach completes without finding a match- pick randomly
						myAttack.TargetsGameObject = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
					}

				} 
			} else {
				myAttack.TargetsGameObject = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
			}
		}

		CollectAction(myAttack);
		survivorTurnList.RemoveAt(0);
	}

	public void SurvivorHasBeenBit (SurvivorStateMachine bitSurvivor) {
		//if it's player character, startup the end-game panel, otherwise just the survivor panel.
		if (bitSurvivor.teamPos == 5) {
			//this is end game condition. the player character is bit.
			Debug.Log ("PLAYER CHARACTER BIT!!!! END GAME SHOULD CALL HERE!~!!!");
		} else {
			//This is just a normal survivor dying.
			//stop the battlestate machine

			//we have to turn off all the zombie and player sprite renderers, or they will be on top of the panel, because of render order.
			foreach (GameObject survivor in survivorList) {
				survivor.GetComponent<SpriteRenderer>().enabled = false;
				survivor.GetComponent<SurvivorStateMachine>().DisableAllWeaponSprites();
			}
			foreach (GameObject zombie in zombieList) {
				zombie.GetComponent<SpriteRenderer>().enabled = false;
			}
			survivorBitText.text = bitSurvivor.survivor.name+" has been bit by the zombie!\nWhat will you do?";
			survivorBitPanel.SetActive(true);
			survivorWithBite = bitSurvivor;
		}
	}

	public void PlayerChooseKillSurvivor () {
		
		//send survivor ID to the server to destoy the record on the server.
		int survivorIDtoDestroy = survivorWithBite.survivor.survivor_id;
		StartCoroutine(SendDeadSurvivorToServer(survivorIDtoDestroy));

		//turn on all disabled survivors + zombies.
		GameObject destroyMe = null;
		foreach (GameObject survivor in survivorList)  {
			survivor.GetComponent<SpriteRenderer>().enabled = true;
			survivor.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
			//destroy the gameobject of the survivor that has died in the scene
			if (survivor.GetComponent<SurvivorStateMachine>().survivor.survivor_id == survivorIDtoDestroy) {
				destroyMe = survivor.gameObject;
			}
		}
		survivorTurnList.Remove(destroyMe);
		survivorList.Remove(destroyMe);
		Destroy(destroyMe);

		foreach (GameObject zombie in zombieList) {
			zombie.GetComponent<SpriteRenderer>().enabled = true;
		}

		//destroy the survivor record on the GameManager.
		GameObject destroyMe2 = null;
		foreach (GameObject surv in GameManager.instance.survivorCardList) {
			if (surv.GetComponent<SurvivorPlayCard>().survivor.survivor_id == survivorIDtoDestroy) {
				destroyMe2 = surv;
			}
		}
		GameManager.instance.survivorCardList.Remove(destroyMe2);
		Destroy(destroyMe2);
		//remove from the survivorlist on battlestatemachine


		//disable the survivor panel
		survivorBitPanel.SetActive(false);

		//resume combat
		battleState = PerformAction.WAIT;
	}

	IEnumerator SendDeadSurvivorToServer(int idToDestroy) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("survivor_id", idToDestroy);

		WWW www = new WWW(destroySurvivorURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
		}else {
			Debug.Log(www.error);
		}
	}

	public void PlayerChoosePurchaseSurvivorSave () {
		int survIDtoRestore = survivorWithBite.survivor.survivor_id;
		//disable the bite panel
		survivorBitPanel.SetActive(false);
		//turn zombies and survivors back on
		foreach (GameObject surv in survivorList) {
			surv.SetActive(true);
			if (surv.GetComponent<SurvivorStateMachine>().survivor.survivor_id == survIDtoRestore) {
				//send full stam to server, and set in UI
				SurvivorStateMachine mySSM = surv.GetComponent<SurvivorStateMachine>();
				mySSM.survivor.curStamina = mySSM.survivor.baseStamina;
				mySSM.UpdateStaminaBar();
				StartCoroutine(SendRestoreSurvivorToServer(survIDtoRestore));
			}
		}
		foreach (GameObject zombie in zombieList) {
			zombie.GetComponent<SpriteRenderer>().enabled = true;
		}
		foreach (GameObject surv in survivorList) {
			surv.GetComponent<SpriteRenderer>().enabled = true;
			surv.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
		}


		//resume combat
		battleState = PerformAction.WAIT;
	}

	IEnumerator SendRestoreSurvivorToServer(int idToRestore) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("survivor_id", idToRestore);

		WWW www = new WWW(restoreSurvivorURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
		}else {
			Debug.Log(www.error);
		}
	}

	public void PlayerChoosesRunAway () {
		//later this will roll odds for their ability to get away, and cost the active team a stamina penalty 

		SceneManager.LoadScene("02a Map Level");
	}
}
