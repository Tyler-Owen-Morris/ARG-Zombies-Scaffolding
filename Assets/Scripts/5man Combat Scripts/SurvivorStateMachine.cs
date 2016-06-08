using UnityEngine;
using System.Collections;

public class SurvivorStateMachine : MonoBehaviour {

	public bool isSelected;
	public BaseSurvivor survivor;
	public GameObject mySelectedIcon;

	private BattleStateMachine BSM;
	private Vector3 startPosition;

	//stuff for taking a turn
	private bool actionStarted = false;
	private float animSpeed = 10.0f;
	public GameObject plyrTarget;

	public enum TurnState 
	{
		WAITING,
		SELECTING,
		DONE,
		INITIALIZING,
		ACTION,
		DEAD
	}
	public TurnState currentState;

	void Start () {
		BSM = FindObjectOfType<BattleStateMachine>();
		isSelected = false;
		currentState = TurnState.INITIALIZING;
		startPosition = gameObject.transform.position;
	}
	

	void Update () {

		switch (currentState) 
		{
			case (TurnState.WAITING):
				//idle
			break;
			case (TurnState.SELECTING):
				
			break;
			case (TurnState.INITIALIZING):
				BSM.survivorTurnList.Add (this.gameObject);
				currentState = TurnState.WAITING;
			break;
			case (TurnState.DONE):
				mySelectedIcon.SetActive(false);
				isSelected = false;
				BSM.playerGUI = BattleStateMachine.PlayerInput.ACTIVATE;
			break;
			case (TurnState.ACTION):
				StartCoroutine(TakeAction());
			break;
			case (TurnState.DEAD):

			break;

		}

	}

	IEnumerator TakeAction () {
		if (actionStarted) {
			yield break;
		} else {
			actionStarted = true;
			int startRenderLayer = gameObject.GetComponent<SpriteRenderer>().sortingOrder;

			//animate the hero to the target
			Vector3 targetPosition = new Vector3 (plyrTarget.transform.position.x - 0.3f, plyrTarget.transform.position.y, plyrTarget.transform.position.z);
			gameObject.GetComponent<SpriteRenderer>().sortingOrder = plyrTarget.GetComponent<SpriteRenderer>().sortingOrder;
			while (MoveTowardsTarget(targetPosition)) {yield return null;}
			//animate weapon fx
			yield return new WaitForSeconds(0.2f);
			//do damage

			//return to start position
			while (MoveTowardsTarget(startPosition)) {yield return null;}
			//remove from list
			BSM.TurnList.RemoveAt(0);


			//reset BSM ->
			BSM.battleState = BattleStateMachine.PerformAction.WAIT;

			gameObject.GetComponent<SpriteRenderer>().sortingOrder = startRenderLayer;
			actionStarted = false;
			currentState = TurnState.DONE;
		}
		
	}
	private bool MoveTowardsTarget (Vector3 goal) {
		return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
	}

	//I decided to remove the player ability to select characters. inventory changes will take place only on player's turn.
	/*
	void OnMouseDown () {
		GameObject[] selectedIconObjectsArray = GameObject.FindGameObjectsWithTag("selectedIcon");
		foreach (GameObject selectedIcon in selectedIconObjectsArray){
			selectedIcon.gameObject.SetActive(false);
			SurvivorStateMachine SSM = gameObject.GetComponentInParent<SurvivorStateMachine>();
			SSM.isSelected = false;
		}

		this.isSelected = true;
		mySelectedIcon.SetActive(true);
	}
	*/
}
