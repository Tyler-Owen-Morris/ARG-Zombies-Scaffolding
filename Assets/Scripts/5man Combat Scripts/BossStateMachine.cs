using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossStateMachine : MonoBehaviour {

    //health, dmg and UI variables
    public Slider bossHealthSlider;
    public int baseHP, curHP;
    public int baseAttack;
    public Sprite[] myBossSprites;//0- Owen 1-David 2-SuperZombie

    //private variables
    private BossBattleStateMachine BBSM;
    private Vector3 startPosition, spawnPoint;
    private Quaternion startRoation;

    public enum TurnState
    {
        WAITING,
        CHOOSEACTION,
        TAKEACTION,
        DEAD
    }

    public TurnState bossTurnState;

    //action variables
    private bool actionStarted = false;
    private bool deathActionStarted = false;
    public GameObject target;
    public float animSpeed = 3000.0f;

	void Start () {
        startPosition = gameObject.transform.position;
        startRoation = gameObject.transform.rotation;
        spawnPoint = new Vector3(startPosition.x + 600f, startPosition.y, startPosition.z);
        bossTurnState = TurnState.WAITING;
        BBSM = FindObjectOfType<BossBattleStateMachine>();

        SetUpTheBoss();
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		//include a call to the turn list "dumper"
		//DumpTurnsToServer(false);//boolean indicates building has NOT been cleared
	}

	void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
		//restart music volume
		MusicManager theMusicManager = FindObjectOfType<MusicManager>();
		theMusicManager.audioSource.volume = GamePreferences.GetMusicVolume();
		
	}

    void SetUpTheBoss()
    {
        SpriteRenderer mySpriteR = this.GetComponent<SpriteRenderer>();
        if (GameManager.instance.activeBldg_name == "owen")
        {
            mySpriteR.sprite = myBossSprites[0];
            int health = 400;
            baseHP = health;
            curHP = health;
            baseAttack = 15;
            Debug.Log("Setting up the Owen boss");
        }else if (GameManager.instance.activeBldg_name == "david")
        {
            mySpriteR.sprite = myBossSprites[1];
            int health = 400;
            baseHP = health;
            curHP = health;
            baseAttack = 15;
            Debug.Log("Setting up the David boss");
        }
        else if (GameManager.instance.activeBldg_name == "superzombie")
        {
            mySpriteR.sprite = myBossSprites[1];
            int health = 500;
            baseHP = health;
            curHP = health;
            baseAttack = 20;
            Debug.Log("Setting up the David boss");
        }
        else
        {
            Debug.Log("Unknown boss type has been rolled... unable to continue with combat");
            SceneManager.LoadScene("02a Map Level");
        }
		SetBossHealthSlider ();
    }
	
	void Update () {
	

        switch (bossTurnState)
        {
            case (TurnState.WAITING):
                //idle
                break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                bossTurnState = TurnState.WAITING;
                break;
            case (TurnState.TAKEACTION):
                StartCoroutine(TakeAction());
                break;
            case (TurnState.DEAD):
                StartCoroutine(DeathAction());
                break;
        }
	}

    void ChooseAction()
    {

        TurnHandler myAttack = new TurnHandler();
        myAttack.attacker = this.gameObject.name;
        myAttack.type = "zombie";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.TargetsGameObject = BBSM.survivorList[Random.Range(0, BBSM.survivorList.Count)];

        BBSM.CollectAction(myAttack);
    }

    private IEnumerator TakeAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        else
        {
            actionStarted = true;
            int startRenderLayer = gameObject.GetComponent<SpriteRenderer>().sortingOrder;

            Vector3 targetPosition = new Vector3(0, 0, 0); //just called to initialize the variable.

            if (target == null)
            {
                //pick a new target.
                int ind = Random.Range(0, BBSM.survivorList.Count - 1);
                target = BBSM.survivorList[ind];
            }

            //animate the enemy near the hero to attack
            targetPosition = new Vector3(target.transform.position.x + 55.0f, target.transform.position.y, target.transform.position.z);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder;


            while (MoveTowardsEnemy(targetPosition)) { yield return null; }
            //animate weaponfx
            BBSM.PlayZombieAttackSound();
            yield return new WaitForSeconds(0.25f);

			//fetch SSM
            SurvivorStateMachine targetSurvivor = target.GetComponent<SurvivorStateMachine>(); 

			//do damage
			int myDmg = baseAttack;
            Debug.Log("Boss hit Survivor for " + myDmg + " damage");
            targetSurvivor.survivor.curStamina -= myDmg;
			SpawnCombatDamageText (target.gameObject, myDmg);

            //check for having bit the player
            float odds = 2.0f;
            bool survivorBit = false;
            if (targetSurvivor.teamPos == 5)
            {
                //this is player character, he has different odds than the team
                GameObject[] survivorsInCombat = GameObject.FindGameObjectsWithTag("survivor");
                if (survivorsInCombat.Length > 1)
                {
                    //player character cannot get bit
                    odds = 0.0f;
                }
                else
                {
                    odds += 0.5f;
                }

                if ((GameManager.instance.GetCurrentTimeAlive().TotalDays) < 2)//if we're within the first 2 days- players can't die to zombies
                {
                    odds = 0.0f;
                }

            }
            else
            {
                odds += 1.1f;//starting odds for a survivor to get bitten.
            }
            int cur_stam = targetSurvivor.survivor.curStamina;
            if (cur_stam < 1)
            {
                //if player is exhausted, 2.5x the odds to get bitten
                odds = odds * 2.5f;
            }
            int double_neg_stam = targetSurvivor.survivor.baseStamina * -2;
            if (cur_stam < double_neg_stam)
            {
                odds = (float)(odds * 2.5f);
            }
            float roll = Random.Range(0.0f, 100.0f);
            if (roll < odds)
            {
                //TARGET HAS BEEN BITTEN
                Debug.Log("Survivor " + targetSurvivor.survivor.name + " has been bitten!!!");
                BBSM.SurvivorHasBeenBit(targetSurvivor);
                survivorBit = true;
            }

            //server update is repaced with locally storing the completed attack data here:
            StoreAttack(targetSurvivor.survivor.survivor_id, survivorBit);


            //return to start position
            Vector3 firstPosition = startPosition;
            while (MoveTowardsEnemy(firstPosition)) { yield return null; }

            //remove turn from list
            BBSM.turnList.RemoveAt(0);

            //reset BBSM -> Wait
            if (survivorBit)
            {
                BBSM.battleState = BossBattleStateMachine.PerformAction.BITECASE;
            }
            else
            {
                BBSM.battleState = BossBattleStateMachine.PerformAction.WAIT;
            }

            gameObject.GetComponent<SpriteRenderer>().sortingOrder = startRenderLayer;
            actionStarted = false;
            bossTurnState = TurnState.WAITING;
			SetBossHealthSlider ();
        }
    }

    private IEnumerator DeathAction()
    {
        Debug.Log(gameObject.name + " calling death");
        if (deathActionStarted)
        {
            yield break;
        }
        else
        {
            deathActionStarted = true;
           // BBSM.bossList.Remove(gameObject); //list object removed for single game object
            GameManager.instance.activeBldg_zombies--;
            BBSM.UpdateUINumbers();
            BBSM.zombiesKilled++;

            /*
            if (BBSM.playerTarget != null)
            {
                BBSM.playerTarget.GetComponent<ZombieStateMachine>().iAmPlayerTarget = false;
                BBSM.playerTarget.GetComponent<ZombieStateMachine>().myTargetGraphic.SetActive(false);
                BBSM.playerTarget = null;
            }
            */

            //animate zombie to the ground.
            Quaternion downPos = new Quaternion(0, 0, -40, 0);
            while (RotateToTarget(downPos)) { yield return null; }
            //move to off screen Death/Spawn target
            while (MoveTowardsEnemy(spawnPoint)) { yield return null; }


            //this.gameObject.SetActive(false);

            //This should send the combat results to server, and then call GameManager to notify building clear/combat over
            //then GameManager will handle load into victory screen through GameManger.BuildingClearCalled()
            BBSM.DumpTurnsToServer(true);

            //Destroy(this.gameObject);  //don't destroy the boss, this was for zombie killing
            bossTurnState = TurnState.WAITING;
            
            deathActionStarted = false;

        }
    }

	public void SetBossHealthSlider () {
		float val = (float)curHP / (float)baseHP;
		Debug.Log("Setting boss health slider to: " +val);
		bossHealthSlider.value = val;
	}

    private void StoreAttack(int surv_id, bool bite)
    {
        TurnResultHolder myTurnresult = new TurnResultHolder();
        myTurnresult.attackType = "zombie";
        myTurnresult.survivor_id = surv_id;
        if (bite)
        {
            myTurnresult.dead = 1;
        }
        else
        {
            myTurnresult.dead = 0;
        }

        BBSM.turnResultJsonString += "{\"attackType\":\"zombie\",\"survivor_id\":" + surv_id + ",\"weapon_id\":0,\"dead\":" + myTurnresult.dead + "},";
        //BBSM.turnResultList.Add(myTurnresult);
    }

    private bool RefreshRotate(Quaternion goal)
    {
        return goal != (transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, animSpeed * Time.deltaTime));
    }

    private bool RotateToTarget(Quaternion goal)
    {
        return goal != (transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsEnemy(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
    }

    public bool CheckForDeath()
    {
		SetBossHealthSlider ();

        if (curHP <= 0)
        {
            this.bossTurnState = TurnState.DEAD;
            Debug.Log("****************>>>>>>>>>>>>> BOSS found itself as DEAD :::: " + bossTurnState.ToString());
            return true;
        }
        else
        {
            Debug.Log("******************>>>>>>>>>>>>>>>>>>> BOSS found itself as ALIVE");
            return false;
        }

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
		if (val > 0) {
			instance.GetComponent<Text> ().color = Color.red;
		}else{
			instance.GetComponent<Text>().color = Color.blue;
		}
		instance.transform.SetParent(zombie_pos.transform, false);
		dmgText.SetDamageText(val.ToString());

	}



}
