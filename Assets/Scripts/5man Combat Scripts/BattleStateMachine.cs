using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

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
	public GameObject autoAttackToggle, survivorBitPanel, playerBitPanel, failedRunAwayPanel, runButton, attackButton, cheatSaveButton;
	public AmputationButtonManager amputationButton;
	public GameObject playerPos1, playerPos2, playerPos3, playerPos4, playerPos5;
	[SerializeField]
	public bool autoAttackIsOn= false;

	public int zombiesKilled = 0;
	public Text zombieCounter, ammmoCounter, survivorBitText, failedToRunText;
    public GameObject blazeOfGloryImage;

    public Sprite[] zombie_sprite_array;

	public AudioClip knifeSound, clubSound, pistolSound, shotgunSound;
	public AudioClip[] zombieSounds, survivorUnarmedSounds;
	public AudioSource myAudioSource;

	public SurvivorStateMachine survivorWithBite;
	private string destroySurvivorURL = GameManager.serverURL+"/DestroySurvivor.php";
	private string injuredSurvivorURL = GameManager.serverURL+"/InjuredSurvivor.php";
	private string restoreSurvivorURL = GameManager.serverURL+"/RestoreSurvivor.php";
	private string combatSuicideURL = GameManager.serverURL+"/CombatSuicide.php";

	//private int totalSurvivorsFound = 0;
	void Awake () {
		LoadInSurvivorCardData();
	}

	void Start () {
		battleState = PerformAction.WAIT;
		playerGUI = PlayerInput.ACTIVATE;

		myAudioSource = GetComponent<AudioSource>();
		myAudioSource.playOnAwake = false;

		survivorList.AddRange (GameObject.FindGameObjectsWithTag("survivor"));
		zombieList.AddRange (GameObject.FindGameObjectsWithTag("zombie"));
		AdjustForLessThan5Zombies ();

		UpdateUINumbers();
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
		if (GameManager.instance.blazeOfGloryActive == true) {
			InitiateBlazeOfGlory();
		} else
        {
            blazeOfGloryImage.SetActive(false);
        }
	}

	void InitiateBlazeOfGlory () {
        blazeOfGloryImage.SetActive(true);
		cheatSaveButton.SetActive(false);
		runButton.SetActive(false);
		attackButton.SetActive(false);
		autoAttackToggle.GetComponent<Toggle>().isOn = true;
		autoAttackIsOn = true;
		autoAttackToggle.SetActive(false);
	}

	void LoadInSurvivorCardData () {

		//find the survivor play card with position 5, and put that as player in position 1
		foreach(GameObject survivorGameobject in GameManager.instance.activeSurvivorCardList) {
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
				mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
			} else if (myPlayCard.team_pos == 4) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos2.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
				mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
			} else if (myPlayCard.team_pos == 3) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos3.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
				mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
			} else if (myPlayCard.team_pos == 2) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos4.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
				mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
			} else if (myPlayCard.team_pos == 1) {
				SurvivorStateMachine mySurvivorStateMachine = playerPos5.GetComponent<SurvivorStateMachine>();
				mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
				mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina; 
				mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
				mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
				mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
				mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
				mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
				mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
			}
		}

		//if there are less than 5 survivors, remove the gameObjects starting with 5 and working up.
		if (GameManager.instance.activeSurvivorCardList.Count < 5) {
			int survivorsToDelete = 5 - GameManager.instance.activeSurvivorCardList.Count;
			for (int i = 0; i < survivorsToDelete; i++) {
				if (i == 0) {
					playerPos5.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
					Destroy(playerPos5);
				} else if (i == 1) {
					playerPos4.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
					Destroy(playerPos4);
				} else if (i == 2) {
					playerPos3.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
					Destroy(playerPos3);
				} else if (i == 3) {
					playerPos2.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
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
					int earned_supply = CalculateSupplyEarned();
					int earned_water = CalculateWaterFound();
					int earned_food = CalculateFoodFound();
                    bool found_survivor = CalculateSurvivorFound();
                    
					GameManager.instance.BuildingIsCleared(found_survivor);
					battleState = PerformAction.COMPLETED;
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
				if (performer != null) {
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
				} else {
					//attacker is dead, remove their turn, and go to wait mode
					TurnList.RemoveAt(0);
					battleState = PerformAction.WAIT;
				}

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

	public void PlayWeaponSound (BaseWeapon.WeaponType myWepType, string wepName) {
		if (myWepType == BaseWeapon.WeaponType.KNIFE) {
			//play the knife stab
			myAudioSource.PlayOneShot(knifeSound);
		} else if (myWepType == BaseWeapon.WeaponType.CLUB) {
			//play the club swing
			myAudioSource.PlayOneShot(clubSound);
		} else if (myWepType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo > 0) {
			//play the correct gunshot noise
			if (wepName == "shotgun") {
				myAudioSource.PlayOneShot(shotgunSound);	
			} else {
				myAudioSource.PlayOneShot(pistolSound);
			}
		} else if (myWepType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo < 1){
			//play the swing noise
			myAudioSource.PlayOneShot(clubSound);
		}
	}

	public void PlayZombieAttackSound () {
		int audioPosition = UnityEngine.Random.Range(0, zombieSounds.Length-1);
		myAudioSource.PlayOneShot(zombieSounds[audioPosition]);
	}

	public void PlayUnarmedSurvivorAttackSound () {
		int audioPosition = UnityEngine.Random.Range(0, zombieSounds.Length-1);
		myAudioSource.PlayOneShot(survivorUnarmedSounds[audioPosition]);
	}

	public void CollectAction (TurnHandler myTurn) {
		TurnList.Add (myTurn);
	}

	public void AutoAttackTogglePressed () {
		autoAttackIsOn = autoAttackToggle.GetComponent<Toggle>().isOn;
	}

	public void ResetAllTurns () {
		//clear and reset the lists
		TurnList.Clear();
		zombieList.Clear();
		survivorList.Clear();
		survivorList.AddRange (GameObject.FindGameObjectsWithTag("survivor"));
		zombieList.AddRange (GameObject.FindGameObjectsWithTag("zombie"));

		foreach (GameObject zombie in zombieList) {
			ZombieStateMachine ZSM = zombie.GetComponent<ZombieStateMachine>();
			ZSM.currentState = ZombieStateMachine.TurnState.CHOOSEACTION;
		}
		foreach (GameObject survivor in survivorList) {
			SurvivorStateMachine SSM = survivor.GetComponent<SurvivorStateMachine>();
			SSM.currentState = SurvivorStateMachine.TurnState.INITIALIZING;
		}

		battleState = PerformAction.WAIT;
	}

	/*
	public void ReTargetSurvivors () {
		//go through the survivors that have turns to come
		foreach (TurnHandler turn in TurnList) {
			//if the attacker is a survivor
			SurvivorStateMachine mySSM = turn.AttackersGameObject.GetComponent<SurvivorStateMachine>();
			if (mySSM != null) {
				TurnList.Remove(turn);
				//by removing the survivor from the turn list, but not the survivorturnlist, he should be added again, and re-pick his target.
				//this is called by the dead zombie to remove inactive zombies from becoming/remaining player targets.
			}
		}
	}
	*/

	public void UpdateUINumbers () {
		zombieCounter.text = GameManager.instance.zombiesToFight.ToString();
		ammmoCounter.text = "Ammo: "+GameManager.instance.ammo.ToString();
	}

	int CalculateSupplyEarned () {
        return GameManager.instance.activeBldg_supply;
        
        /*
        int supply_in_bldg = GameManager.instance.activeBldg_supply;

		if (supply_in_bldg > 0) {
			int supply_grabbed= UnityEngine.Random.Range(1, supply_in_bldg);
			return supply_grabbed;
		} else {
			return 0;
		}
		
		//Debug.Log ("Calculating the sum of supply earned from " + zombiesKilled + " zombies killed.");
		int sum = 0;

		for (int i = 0; i < zombiesKilled; i++) {
			int num = UnityEngine.Random.Range(0, 8);
			sum += num;
			//Debug.Log ("adding "+ num +" supply to the list");
		}
		float odds = CalculateCoreFalloffOdds()+0.2f; //flat 20% odds added to the max 50% that comes from 

		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
		//Roll being higher 
		if (roll > odds) {
			sum = 0;
		}

		return sum;
		*/
	}

	int CalculateWaterFound () {
        return GameManager.instance.activeBldg_water;
        /*
        int water_in_bldg = GameManager.instance.activeBldg_water;

		if (water_in_bldg > 0) {
			int water_found = UnityEngine.Random.Range(1, water_in_bldg);
			return water_found;
		} else {
			return 0;
		}
	
		/*
		int sum = 0;

		for (int i = 0; i < zombiesKilled; i++) {
				int amount = (int)UnityEngine.Random.Range( 1 , 4 );
				sum += amount;
		}

		float oddsToFind = CalculateCoreFalloffOdds()+0.05f; //5% flat odds to find
		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
		// this is so that you can find nothing
		if (roll > oddsToFind) {
			sum = 0;
		}

		return sum;
		*/
	}

	int CalculateFoodFound () {
        return GameManager.instance.activeBldg_food;
        /*
		int food_in_bldg = GameManager.instance.activeBldg_food;

		if (food_in_bldg > 0) {
			int food_found= UnityEngine.Random.Range(1, food_in_bldg);
			return food_found;
		} else {
			return 0;
		}
		/*
		int sum = 0;


		for (int i = 0; i < zombiesKilled; i++) {
				int amount = (int)UnityEngine.Random.Range( 1 , 4 );
				sum += amount;
		}

		float oddsToFind = CalculateCoreFalloffOdds()+0.05f; //%5 flat odds
		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
		// this is so that you can find nothing
		if (roll > oddsToFind) {
			sum = 0;
		}

		return sum;
		*/
	}

	bool CalculateSurvivorFound () {
		float odds =0.0f;

        GameManager.instance.daysSurvived = Mathf.FloorToInt((float)(DateTime.Now-GameManager.instance.timeCharacterStarted).TotalDays);

		if (GameManager.instance.daysSurvived < GameManager.DaysUntilOddsFlat) {
			DateTime now = System.DateTime.Now;
			double days_alive = (now-GameManager.instance.timeCharacterStarted).TotalDays;

			int exponent = 6; //the higher this exponent, the more rapid the falloff becomes.
			float max_percentage = 0.5f; //this starts us at 50/50 odds.
			double full_value = Mathf.Pow(GameManager.DaysUntilOddsFlat, exponent)/ max_percentage;

			float inverse_day_value = (float)(GameManager.DaysUntilOddsFlat - days_alive);
			float current_value = (float)(Mathf.Pow(inverse_day_value, exponent)/full_value);
			Debug.Log("calculating players odds to be at "+current_value);
			odds = current_value;
		}
		//else just let the odds remain 0.0f- nothing should be found.

		/*  This section is the old way I was evaluating odds to find a survivor. Retaining for reference

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
		*/

		float roll = UnityEngine.Random.Range(0.0f, 1.0f);
        Debug.Log(roll.ToString() + " is player roll, and " + odds + " are calculated player odds");
		if (roll < odds) {
			Debug.Log("survivor found!");
			return true;
		}else{
			Debug.Log("no survivors found");
			return false;
		}
	}

	float CalculateCoreFalloffOdds () {
		float odds =0.0f;

		if (GameManager.instance.daysSurvived < GameManager.DaysUntilOddsFlat) {
			DateTime now = System.DateTime.Now;
			double days_alive = (now-GameManager.instance.timeCharacterStarted).TotalDays;

			int exponent = 8;
			float max_percentage = 0.5f; //this starts us at 50/50 odds.
			double full_value = Mathf.Pow(GameManager.DaysUntilOddsFlat, exponent)/ max_percentage;

			float inverse_day_value = (float)(GameManager.DaysUntilOddsFlat - days_alive);
			float current_value = (float)(Mathf.Pow(inverse_day_value, exponent)/full_value);
			Debug.Log("calculating players odds to be at "+current_value);
			odds = current_value;
		}

		return odds;
	}

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
		GameObject[] battle_survivors = GameObject.FindGameObjectsWithTag("survivor");
		if (bitSurvivor.teamPos == 5 && battle_survivors.Length == 1) {
			//this is end game condition. the player character is bit.
			if (GameManager.instance.blazeOfGloryActive == true) {
				GameManager.instance.BuildingIsCleared(false);
				//this will launch the coroutine, which reacts to the string of active building, and notifies the server of game over, then loads game over scene
			} else {
				StartCoroutine(GameManager.instance.PlayerBit());
			}

			Debug.Log ("PLAYER CHARACTER BIT!!!! END GAME SHOULD CALL HERE!~!!!");
		} else if (bitSurvivor.teamPos != 5) {
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

			//update and show the panel.
			survivorBitText.text = bitSurvivor.survivor.name+" has been bit by the zombie!\nWhat will you do?";
			float timer = UnityEngine.Random.Range(4.0f, 9.5f);
			amputationButton.StartTheCountdown(timer);
			survivorBitPanel.SetActive(true);
			survivorWithBite = bitSurvivor;
		}
	}

	public void GameOverBiteCallback () {
		battleState = PerformAction.BITECASE;
		playerBitPanel.SetActive(true);
		KillYourselfButtonManager KYBM = KillYourselfButtonManager.FindObjectOfType<KillYourselfButtonManager>();
		float timer = UnityEngine.Random.Range(4.0f, 12.0f);
		KYBM.StartTheCountdown(timer);
	}

	public void SuccessfulCombatSuicide () {
		StartCoroutine(KillPlayerZombie());
	}

	public IEnumerator KillPlayerZombie() {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(combatSuicideURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData json = JsonMapper.ToObject(www.text);

			if (json[0].ToString() == "Success") {
				GameManager.instance.playerIsZombie = false;
				SceneManager.LoadScene("03b Game Over");
			}
		} else {
			Debug.Log(www.error);
		}

	}

	public void SuccessfulAmputation () {

		//send ID to server to add the survivor to the injured list.
		int survivorIDamputated = survivorWithBite.survivor.survivor_id;
		StartCoroutine(SendInjuredSurvivorToServer(survivorIDamputated));

		//re-enable all the game objects, except the injured survivor
		GameObject destroyMe = null;
		foreach (GameObject survivor in survivorList)  {
			survivor.GetComponent<SpriteRenderer>().enabled = true;
			survivor.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
			//destroy the gameobject of the survivor that has been injured in the scene
			if (survivor.GetComponent<SurvivorStateMachine>().survivor.survivor_id == survivorIDamputated) {
				destroyMe = survivor.gameObject;
			}
		}
		survivorTurnList.Remove(destroyMe);
		survivorList.Remove(destroyMe);
		Destroy(destroyMe);

		foreach (GameObject zombie in zombieList) {
			zombie.GetComponent<SpriteRenderer>().enabled = true;
		}

		//close the decision window & reset turns
		survivorBitPanel.SetActive(false);
		ResetAllTurns(); //this is to ensure the inactive player is not falsely targeted by a zombie. 

		//pop up text to notify player

		//resume combat
		//battleState = PerformAction.WAIT;
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
				break;
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
		foreach (GameObject surv in GameManager.instance.activeSurvivorCardList) {
			if (surv.GetComponent<SurvivorPlayCard>().survivor.survivor_id == survivorIDtoDestroy) {
				destroyMe2 = surv;
			}
		}
		GameManager.instance.activeSurvivorCardList.Remove(destroyMe2);
		Destroy(destroyMe2);
		//remove from the survivorlist on battlestatemachine


		//disable the survivor panel
		survivorBitPanel.SetActive(false);
		ResetAllTurns();

		//resume combat
		//battleState = PerformAction.WAIT;
	}

	//this handles a non-player survivor being bit
	public void PlayerChoosesToFightOn () {
		//destroy survivor on the server
		StartCoroutine(SendDeadSurvivorToServer(survivorWithBite.survivor.survivor_id));

		//notify survivor state machine
		survivorWithBite.BiteTimerStart();

		//re-enable the UI
		foreach (GameObject survivor in survivorList)  {
			survivor.GetComponent<SpriteRenderer>().enabled = true;
			survivor.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
		}

		foreach (GameObject zombie in zombieList) {
			zombie.GetComponent<SpriteRenderer>().enabled = true;
		}

		//close window
		survivorBitPanel.SetActive(false);

		//resume combat
		battleState = PerformAction.WAIT;
	}

	//This handles the player character being bit
	public void PlayerChoosesToFightToTheBitterEnd () {
		//server has already been updated with the zombie status. 
		//changing these variables will allow combat to end, but GameOver/YouAreAZombie should load instead of a victory screen.
		GameManager.instance.activeBldg_name = "bite_case";
		GameManager.instance.playerIsZombie = true;
		playerBitPanel.SetActive(false);
		battleState = PerformAction.WAIT;
	}

	IEnumerator SendInjuredSurvivorToServer (int idInjured) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("survivor_id", idInjured);

		WWW www = new WWW(injuredSurvivorURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
		}else{
			Debug.Log(www.error);
		}
		ResetAllTurns();
	}

	IEnumerator SendDeadSurvivorToServer(int idToDestroy) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
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
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("survivor_id", idToRestore);

		WWW www = new WWW(restoreSurvivorURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
		}else {
			Debug.Log(www.error);
		}
	}

	//this is called from the run away failed panel
	public void TurnAndFight () {
		failedRunAwayPanel.SetActive(false);
		battleState = PerformAction.WAIT;
	}

	public void PlayerChoosesRunAway () {

		survivorList.RemoveAll(GameObject => GameObject == null);

		/*
		//clean the list of any destroyed game-objects
		for (int i = survivorList.Count; 0 < i ; i--) {
			if (survivorList[i] != null) {
				continue;
			} else {
				//since the list will collapse, we need to repeat the same position in the next loop
				survivorList.RemoveAt(i);
			}
		}
		*/

		int got_away = 0;
		int running_away = survivorList.Count;

		foreach(GameObject survivor in survivorList) {
			float odds = 0;
			SurvivorStateMachine mySSM = survivor.GetComponent<SurvivorStateMachine>();
			float healthPercent = 100*(mySSM.survivor.curStamina/mySSM.survivor.baseStamina);
			if (healthPercent < 10) {
				odds += 1;
			}
			if (mySSM.survivor.curStamina <=0) {
				odds +=3;
			}
			if (mySSM.survivor.curStamina <= -1*mySSM.survivor.baseStamina) {
				odds = odds * 5; //should =30%
			}
			if (mySSM.survivor.curStamina <= -2*mySSM.survivor.baseStamina) {
				odds = odds * 2; //should =60%
			}
			if (mySSM.survivor.curStamina <= -3*mySSM.survivor.baseStamina) {
				odds = odds + 30; //should total 90%
			}

			float roll = UnityEngine.Random.Range(0.0f, 100.0f);
			if (roll <= odds) {
				//this survivor has failed to run away.
				StartCoroutine(SendDeadSurvivorToServer(mySSM.survivor.survivor_id));
				failedToRunText.text = mySSM.survivor.name+" fell behind and was overcome by zombies. they didn't make it.\n what will you do?";
				failedRunAwayPanel.SetActive(true);
				ContinueRunningButtonManager myContRunButMgr = ContinueRunningButtonManager.FindObjectOfType<ContinueRunningButtonManager>();
				float tmr = UnityEngine.Random.Range(8.0f, 25.0f);
				myContRunButMgr.StartTheCountdown(tmr);
				break;
			} else {
			Debug.Log(mySSM.survivor.name+" successfully ran away.");
				got_away++;
				continue;
			}
		}

		//if everyone got away, go to map level. Otherwise the above loop has been broken with a dead survivor.
		if (got_away == running_away) {
			SceneManager.LoadScene("02a Map Level");
		}
	}
}
