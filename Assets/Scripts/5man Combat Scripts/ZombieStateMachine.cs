using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZombieStateMachine : MonoBehaviour {

	public BaseZombie zombie;
	public GameObject myTargetGraphic;
	public Text myTypeText;

	private BattleStateMachine BSM;
	private Vector3 startPosition;
	private Quaternion startRotation;
	private Vector3 spawnPoint;
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
	private bool deathActionStarted = false;
	private float animSpeed = 15.0f;
	public GameObject target;


	// Use this for initialization
	void Start () {
		myTargetGraphic.SetActive(false);
		startPosition = gameObject.transform.position;
		startRotation = gameObject.transform.rotation;
		spawnPoint = new Vector3 (startPosition.x + 2.5f, startPosition.y, startPosition.z);
		currentState = TurnState.WAITING;
		BSM = FindObjectOfType<BattleStateMachine>();
		myTypeText.text = zombie.zombieType.ToString();
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
				if (GameManager.instance.zombiesToFight >= 6) {
					//kill me and refresh me.
					StartCoroutine(DeathAction(true));
				} else {
					//Kill me and do not refresh.
					StartCoroutine(DeathAction(false));
				}
			break;

		}
	}

	public void CheckForDeath () {
		if (zombie.curHP <= 0 ) {
			currentState = TurnState.DEAD;
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
			SurvivorStateMachine targetSurvivor = target.GetComponent<SurvivorStateMachine>();
			int myDmg = CalculateMyDamage ();
			Debug.Log ("Zombie hit Survivor for "+myDmg+" damage");
			targetSurvivor.survivor.curStamina -= myDmg;
			//check for player *exhaustion*


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

	private IEnumerator DeathAction (bool refresh) {
		if (deathActionStarted) {
			yield break;
		} else {
			deathActionStarted = true;
			BSM.zombieList.Remove(gameObject);
			GameManager.instance.zombiesToFight --;
			BSM.UpdateZombieCount();
			BSM.zombiesKilled ++;

			if (BSM.playerTarget != null) {
				BSM.playerTarget.GetComponent<ZombieStateMachine>().iAmPlayerTarget = false;
				BSM.playerTarget.GetComponent<ZombieStateMachine>().myTargetGraphic.SetActive(false);
				BSM.playerTarget = null;
			}

			//animate zombie to the ground.
			Quaternion downPos = new Quaternion (0,0,-90,0);
			while (RotateToTarget(downPos)) {yield return null;}
			//move to off screen Death/Spawn target
			while (MoveTowardsEnemy(spawnPoint)) {yield return null;}
			//check refresh
			if (refresh) {
				//reactivate the zombie, and return to start position. set state to waiting.
				ReRollType();
				zombie.curHP = zombie.baseHP;
				while (RefreshRotate(startRotation)) {yield return null;}
				while (MoveTowardsEnemy(startPosition)) {yield return null;}
				BSM.zombieList.Add(gameObject);
				currentState = TurnState.WAITING;
			} else {
				//do not reactivate or animate- just leave the zombie dead off screen and change its state.
				foreach (GameObject zombie in BSM.zombieList) {
					if (this.name == zombie.name) {
						//remove from game list
						zombie.GetComponent<ZombieStateMachine>().myTypeText.text = "";
						BSM.zombieList.Remove(zombie);
					}
				}
				currentState = TurnState.WAITING;
			}
			deathActionStarted = false;

		}
	}

	private bool RefreshRotate (Quaternion goal) {
		return goal != (transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, 90 *Time.deltaTime));
	}

	private bool RotateToTarget (Quaternion goal) {
		return goal != (transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, animSpeed *Time.deltaTime));
	}

	private bool MoveTowardsEnemy (Vector3 goal) {
		return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
	}

	void ReRollType () {
		int roll = Random.Range (1,3);
		if (roll == 1)  {
			zombie.zombieType = BaseZombie.Type.FAT;
		} else if (roll == 2) {
			zombie.zombieType = BaseZombie.Type.NORMAL;
		} else if (roll == 3) {
			zombie.zombieType = BaseZombie.Type.SKINNY;
		}
		myTypeText.text = zombie.zombieType.ToString();
	}

	int CalculateMyDamage () {
		return zombie.baseAttack;
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
