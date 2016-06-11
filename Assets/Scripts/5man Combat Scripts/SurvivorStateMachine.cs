using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SurvivorStateMachine : MonoBehaviour {

	public bool isSelected;
	public BaseSurvivor survivor;
	public GameObject mySelectedIcon;
	public Slider myStamSlider;

	private BattleStateMachine BSM;
	private Vector3 startPosition;

	//stuff for taking a turn
	private bool actionStarted = false;
	private float animSpeed = 15.0f;
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
				UpdateStaminaBar();
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

			ZombieStateMachine targetZombie = plyrTarget.GetComponent<ZombieStateMachine>();
			int myDmg = CalculateMyDamage();
			Debug.Log ("Survivor hit the zombie for " +myDmg+ " damage");
			targetZombie.zombie.curHP = targetZombie.zombie.curHP - myDmg;
			targetZombie.CheckForDeath();
			survivor.curStamina -= survivor.weaponEquipped.GetComponent<BaseWeapon>().stamCost;
			UpdateStaminaBar();

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

	private int CalculateMyDamage () {
		ZombieStateMachine myTarget = plyrTarget.GetComponent<ZombieStateMachine>();
		int myDmg = 0;
		myDmg += survivor.baseAttack;

		if (survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.KNIFE) {
			int wepDmg = Random.Range(survivor.weaponEquipped.GetComponent<BaseWeapon>().botDmg, survivor.weaponEquipped.GetComponent<BaseWeapon>().topDmg);
			if (myTarget.zombie.zombieType == BaseZombie.Type.FAT) {
				float multiplier = Random.Range(0.9f, 1.25f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("KNIFE on FAT zombie is safe. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			} else if (myTarget.zombie.zombieType == BaseZombie.Type.NORMAL) {
				float multiplier = Random.Range(0.6f, 1.0f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("KNIFE on NORMAL zombie is weak. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			} else if (myTarget.zombie.zombieType == BaseZombie.Type.SKINNY) {
				float multiplier = Random.Range(1.1f, 2.1f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("KNIFE on SKINNY zombie is super strong. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			}else {
				myDmg = Mathf.RoundToInt((myDmg + wepDmg));
				Debug.Log ("No Zombie type found- returning dmg without multiplier: "+ myDmg);
				return myDmg;
			}

		} else if (survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.CLUB) {
			int wepDmg = Random.Range(survivor.weaponEquipped.GetComponent<BaseWeapon>().botDmg, survivor.weaponEquipped.GetComponent<BaseWeapon>().topDmg);
			if (myTarget.zombie.zombieType == BaseZombie.Type.FAT) {
				float multiplier = Random.Range(0.6f, 1.0f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("CLUB on FAT zombie is weak. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			} else if (myTarget.zombie.zombieType == BaseZombie.Type.NORMAL) {
				float multiplier = Random.Range(1.1f, 2.1f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("CLUB on NORMAL zombie is super strong. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			} else if (myTarget.zombie.zombieType == BaseZombie.Type.SKINNY) {
				float multiplier = Random.Range(0.9f, 1.25f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("CLUB on SKINNY zombie is safe. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			}else {
				myDmg = Mathf.RoundToInt((myDmg + wepDmg));
				Debug.Log ("No Zombie type found- returning dmg without multiplier: "+ myDmg);
				return myDmg;
			}
	
		} else if (survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.GUN) {
			int wepDmg = Random.Range(survivor.weaponEquipped.GetComponent<BaseWeapon>().botDmg, survivor.weaponEquipped.GetComponent<BaseWeapon>().topDmg);
			if (myTarget.zombie.zombieType == BaseZombie.Type.FAT) {
				float multiplier = Random.Range(1.1f, 2.25f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("GUN on FAT zombie is super strong. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			} else if (myTarget.zombie.zombieType == BaseZombie.Type.NORMAL) {
				float multiplier = Random.Range(0.9f, 1.25f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("GUN on NORMAL zombie is safe. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			} else if (myTarget.zombie.zombieType == BaseZombie.Type.SKINNY) {
				float multiplier = Random.Range(0.6f, 1.0f);
				myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
				Debug.Log ("GUN on SKINNY zombie is weak. multiplier is "+multiplier+" making final damage= " +myDmg);
				return myDmg;
			} else {
				myDmg = Mathf.RoundToInt((myDmg + wepDmg));
				Debug.Log ("No Zombie type found- returning dmg without multiplier: "+ myDmg);
				return myDmg;
			}

		} else {
			Debug.Log ("!!PLAYER HAS NO WEAPON EQUIPPED!! this should set off alarms to the player");
			return myDmg;
		}
	}

	void UpdateStaminaBar () {
		float sliderValue = ((float)(survivor.curStamina) / (float)(survivor.baseStamina));
		myStamSlider.value = sliderValue;
		//Debug.Log ("Setting "+gameObject.name+" stamina slider to "+sliderValue);
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
