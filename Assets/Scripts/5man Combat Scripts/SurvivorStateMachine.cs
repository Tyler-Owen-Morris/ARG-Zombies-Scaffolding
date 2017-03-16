using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class SurvivorStateMachine : MonoBehaviour {

	public bool isSelected;
	public BaseSurvivor survivor;
	public int teamPos, turnsTillTurning;
	public GameObject mySelectedIcon, myGunShot, myClubSlash, myKnifeStab, combatTextPrefab;
	public Slider myStamSlider;
	public GameObject[] myWepSprites;
	public Text sliderNameText;
    public SpriteRenderer my_face;

	private BattleStateMachine BSM;
    private BossBattleStateMachine BBSM;
	private Vector3 startPosition;

	//stuff for taking a turn
	private bool actionStarted = false, iAmBit = false;
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
        BBSM = FindObjectOfType<BossBattleStateMachine>();
		isSelected = false;
		currentState = TurnState.INITIALIZING;
		startPosition = gameObject.transform.position;
		UpdateWeaponSprite();

		myGunShot = transform.FindChild("GunShot").gameObject;
		myClubSlash = transform.FindChild("ClubSlash").gameObject;
		myKnifeStab = transform.FindChild("KnifeStab").gameObject;

       
	}

    public void SetMyPofilePic(string url)
    {
        my_face.sprite = GameManager.instance.profileImageManager.GetMyProfilePic(survivor.survivor_id, url);
        Image healthbar_img = myStamSlider.transform.FindChild("Profile pic").GetComponent<Image>();
        healthbar_img.sprite = my_face.sprite;
        my_face.gameObject.SetActive(false);
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
                if (BSM != null) {
                    BSM.survivorTurnList.Add(this.gameObject);
                    //Debug.Log(this.gameObject.name + " is sending it's gameobject to the BSM- NOT BOSS COMBAT");
                }
                if (BBSM != null) {
                    BBSM.survivorTurnList.Add(this.gameObject);
                    //Debug.Log(this.gameObject.name + " is sending its gameobject to the BBSM- YES BOSS COMBAT");
                }
				currentState = TurnState.WAITING;
			break;
			case (TurnState.DONE):
				mySelectedIcon.SetActive(false);
				isSelected = false;
                if (BSM != null) { BSM.playerGUI = BattleStateMachine.PlayerInput.ACTIVATE; }
                if (BBSM != null) { BBSM.playerGUI = BossBattleStateMachine.PlayerInput.ACTIVATE; }
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
					Vector3 targetPosition = new Vector3 (plyrTarget.transform.position.x - 55.0f, plyrTarget.transform.position.y, plyrTarget.transform.position.z);
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
				
				

				//do damage
				ZombieStateMachine targetZombie = plyrTarget.GetComponent<ZombieStateMachine>();
                BossStateMachine targetBoss = plyrTarget.GetComponent<BossStateMachine>();
                //one of the above will return null.

				int myDmg = CalculateMyDamage();
                if (BSM != null) { BSM.PlayWeaponSound(myWeapon.weaponType, myWeapon.name, myDmg); } //tell normal battle state machine to play sound
                if (BBSM != null) { BBSM.PlayWeaponSound(myWeapon.weaponType, myWeapon.name, myDmg); } //tell BOSS battle state machine to play sound
                //animate weapon fx
				yield return new WaitForSeconds(0.5f);

                bool isDed = false;
                if (targetZombie != null)
                {
                    Debug.Log("Survivor hit the zombie for " + myDmg + " damage");
                    targetZombie.zombie.curHP = targetZombie.zombie.curHP - myDmg;
                    Debug.Log("Survivor sees the zombie HP at: " + targetZombie.zombie.curHP + " *******<<<<<<<<<<<<<<<<");
                    SpawnCombatDamageText(targetZombie.gameObject, myDmg);
                    isDed = targetZombie.CheckForDeath();
                } else if (targetBoss != null)
                {
                    Debug.Log("Survivor hit the BOSS for "+myDmg+" damage");
                    targetBoss.curHP = targetBoss.curHP - myDmg;
                    SpawnCombatDamageText(targetBoss.gameObject, myDmg);
                    isDed = targetBoss.CheckForDeath();
                }else
                {
                    Debug.Log("unable to identify target state machine type.");

                }


                //DO NOT send each attack to server- store results to list.
                //StartCoroutine(SendAttackToServer(survivor.survivor_id, myWeapon.weapon_id, isDed)); //no longer updating server on each attack.
                StoreAttack(survivor.survivor_id, myWeapon.weapon_id, isDed);

                UpdateStaminaBar();
				if (survivor.weaponEquipped != null) {
					survivor.curStamina -= myWeapon.stam_cost;	
				}
				//Record the durability loss
				myWeapon.durability = myWeapon.durability - 1;
				if (myWeapon.durability < 1) {
                    //notify the BSM so that player can select a new weapon.
                    BSM.WeaponDestroyed(this.survivor.survivor_id);

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
				bool isDed = targetZombie.CheckForDeath();
                if (isDed)
                {
                    targetZombie.currentState = ZombieStateMachine.TurnState.DEAD;
                    Debug.Log("Manually calling the zombie dead");
                }
                StoreAttack(survivor.survivor_id, 0, isDed);
                //StartCoroutine(SendAttackToServer(survivor.survivor_id, 0, isDed));
				//adjust the local data and update the UI
				survivor.curStamina -= 5;
				UpdateStaminaBar();

			}
			//return to start position, gun shots should already be there.
			while (MoveTowardsTarget(startPosition)) {yield return null;}
            //remove from list
            if (BSM != null) { BSM.TurnList.RemoveAt(0); }
            if (BBSM != null) { BBSM.turnList.RemoveAt(0); }

			if (iAmBit == true) {
				turnsTillTurning--;
				if (turnsTillTurning < 1) {
					TurnMeIntoAZombie();
				}
			}

            //reset BSM ->
            if (BSM != null) { BSM.battleState = BattleStateMachine.PerformAction.WAIT; }
            if (BBSM != null) { BBSM.battleState = BossBattleStateMachine.PerformAction.WAIT; }

			gameObject.GetComponent<SpriteRenderer>().sortingOrder = startRenderLayer;
			actionStarted = false;
			BlinkingSliderCheck();
			currentState = TurnState.DONE;
		}
		
	}
	private bool MoveTowardsTarget (Vector3 goal) {
		return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
	}

	IEnumerator SendAttackToServer (int survivorID, int weaponID, bool kill) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("survivor_id", survivorID);
		form.AddField("weapon_id", weaponID);
		form.AddField("bldg_name", GameManager.instance.activeBldg_name);
		if (kill == true) {
			form.AddField("zombie_kill", "1");
		} else {
			form.AddField("zombie_kill", "0");
		}

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

    private void StoreAttack (int survivor_id, int weapon_id, bool ded)
    {
        TurnResultHolder myTurnResult = new TurnResultHolder();
        myTurnResult.attackType = "survivor";
        myTurnResult.survivor_id = survivor_id;
        myTurnResult.weapon_id = weapon_id;
        if (ded)
        {
            myTurnResult.dead = 1;
        }else
        {
            myTurnResult.dead = 0;
        }

        if (BSM != null) { BSM.turnResultJsonString +=
            "{\"attackType\":\"survivor\",\"survivor_id\":" + survivor_id + ",\"weapon_id\":" + weapon_id + ",\"dead\":" + myTurnResult.dead + "},";
        } else if (BBSM != null)
        {
           BBSM.turnResultJsonString +=
                "{\"attackType\":\"survivor\",\"survivor_id\":" + survivor_id + ",\"weapon_id\":" + weapon_id + ",\"dead\":" + myTurnResult.dead + "},";
        }
        else
        {
            Debug.LogError("Unable to locate battle manager");
        }
        //BSM.turnResultList.Add(myTurnResult);
    }

	private int CalculateMyDamage () {
		ZombieStateMachine myTarget = plyrTarget.GetComponent<ZombieStateMachine>();
        float miss_chance = 0.0f;
		int myDmg = 0;
		myDmg += survivor.baseAttack;
		if (survivor.weaponEquipped != null) {
            BaseWeapon my_weapon = survivor.weaponEquipped.GetComponent<BaseWeapon>();
            miss_chance = my_weapon.miss_chance * 0.01f;//stored as int 0-100 - multiply by 1/100th to get %value 0-1

            if (myTarget == null)
            {
                BossStateMachine bossCheck = plyrTarget.GetComponent<BossStateMachine>();
                if (bossCheck != null)
                {
                    if (my_weapon != null)
                    {
                        int wepDmg = my_weapon.base_dmg + Random.Range(0, my_weapon.modifier);
                        myDmg += wepDmg;
                    }

                    float boss_miss_roll = Random.Range(0.0f, 1.0f);
                    if (boss_miss_roll < miss_chance)
                    {
                        myDmg = 0;
                        Debug.Log("PLAYER MISSED!!! MISS MISS MISS MISS MISSSSYS MISSSYYYYYY MISS SMSSSSS!!!!!!!!");
                    }

                    Debug.Log("Player DMG against BOSS is: " + myDmg);
                    return myDmg;
                }
                else
                {
                    Debug.Log("Survivor unable to determine enemy state machine- Zombie and Boss have both returned null");
                }
            }

            //handle all NON-BOSS combat interactions
            if (my_weapon.weaponType == BaseWeapon.WeaponType.KNIFE) {
				int wepDmg = my_weapon.base_dmg + Random.Range(0, my_weapon.modifier);
				if (myTarget.zombie.zombieType == BaseZombie.Type.FAT) {
					float multiplier = Random.Range(0.9f, 1.25f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("KNIFE on FAT zombie is safe. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.NORMAL) {
					float multiplier = Random.Range(0.6f, 1.0f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("KNIFE on NORMAL zombie is weak. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				} else if (myTarget.zombie.zombieType == BaseZombie.Type.SKINNY) {
					float multiplier = Random.Range(1.1f, 2.1f);
					myDmg = Mathf.RoundToInt((myDmg + wepDmg) * multiplier);
					Debug.Log ("KNIFE on SKINNY zombie is super strong. multiplier is "+multiplier+" making final damage= " +myDmg);
					//return myDmg;
				}else {
					myDmg = Mathf.RoundToInt((myDmg + wepDmg));
					Debug.Log ("No Zombie type found- returning dmg without multiplier: "+ myDmg);
					//return myDmg;
				}

			} else if (my_weapon.weaponType == BaseWeapon.WeaponType.CLUB) {
				int wepDmg = my_weapon.base_dmg + Random.Range(0, my_weapon.modifier);
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
		
			} else if (my_weapon.weaponType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo > 0) {
				int wepDmg = my_weapon.base_dmg + Random.Range(0, my_weapon.modifier);
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
			} else if (my_weapon.weaponType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo < 1) {
				int wepDmg = my_weapon.base_dmg + Random.Range(0, my_weapon.modifier);
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

		//if player is out of stamina, their attack is 3/4th'd
		if (survivor.curStamina < 1) {
			myDmg = Mathf.RoundToInt(myDmg*0.75f);
		}

        float miss_roll = Random.Range(0.0f, 1.0f);
        if (miss_roll < miss_chance)
        {
            myDmg = 0;
            Debug.Log("PLAYER MISSED!!! MISS MISS MISS MISS MISSSSYS MISSSYYYYYY MISS SMSSSSS!!!!!!!!");
        }

		return myDmg;
	}

    void SpawnCombatDamageText (GameObject trgt, int val)
    {
        GameObject[] deleteThese = GameObject.FindGameObjectsWithTag("deleteme");
        foreach (GameObject dlt in deleteThese)
        {
            Destroy(dlt);
        }

        //spawn empty game object, attach to canvas, and locate in correct position.
        GameObject canvas = GameObject.Find("Canvas");
        GameObject empty = new GameObject();
        empty.name = "deleteme";
        empty.tag = "deleteme";
        GameObject zombie_pos = Instantiate(empty);
        zombie_pos.transform.SetParent(canvas.transform, false);
        Camera myCamera = FindObjectOfType<Camera>();
        Vector3 screen_pos = myCamera.WorldToScreenPoint(trgt.transform.position);
        Vector3 tmp_pos = screen_pos + new Vector3(0.0f, 100.0f, 0.0f);
        zombie_pos.transform.position = tmp_pos;

        //get the prefab and instantiate it
        GameObject dmgTxtPrefab = Resources.Load<GameObject>("Prefabs/Damage Text Prefab");
        GameObject instance = Instantiate(dmgTxtPrefab);
        StaminaText dmgText = instance.GetComponent<StaminaText>();
        instance.transform.SetParent(zombie_pos.transform, false);
        dmgText.SetDamageText(val.ToString());
        
    }

	public void BiteTimerStart () {
		//calculate how many turns you have left
		int turns = Random.Range(1, 5);
		turnsTillTurning = turns;
		iAmBit = true;
	}

	private void TurnMeIntoAZombie () {
		//count the zombies to kill up 1
		GameManager.instance.activeBldg_zombies++;

		//remove the survivor from the battlestate machine.
		BSM.survivorList.Remove(this.gameObject);
		BSM.survivorTurnList.Remove(this.gameObject);

		//remove the survivor from the gamemanager
		GameManager.instance.LoadAllGameData();
		Destroy(this.gameObject);
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
            //Debug.Log(this.survivor.name + " believes to have a weapon equipped , wep name: " + this.survivor.weaponEquipped.name);

            //this should change to get the name
            string wep_name = this.survivor.weaponEquipped.GetComponent<BaseWeapon>().name;
            if ( wep_name == "crude shiv" || wep_name =="shank" ) {
				myWepSprites[0].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a knife sprite");
			} else if (wep_name == "deadly bat" || wep_name == "baseball bat") {
				myWepSprites[1].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a club sprite");
			} else if (wep_name == ".22 Revolver" || wep_name == ".22 revolver") {
				myWepSprites[2].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}else if (wep_name == "hunting knife" || wep_name == "basic knife") {
				myWepSprites[3].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}else if (wep_name == "sledgehammer" ) {
				myWepSprites[4].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}else if (wep_name == "shotgun" ) {
				myWepSprites[5].SetActive(true);
				//Debug.Log (this.survivor.name + " is equipping a gun sprite");
			}else if (wep_name == "zip gun")
            {
                myWepSprites[6].SetActive(true);
            }else if (wep_name == "zip gun 2.0")
            {
                myWepSprites[7].SetActive(true);
            }else if (wep_name == "crude club")
            {
                myWepSprites[8].SetActive(true);
            }else if (wep_name == "reinforced club")
            {
                myWepSprites[9].SetActive(true);
            }else
            {
                Debug.Log("unable to find a sprite match for currently equipped weapon: " + wep_name);
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
