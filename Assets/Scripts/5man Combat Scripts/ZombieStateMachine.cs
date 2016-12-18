using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpriteColorFX;

public class ZombieStateMachine : MonoBehaviour {

	public BaseZombie zombie;
	public GameObject myTargetGraphic;
	public Text myTypeText;
    [SerializeField]
    public ZombieColorProfile[] zombieColorProfileArray;

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
	private float animSpeed = 3000.0f;
	public GameObject target;

	private string ZombieAttackURL = GameManager.serverURL+"/ZombieAttack.php";


	// Use this for initialization
	void Start () {
		myTargetGraphic.SetActive(false);
		startPosition = gameObject.transform.position;
		startRotation = gameObject.transform.rotation;
		spawnPoint = new Vector3 (startPosition.x + 600f, startPosition.y, startPosition.z);
		currentState = TurnState.WAITING;
		BSM = FindObjectOfType<BattleStateMachine>();
		myTypeText.text = zombie.zombieType.ToString();
        SetMySprite();
	}

    void SetMySprite()
    {
        //find my sprite renderer
        SpriteRenderer my_spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        SpriteColorMasks3 spriteRecolorer = GetComponent<SpriteColorMasks3>();

        if (zombie.zombieType == BaseZombie.Type.SKINNY)
        {
            my_spriteRenderer.sprite = BSM.zombie_sprite_array[0];
            spriteRecolorer.textureMask = BSM.zombie_texture_array[0] as Texture2D;
        }else if (zombie.zombieType == BaseZombie.Type.NORMAL)
        {
            my_spriteRenderer.sprite = BSM.zombie_sprite_array[1];
            spriteRecolorer.textureMask = BSM.zombie_texture_array[1] as Texture2D;
        }
        else if (zombie.zombieType == BaseZombie.Type.FAT)
        {
            my_spriteRenderer.sprite = BSM.zombie_sprite_array[2];
            spriteRecolorer.textureMask = BSM.zombie_texture_array[2] as Texture2D;
        }
        spriteRecolorer.pixelOp = SpriteColorHelper.PixelOp.Overlay;
        RecolorMySprite();
        //spriteRecolorer.colorMaskRed = GimmieRandomColor();
        //spriteRecolorer.colorMaskGreen = GimmieRandomColor();
        //spriteRecolorer.colorMaskBlue = GimmieRandomColor();

        Debug.Log(spriteRecolorer.colorMaskRed.ToString() +"R :: "+spriteRecolorer.colorMaskGreen.ToString()+"G :: "+spriteRecolorer.colorMaskBlue.ToString()+"B");

        Debug.Log(this.gameObject.name + " is setting their sprite to a " + zombie.zombieType.ToString() + " zombie");
    }

    Vector4 GimmieRandomColor() {
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);
        float a = Random.Range(0.0f, 1.0f);
        return new Vector4(r, g, b, a);
    }

    [System.Serializable]
    public class ZombieColorProfile
    {
        public SpriteColorHelper.PixelOp comp_type;
        public Color r_channel;
        public Color g_channel;
        public Color b_channel;
    }

    void RecolorMySprite ()
    {
        SpriteColorMasks3 my_spriteRecolorer = GetComponent<SpriteColorMasks3>();

        if (my_spriteRecolorer!= null)
        {
            int pos = Random.Range(0, zombieColorProfileArray.Length - 1);
            my_spriteRecolorer.pixelOp = zombieColorProfileArray[pos].comp_type;
            my_spriteRecolorer.colorMaskRed = zombieColorProfileArray[pos].r_channel;
            my_spriteRecolorer.colorMaskGreen = zombieColorProfileArray[pos].g_channel;
            my_spriteRecolorer.colorMaskBlue = zombieColorProfileArray[pos].b_channel;
        }
        else
        {
            Debug.Log("no sprite color mask found- leaving default color");
        }
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

	public bool CheckForDeath () {
		if (zombie.curHP <= 0 ) {
			currentState = TurnState.DEAD;
			BSM.zombieList.Remove(this.gameObject);
			return true;
		} else {
			return false;
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

			Vector3 targetPosition = new Vector3(0, 0, 0); //just called to initialize the variable.
			if (target != null) {
				//animate the enemy near the hero to attack
				targetPosition = new Vector3(target.transform.position.x + 55.0f, target.transform.position.y, target.transform.position.z);
				gameObject.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder;
			} else {
				currentState = TurnState.CHOOSEACTION;
				actionStarted = false;
				StopCoroutine(TakeAction());
			}

			while (MoveTowardsEnemy(targetPosition)) {yield return null;}
			//animate weaponfx
			BSM.PlayZombieAttackSound();
			yield return new WaitForSeconds(0.25f);

			//do damage
			SurvivorStateMachine targetSurvivor = target.GetComponent<SurvivorStateMachine>();
			int myDmg = CalculateMyDamage ();
			StartCoroutine(SendZombieAttack(targetSurvivor.survivor.survivor_id, myDmg));
			Debug.Log ("Zombie hit Survivor for "+myDmg+" damage");
			targetSurvivor.survivor.curStamina -= myDmg;

			//check for having bit the player
			float odds = 2.0f;
			bool survivorBit= false;
			if (targetSurvivor.teamPos == 5) {
				//this is player character, he has different odds than the team
				GameObject[] survivorsInCombat = GameObject.FindGameObjectsWithTag("survivor");
				if (survivorsInCombat.Length > 1) {
					//player character cannot get bit
					odds = 0.0f;
				} else {
					odds += 0.5f;
				}

			} else {
				odds += 1.1f;//starting odds for a survivor to get bitten.
			}
			int cur_stam = targetSurvivor.survivor.curStamina;
			if (cur_stam < 1) {
				//if player is exhausted, 2.5x the odds to get bitten
				odds = odds*2.5f;
			}
			int double_neg_stam = targetSurvivor.survivor.baseStamina * -2;
			if (cur_stam < double_neg_stam) {
				odds = (float)(odds*2.5f);
			}
			float roll = Random.Range(0.0f, 100.0f);
			if (roll < odds) {
				//TARGET HAS BEEN BITTEN
				Debug.Log("Survivor "+targetSurvivor.survivor.name+" has been bitten!!!");
				BSM.SurvivorHasBeenBit (targetSurvivor);
				survivorBit = true;
			}


			//return to start position
			Vector3 firstPosition = startPosition;
			while (MoveTowardsEnemy(firstPosition)) {yield return null;}

			//remove turn from list
			BSM.TurnList.RemoveAt(0);

			//reset BSM -> Wait
			if (survivorBit) {
				BSM.battleState = BattleStateMachine.PerformAction.BITECASE;
			} else {
				BSM.battleState = BattleStateMachine.PerformAction.WAIT;
			}

			gameObject.GetComponent<SpriteRenderer>().sortingOrder = startRenderLayer;
			actionStarted = false;
			currentState = TurnState.WAITING;
		}
	}



	IEnumerator SendZombieAttack (int survivorID, int dmg) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("survivor_id", survivorID);
		form.AddField("dmg", dmg);

		WWW www = new WWW(ZombieAttackURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log("zombie damage sent");
		} else {
			Debug.Log(www.error);
		}
	}

	private IEnumerator DeathAction (bool refresh) {
		if (deathActionStarted) {
			yield break;
		} else {
			deathActionStarted = true;
			BSM.zombieList.Remove(gameObject);
			GameManager.instance.zombiesToFight --;
			BSM.UpdateUINumbers();
			BSM.zombiesKilled ++;

			if (BSM.playerTarget != null) {
				BSM.playerTarget.GetComponent<ZombieStateMachine>().iAmPlayerTarget = false;
				BSM.playerTarget.GetComponent<ZombieStateMachine>().myTargetGraphic.SetActive(false);
				BSM.playerTarget = null;
			}

			//animate zombie to the ground.
			Quaternion downPos = new Quaternion (0,0,-40,0);
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
				this.gameObject.SetActive(false);
				myTypeText.text = "";
				//do not reactivate or animate- just leave the zombie dead off screen and change its state.
				foreach (GameObject zombie in BSM.zombieList) {
					if (this.name == zombie.name) {
						//remove from game list
						zombie.GetComponent<ZombieStateMachine>().myTypeText.text = "";
						BSM.zombieList.Remove(zombie);

					}
				}
				//turn off in heirarchy- reset the turns- 
				Destroy(this.gameObject);
				currentState = TurnState.WAITING;
			}
			deathActionStarted = false;

		}
	}

	private bool RefreshRotate (Quaternion goal) {
		return goal != (transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, animSpeed *Time.deltaTime));
	}

	private bool RotateToTarget (Quaternion goal) {
		return goal != (transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, animSpeed *Time.deltaTime));
	}

	private bool MoveTowardsEnemy (Vector3 goal) {
		return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
	}

	void ReRollType () {
        //determine and set new type
		int roll = Random.Range (1,3);
		if (roll == 1)  {
			zombie.zombieType = BaseZombie.Type.FAT;
		} else if (roll == 2) {
			zombie.zombieType = BaseZombie.Type.NORMAL;
		} else if (roll == 3) {
			zombie.zombieType = BaseZombie.Type.SKINNY;
		}

        //update the UI text and sprite
		myTypeText.text = zombie.zombieType.ToString();
        SetMySprite();
	}

	int CalculateMyDamage () {
		return zombie.baseAttack;
	}

	//this is being used for players targeting zombies
	void OnMouseDown () {
        //provided characters have not already started moving.
		if (actionStarted == false) {
            //turn all target reticles off
            GameObject[] targetReticleObjects = GameObject.FindGameObjectsWithTag("targetReticle");
            foreach (GameObject targetReticle in targetReticleObjects)
            {
                ZombieStateMachine ZSM = gameObject.GetComponentInParent<ZombieStateMachine>();
                ZSM.iAmPlayerTarget = false;
                targetReticle.SetActive(false);
            }

            
            if (BSM.playerTarget != this.gameObject)
            {
                //if this is not already player target- set it to player target
                iAmPlayerTarget = true;
                myTargetGraphic.SetActive(true);
                BSM.playerTarget = this.gameObject;
            }else
            {
                //if this is already player target, UN-target this zombie
                //Debug.Log("un-targeting this zombie");
                BSM.playerTarget = null;
                iAmPlayerTarget = false;
                myTargetGraphic.SetActive(false);
            }
		}
	}
}
