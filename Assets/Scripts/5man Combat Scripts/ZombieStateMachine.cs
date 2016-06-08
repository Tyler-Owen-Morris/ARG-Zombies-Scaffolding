using UnityEngine;
using System.Collections;

public class ZombieStateMachine : MonoBehaviour {

	public BaseZombie zombie;
	public GameObject myTargetGraphic;

	private BattleStateMachine BSM;
	private Vector3 startPosition;
	public bool iAmPlayerTarget = false;

	//state machine enum
	public enum TurnState 
	{
		WAITING,
		CHOOSEACTION,
		TAKEACTION,
		DEAD
	}

	public TurnState currentState;

	//timeforaction variables
	private bool actionStarted = false;
	private float animSpeed = 10.0f;
	public GameObject target;

	// Use this for initialization
	void Start () {
		myTargetGraphic.SetActive(false);
		startPosition = gameObject.transform.position;
		currentState = TurnState.CHOOSEACTION;
		BSM = FindObjectOfType<BattleStateMachine>();
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentState) 
		{
			case (TurnState.WAITING):
				//idle
			break;
			case (TurnState.CHOOSEACTION):
				ChooseAction();
				currentState = TurnState.WAITING;
			break;
			case (TurnState.TAKEACTION):
				StartCoroutine (TakeAction());
			break;
			case (TurnState.DEAD):

			break;

		}
	}

	void ChooseAction () {

		TurnHandler myAttack = new TurnHandler();
		myAttack.attacker = zombie.name;
		myAttack.type = "zombie";
		myAttack.AttackersGameObject = this.gameObject;
		myAttack.TargetsGameObject = BSM.survivorList[Random.Range(0, BSM.survivorList.Count)];

		BSM.CollectAction(myAttack);
	}

	private IEnumerator TakeAction () {
		if (actionStarted) {
			yield break;
		} else {
			actionStarted = true;
			int startRenderLayer = gameObject.GetComponent<SpriteRenderer>().sortingOrder;

			//animate the enemy near the hero to attack
			Vector3 targetPosition = new Vector3(target.transform.position.x + 0.3f, target.transform.position.y, target.transform.position.z);
			gameObject.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder;
			while (MoveTowardsEnemy(targetPosition)) {yield return null;}
			//animate weaponfx
			yield return new WaitForSeconds(0.25f);
			//do damage

			//return to start position
			Vector3 firstPosition = startPosition;
			while (MoveTowardsEnemy(firstPosition)) {yield return null;}

			//remove turn from list
			BSM.TurnList.RemoveAt(0);

			//reset BSM -> Wait
			BSM.battleState = BattleStateMachine.PerformAction.WAIT;

			gameObject.GetComponent<SpriteRenderer>().sortingOrder = startRenderLayer;
			actionStarted = false;
			currentState = TurnState.WAITING;
		}
	}

	private bool MoveTowardsEnemy (Vector3 goal) {
		return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
	}

	//this is being used for players targeting zombies
	void OnMouseDown () {
		if (actionStarted == false) {
			GameObject[] targetReticleObjects = GameObject.FindGameObjectsWithTag("targetReticle");
			foreach (GameObject targetReticle in targetReticleObjects){
				ZombieStateMachine ZSM = gameObject.GetComponentInParent<ZombieStateMachine>();
				ZSM.iAmPlayerTarget =false;
				targetReticle.SetActive(false);
			}
			

			iAmPlayerTarget = true;
			myTargetGraphic.SetActive(true);
			BSM.playerTarget = this.gameObject;
		}
	}
}
