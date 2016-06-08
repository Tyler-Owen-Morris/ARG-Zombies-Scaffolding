using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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


	void Start () {
		battleState = PerformAction.WAIT;
		playerGUI = PlayerInput.ACTIVATE;

		survivorList.AddRange (GameObject.FindGameObjectsWithTag("survivor"));
		zombieList.AddRange (GameObject.FindGameObjectsWithTag("zombie"));
	}
	
	// Update is called once per frame
	void Update () {

		switch (battleState) {
			case (PerformAction.WAIT):
				if (TurnList.Count > 0) {
					battleState = PerformAction.SELECTACTION;
				} else if (autoAttackIsOn && survivorTurnList.Count > 0) {
					AttackButtonPressed();
				} else if (survivorTurnList.Count < 1 && TurnList.Count < 1) {
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
}
