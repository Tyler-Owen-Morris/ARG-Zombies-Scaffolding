using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class SurvivorStateMachine : MonoBehaviour {

	public bool isSelected;
	public BaseSurvivor survivor;
	public int teamPos;
	public GameObject mySelectedIcon, myGunShot, myClubSlash, myKnifeStab;
	public Slider myStamSlider;
	public GameObject[] myWepSprites;

	private BattleStateMachine BSM;
	private Vector3 startPosition;

	//stuff for taking a turn
	private bool actionStarted = false;
	private float animSpeed = 3000.0f;
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

	private string sendAttackURL = GameManager.serverURL+"/SendAttack.php";

	void Start () {
		BSM = FindObjectOfType<BattleStateMachine>();
		isSelected = false;
		currentState = TurnState.INITIALIZING;
		startPosition = gameObject.transform.position;
		UpdateWeaponSprite();

		myGunShot = transform.FindChild("GunShot").gameObject;
		myClubSlash = transform.FindChild("ClubSlash").gameObject;
		myKnifeStab = transform.FindChild("KnifeStab").gameObject;
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
			//match the sprite layer to that of the target
			int startRenderLayer = gameObject.GetComponent<SpriteRenderer>().sortingOrder;


			if (survivor.weaponEquipped != null){
				//attack with a weapon
				BaseWeapon myWeapon = survivor.weaponEquipped.GetComponent<BaseWeapon>();


				//animate the hero to the target, unless you have a gun equipped and at least 1 bullet
				if (myWeapon.weaponType == BaseWeapon.WeaponType.KNIFE || myWeapon.weaponType == BaseWeapon.WeaponType.CLUB || GameManager.instance.ammo < 1) {
					Vector3 targetPosition = new Vector3 (plyrTarget.transform.position.x - 0.3f, plyrTarget.transform.position.y, plyrTarget.transform.position.z);
					gameObject.GetComponent<SpriteRenderer>().sortingOrder = plyrTarget.GetComponent<SpriteRenderer>().sortingOrder;
					while (MoveTowardsTarget(targetPosition)) {yield return null;}
					if (myWeapon.weaponType == BaseWeapon.WeaponType.KNIFE) {
						//if player has a knife- activate the knife graphic game object
						myKnifeStab.SetActive(true);
					} else {
						//Whether it's a club or gun, activate the club animation
						myClubSlash.SetActive(true);
					}
				} else {
					myGunShot.SetActive(true);
				}
				BSM.PlayWeaponSound(myWeapon.weaponType, myWeapon.name);
				//animate weapon fx
				yield return new WaitForSeconds(0.5f);

				//do damage
				ZombieStateMachine targetZombie = plyrTarget.GetComponent<ZombieStateMachine>();
				int myDmg = CalculateMyDamage();
				Debug.Log ("Survivor hit the zombie for " +myDmg+ " damage");
				targetZombie.zombie.curHP = targetZombie.zombie.curHP - myDmg;
				targetZombie.CheckForDeath();
				StartCoroutine(SendAttackToServer(survivor.survivor_id, myWeapon.weapon_id));
				UpdateStaminaBar();
				if (survivor.weaponEquipped != null) {
					survivor.curStamina -= myWeapon.stam_cost;	
				}
				//Record the durability loss
				myWeapon.durability = myWeapon.durability - 1;
				if (myWeapon.durability < 1) {
//					//remove from list first
//					int whereIsIt = GameManager.instance.survivorCardList.IndexOf(survivor.weaponEquipped);
//					Debug.Log(whereIsIt);
//					GameManager.instance.survivorCardList.RemoveAt(whereIsIt);

					//destoy game object, and update survivor sprite
					Destroy(survivor.weaponEquipped.gameObject);
					UpdateWeaponSprite();

					//send message warning player.
					Debug.Log("oh noes! weapon is broken beyond repair!");
				}
			} else {
				//unequipped weapon attack.
				Vector3 targetPosition = new Vector3 (plyrTarget.transform.position.x - 0.3f, plyrTarget.transform.position.y, plyrTarget.transform.position.z);
				gameObject.GetComponent<SpriteRenderer>().sortingOrder = plyrTarget.GetComponent<SpriteRenderer>().sortingOrder;
				while (MoveTowardsTarget(targetPosition)) {yield return null;}

				//animate weapon fx
				BSM.PlayUnarmedSurvivorAttackSound();
				yield return new WaitForSeconds(0.2f);

				//do damage
				ZombieStateMachine targetZombie = plyrTarget.GetComponent<ZombieStateMachine>();
				int myDmg = CalculateMyDamage();
				Debug.Log ("Survivor hit the zombie for " +myDmg+ " damage");
				targetZombie.zombie.curHP = targetZombie.zombie.curHP - myDmg;
				targetZombie.CheckForDeath();
				StartCoroutine(SendAttackToServer(survivor.survivor_id, 0));
				//adjust the local data and update the UI
				survivor.curStamina -= 5;
				UpdateStaminaBar();

			}
			//return to start position, gun shots should already be there.
			while (MoveTowardsTarget(startPosition)) {yield return null;}
			//remove from list
			BSM.TurnList.RemoveAt(0);


			//reset BSM ->
			BSM.battleState = BattleStateMachine.PerformAction.WAIT;

			gameObject.GetComponent<SpriteRenderer>().sortingOrder = startRenderLayer;
			actionStarted = false;
			BlinkingSliderCheck();
			currentState = TurnState.DONE;
		}
		
	}
	private bool MoveTowardsTarget (Vector3 goal) {
		return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
	}

	IEnumerator SendAttackToServer (int survivorID, int weaponID) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("survivor_id", survivorID);
		form.AddField("weapon_id", weaponID);

		WWW www = new WWW(sendAttackURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
			JsonData attackReturn = JsonMapper.ToObject(www.text);

			if (attackReturn[0].ToString() == "Success") {
				Debug.Log(attackReturn[1].ToString());
			} else {
				Debug.Log(attackReturn[1].ToString());
			}
		} else {
			Debug.Log(www.error);
		}
	}

	private int CalculateMyDamage () {
		ZombieStateMachine myTarget = plyrTarget.GetComponent<ZombieStateMachine>();
		int myDmg = 0;
		myDmg += survivor.baseAttack;
		if (survivor.weaponEquipped != null) {
			if (survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.KNIFE) {
				int wepDmg = survivor.weaponEquipped.GetComponent<BaseWeapon>().base_dmg + Random.Range(0, survivor.weaponEquipped.GetComponent<BaseWeapon>().modifier);
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
				int wepDmg = survivor.weaponEquipped.GetComponent<BaseWeapon>().base_dmg + Random.Range(0, survivor.weaponEquipped.GetComponent<BaseWeapon>().modifier);
				if (myTarget.zombie.zombieType == BaseZombie.Type.FAT) {
					float multiplier = Random.Range(0.6f, 1.0f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("CLUB on FAT zombie is weak. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.NORMAL) {
					float multiplier = Random.Range(1.1f, 2.1f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("CLUB on NORMAL zombie is super strong. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.SKINNY) {
					float multiplier = Random.Range(0.9f, 1.25f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("CLUB on SKINNY zombie is safe. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				}else {
					myDmg = Mathf.RoundToInt((myDmg + wepDmg));
					Debug.Log ("No Zombie type found- returning dmg without multiplier: "+ myDmg);
					//return myDmg;
				}
		
			} else if (survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo > 0) {
				int wepDmg = survivor.weaponEquipped.GetComponent<BaseWeapon>().base_dmg + Random.Range(0, survivor.weaponEquipped.GetComponent<BaseWeapon>().modifier);
				if (myTarget.zombie.zombieType == BaseZombie.Type.FAT) {
					float multiplier = Random.Range(1.1f, 2.25f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("GUN on FAT zombie is super strong. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.NORMAL) {
					float multiplier = Random.Range(0.9f, 1.25f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("GUN on NORMAL zombie is safe. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.SKINNY) {
					float multiplier = Random.Range(0.6f, 1.0f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("GUN on SKINNY zombie is weak. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else {
					myDmg = Mathf.RoundToInt((myDmg + wepDmg));
					Debug.Log ("No Zombie type found- returning dmg without multiplier: "+ myDmg);
					//return myDmg;
				}
				//this is where we remove the ammo from the game. server should execute the amo reduction when the attack is sent
				GameManager.instance.ammo--;
				BSM.UpdateUINumbers();
			} else if (survivor.weaponEquipped.GetComponent<BaseWeapon>().weaponType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo < 1) {
				int wepDmg = survivor.weaponEquipped.GetComponent<BaseWeapon>().base_dmg + Random.Range(0, survivor.weaponEquipped.GetComponent<BaseWeapon>().modifier);
				if (myTarget.zombie.zombieType == BaseZombie.Type.FAT) {
					float multiplier = Random.Range(1.1f, 2.25f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("GUN on FAT zombie is super strong. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.NORMAL) {
					float multiplier = Random.Range(0.9f, 1.25f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("GUN on NORMAL zombie is safe. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.SKINNY) {
					float multiplier = Random.Range(0.6f, 1.0f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("GUN on SKINNY zombie is weak. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else {
					myDmg = Mathf.RoundToInt((myDmg + wepDmg));
					Debug.Log ("No Zombie type found- returning dmg without multiplier: "+ myDmg);
					//return myDmg;
				}
				myDmg = Mathf.RoundToInt(myDmg * 0.6f);
			} else {
				Debug.Log ("MAJOR error, there's a weapon equipped without a type value. executing an unarmed swing, but this is broken");
				int multiplier = Random.Range(0, 7);
				myDmg += multiplier;
				//return myDmg;
			}
		} else {
			Debug.Log ("!!PLAYER HAS NO WEAPON EQUIPPED!! this should set off alarms to the player");
			int multiplier = Random.Range(0, 7);
			myDmg += multiplier;
			//return myDmg;
		}

		//if player is out of stamina, their attack is halved
		if (survivor.curStamina < 1) {
			myDmg = Mathf.RoundToInt(myDmg/2);
		}

		return myDmg;
	}

	private void BlinkingSliderCheck () {
		BlinkingSlider myBlinkingSlider = myStamSlider.gameObject.GetComponent<BlinkingSlider>();
		if (survivor.curStamina < -50) {
			myBlinkingSlider.StartBlinking(0.2f);
		} else if (survivor.curStamina < -25) {
			myBlinkingSlider.StartBlinking(0.6f);
		} else if (survivor.curStamina < -10) {
			myBlinkingSlider.StartBlinking(1.1f);
		} else if (survivor.curStamina >= 0) {
			myBlinkingSlider.StopBlinking();
		}
	}

	public void UpdateStaminaBar () {
		float sliderValue = ((float)(survivor.curStamina) / (float)(survivor.baseStamina));
		if (sliderValue <=0) {
			sliderValue=0;
		}
		myStamSlider.value = sliderValue;
		//Debug.Log ("Setting "+gameObject.name+" stamina slider to "+sliderValue);
	}

	public void DisableAllWeaponSprites () {
		foreach (GameObject sprite in myWepSprites) {
			sprite.SetActive(false);
		}
		mySelectedIcon.SetActive(false);
	}

	public void UpdateWeaponSprite () {
		//first turn all of the sprites off.
		foreach (GameObject sprite in myWepSprites) {
			sprite.SetActive(false);
		}
		if (this.survivor.weaponEquipped != null) {
			Debug.Log(this.survivor.name + " believes to have a weapon equipped , wep name: " + this.survivor.weaponEquipped.name);

			//this should change to get the name
			if (this.survivor.weaponEquipped.GetComponent<BaseWeapon>().name == "crude shiv") {
				myWepSprites[0].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a knife sprite");
			} else if (this.survivor.weaponEquipped.GetComponent<BaseWeapon>().name == "baseball bat") {
				myWepSprites[1].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a club sprite");
			} else if (this.survivor.weaponEquipped.GetComponent<BaseWeapon>().name == ".22 Revolver" ) {
				myWepSprites[2].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}else if (this.survivor.weaponEquipped.GetComponent<BaseWeapon>().name == "hunting knife" ) {
				myWepSprites[3].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}else if (this.survivor.weaponEquipped.GetComponent<BaseWeapon>().name == "sledgehammer" ) {
				myWepSprites[4].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}else if (this.survivor.weaponEquipped.GetComponent<BaseWeapon>().name == "shotgun" ) {
				myWepSprites[5].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}
		} else {
			Debug.Log("Warning: No weapon equipped!!!");
		}
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
