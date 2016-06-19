using UnityEngine;
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
		PERFORMACTION,
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
	public GameObject autoAttackToggle;
	[SerializeField]
	public bool autoAttackIsOn= false;

	public int zombiesKilled = 0;
	public Text zombieCounter;

	private int totalSurvivorsFound = 0;

	void Start () {
		battleState = PerformAction.WAIT;
		playerGUI = PlayerInput.ACTIVATE;

		survivorList.AddRange (GameObject.FindGameObjectsWithTag("survivor"));
		zombieList.AddRange (GameObject.FindGameObjectsWithTag("zombie"));
		AdjustForLessThan5Zombies ();

		UpdateZombieCount();
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
					GameManager.instance.BuildingIsCleared(CalculateSupplyEarned(), CalculateWaterFound(), CalculateFoodFound(), CalculateFoundTotalSurvivors(), CalculateActiveSurvivorsFound());
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
				battleState = PerformAction.PERFORMACTION;

			break;
			case (PerformAction.PERFORMACTION):

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
	public void UpdateZombieCount () {
		zombieCounter.text = GameManager.instance.zombiesToFight.ToString();
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

	//this is activated on the GUI button press.
	public void AttackButtonPressed () {
		TurnHandler myAttack = new TurnHandler();
		myAttack.attacker = survivorTurnList[0].name;
		myAttack.type = "survivor";
		myAttack.AttackersGameObject = survivorTurnList[0];
		if (playerTarget != null) {
			myAttack.TargetsGameObject = playerTarget;
		} else {
			myAttack.TargetsGameObject = zombieList[Random.Range(0, zombieList.Count)];
		}

		CollectAction(myAttack);
		survivorTurnList.RemoveAt(0);
	}

	public void PlayerChoosesRunAway () {
		//later this will roll odds for their ability to get away, and cost the active team a stamina penalty 

		SceneManager.LoadScene("02a Map Level");
	}
}
