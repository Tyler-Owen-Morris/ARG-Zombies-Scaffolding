using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class BossBattleStateMachine : MonoBehaviour {

    //this state machine handles computer turns
	public enum PerformAction
    {
        WAIT,
        SELECTACTION,
        BITECASE,
        COMPLETED
    }
    public PerformAction battleState;

    //this state Machine handles player input
    public enum PlayerInput
    {
        ACTIVATE,
        WAITING
    }
    public PlayerInput playerGUI;

    public List<GameObject> survivorTurnList = new List<GameObject>();
    private TurnHandler survivorChoice;

    public List<TurnHandler> turnList = new List<TurnHandler>();
    public List<GameObject> survivorList = new List<GameObject>();
    public GameObject boss;

    public string turnResultJsonString = "";

    public GameObject weaponBrokenPanel, autoAttackToggle, survivorBitPanel, playerBitPanel, failedRunAwayPanel, runButton, attackButton, cheatSaveButton;
    public GameObject playerPos1, playerPos2, playerPos3, playerPos4, playerPos5;
    public bool autoAttackIsOn = false;
    public int zombiesKilled, brokenWep_surv_id;
    public Text ammoCounter, survivorBitText, failedToRunText;
    public AmputationButtonManager amputationButton;
    public CombatWeaponListPopulator my_CWLP;
    public SurvivorStateMachine survivorWithBite;

    public AudioClip missSound, knifeSound, clubSound, pistolSound, shotgunSound, foundZombieIntroSound;
    public AudioClip[] zombieSounds, survivorUnarmedSounds;
    public AudioSource myAudioSource;

    private string postTurnsURL = GameManager.serverURL + "/PostTurns.php";
    private string equipWeaponURL = GameManager.serverURL + "/EquipWeapon.php";
    private string combatSuicideURL = GameManager.serverURL + "/CombatSuicide.php";
    private string injuredSurvivorURL = GameManager.serverURL + "/InjuredSurvivor.php";
    private string destroySurvivorURL = GameManager.serverURL + "/DestroySurvivor.php";
    private string restoreSurvivorURL = GameManager.serverURL + "/RestoreSurvivor.php";
    private string storyRequestURL = GameManager.serverURL + "/StoryRequest.php";

    void Awake ()
    {
        battleState = PerformAction.COMPLETED; //set machine to idle while load in completes
        LoadInSurvivorCardData();
    }

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.playOnAwake = false;
        myAudioSource.volume = GamePreferences.GetSFXVolume();//set the SFX audio source to GamePref volume value

        survivorList.AddRange(GameObject.FindGameObjectsWithTag("survivor"));

        UpdateUINumbers();
        PlayIntroSound();

        //AFTER all other pieces of data are loaded- set the battlestate to start the machine
        battleState = PerformAction.WAIT;//this should start the machine moving.
        playerGUI = PlayerInput.ACTIVATE;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        //include a call to the turn list "dumper"
        DumpTurnsToServer(false);//boolean indicates building has NOT been cleared
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //restart music volume
        MusicManager theMusicManager = FindObjectOfType<MusicManager>();
        theMusicManager.audioSource.volume = GamePreferences.GetMusicVolume();
    }

    void LoadInSurvivorCardData()
    {

        //find the survivor play card with position 5, and put that as player in position 1
        foreach (GameObject survivorGameobject in GameManager.instance.activeSurvivorCardList)
        {
            //load in the card data off of current game object
            SurvivorPlayCard myPlayCard = survivorGameobject.GetComponent<SurvivorPlayCard>();
            //match corresponding players to their combat positions
            if (myPlayCard.team_pos == 5)
            {
                SurvivorStateMachine mySurvivorStateMachine = playerPos1.GetComponent<SurvivorStateMachine>();
                mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
                mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina;
                mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
                mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
                mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
                mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
                mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
                mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
                mySurvivorStateMachine.my_face.sprite = GameManager.instance.my_profile_pic;
                mySurvivorStateMachine.myStamSlider.gameObject.transform.Find("Profile pic").GetComponent<Image>().sprite = GameManager.instance.my_profile_pic;
                mySurvivorStateMachine.my_face.gameObject.SetActive(false);
            }
            else if (myPlayCard.team_pos == 4)
            {
                SurvivorStateMachine mySurvivorStateMachine = playerPos2.GetComponent<SurvivorStateMachine>();
                mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
                mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina;
                mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
                mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
                mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
                mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
                mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
                mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
                mySurvivorStateMachine.SetMyPofilePic(myPlayCard.profilePicURL);
            }
            else if (myPlayCard.team_pos == 3)
            {
                SurvivorStateMachine mySurvivorStateMachine = playerPos3.GetComponent<SurvivorStateMachine>();
                mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
                mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina;
                mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
                mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
                mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
                mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
                mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
                mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
                mySurvivorStateMachine.SetMyPofilePic(myPlayCard.profilePicURL);
            }
            else if (myPlayCard.team_pos == 2)
            {
                SurvivorStateMachine mySurvivorStateMachine = playerPos4.GetComponent<SurvivorStateMachine>();
                mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
                mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina;
                mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
                mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
                mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
                mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
                mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
                mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
                mySurvivorStateMachine.SetMyPofilePic(myPlayCard.profilePicURL);
            }
            else if (myPlayCard.team_pos == 1)
            {
                SurvivorStateMachine mySurvivorStateMachine = playerPos5.GetComponent<SurvivorStateMachine>();
                mySurvivorStateMachine.survivor.name = myPlayCard.survivor.name;
                mySurvivorStateMachine.survivor.baseStamina = myPlayCard.survivor.baseStamina;
                mySurvivorStateMachine.survivor.curStamina = myPlayCard.survivor.curStamina;
                mySurvivorStateMachine.survivor.baseAttack = myPlayCard.survivor.baseAttack;
                mySurvivorStateMachine.survivor.weaponEquipped = myPlayCard.survivor.weaponEquipped;
                mySurvivorStateMachine.survivor.survivor_id = myPlayCard.entry_id;
                mySurvivorStateMachine.teamPos = myPlayCard.team_pos;
                mySurvivorStateMachine.sliderNameText.text = myPlayCard.survivor.name;
                mySurvivorStateMachine.SetMyPofilePic(myPlayCard.profilePicURL);
            }
        }

        //if there are less than 5 survivors, remove the gameObjects starting with 5 and working up.
        if (GameManager.instance.activeSurvivorCardList.Count < 5)
        {
            int survivorsToDelete = 5 - GameManager.instance.activeSurvivorCardList.Count;
            for (int i = 0; i < survivorsToDelete; i++)
            {
                if (i == 0)
                {
                    playerPos5.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
                    Destroy(playerPos5);
                }
                else if (i == 1)
                {
                    playerPos4.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
                    Destroy(playerPos4);
                }
                else if (i == 2)
                {
                    playerPos3.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
                    Destroy(playerPos3);
                }
                else if (i == 3)
                {
                    playerPos2.GetComponent<SurvivorStateMachine>().myStamSlider.gameObject.SetActive(false);
                    Destroy(playerPos2);
                }
                else if (i == 4)
                {
                    Debug.Log("Major problem! you have no survivor cards to load into combat with");
                }
            }
        }
    }

    public void UpdateUINumbers()
    {
        ammoCounter.text = "Ammo: " + GameManager.instance.ammo.ToString();
    }

    void PlayIntroSound()
    {
        if (GameManager.instance.activeBldg_zombies > 0)
        {
            myAudioSource.PlayOneShot(foundZombieIntroSound);
        }
    }

    public void DumpTurnsToServer(bool clr)
    {
        Debug.Log(turnResultJsonString);
        if (turnResultJsonString != null || turnResultJsonString != "")
        {

            string json_data = "";
            if (turnResultJsonString.Length > 2)
            {
                json_data = "[" + turnResultJsonString.Substring(0, turnResultJsonString.Length - 1) + "]";//every entry ends with a comma, remove the comma and put caps on the json
            }
            else
            {
                json_data = "[]";
            }
            Debug.Log(json_data);

            StartCoroutine(PostTurnsToServer(json_data, clr));
        }
        else
        {
            Debug.Log("***");
            //check building status being passed
            if (clr)
            {
                GameManager.instance.BuildingIsCleared(false); //false means no survivors found, and this should load us to victory etc
            }
            else
            {
                //this means player ran before making any turns- we don't care 
                Debug.Log("this should only be happening when player is running away before making turns");
            }

        }
    }

    IEnumerator PostTurnsToServer(string json_string, bool clear)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

        form.AddField("turns", json_string);
        form.AddField("bldg_name", GameManager.instance.activeBldg_name);
        if (clear)
        {
            form.AddField("clear", 1);
        }
        else
        {
            form.AddField("clear", 0);
        }

        WWW www = new WWW(postTurnsURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            JsonData turnPostResultJson = JsonMapper.ToObject(www.text);
            if (turnPostResultJson[0].ToString() == "Success")
            {
                turnResultJsonString = ""; //using a string instead of a list. conversion from list>json was not going well on devices.
                                           //                turnResultList.Clear();
                if (clear)
                {
                    GameManager.instance.BuildingIsCleared(false); //if clear, proceed to victory screen, false indicates no survivors found (ever)
                }
            }
            else
            {
                Debug.Log(turnPostResultJson[1].ToString());
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    void Update ()
    {
        switch (battleState)
        {
            case (PerformAction.WAIT):
                if (turnList.Count > 0)
                {
                    battleState = PerformAction.SELECTACTION;
                }
                else if (boss.GetComponent<BossStateMachine>().curHP <= 0 )
                {
                    // end of the boss
                    Debug.Log("End boss fight called");
                    /*
                    int earned_wood = CalculateWoodEarned();
                    int earned_metal = CalculateMetalFound();
                    int earned_water = CalculateWaterFound();
                    int earned_food = CalculateFoodFound();
                    bool found_survivor = CalculateSurvivorFound();
                    */
                    DumpTurnsToServer(true);
                    battleState = PerformAction.COMPLETED;
                }
                else if (autoAttackIsOn && survivorTurnList.Count > 0)
                {
                    //continue auto attack
                    AttackButtonPressed();
                }
                else if (survivorTurnList.Count < 1 && turnList.Count < 1)
                {
                    //give NPC turns after Player has expended all their turns. 
                    ResetAllTurns();
                }
                break;
            case (PerformAction.SELECTACTION):
                GameObject performer = GameObject.Find(turnList[0].attacker);
                if (performer != null)
                {
                    if (turnList[0].type == "zombie")
                    {
                        BossStateMachine BSM = performer.GetComponent<BossStateMachine>();
                        BSM.target = turnList[0].TargetsGameObject;
                        BSM.bossTurnState = BossStateMachine.TurnState.TAKEACTION;

                    }
                    if (turnList[0].type == "survivor")
                    {
                        SurvivorStateMachine SSM = performer.GetComponent<SurvivorStateMachine>();
                        SSM.plyrTarget = turnList[0].TargetsGameObject;
                        SSM.currentState = SurvivorStateMachine.TurnState.ACTION;
                    }
                    battleState = PerformAction.COMPLETED;
                }
                else
                {
                    //attacker is dead, remove their turn, and go to wait mode
                    turnList.RemoveAt(0);
                    battleState = PerformAction.WAIT;
                }
                break;
            case (PerformAction.COMPLETED):

            break;
        }

        switch (playerGUI)
        {
            case (PlayerInput.ACTIVATE):
                if (survivorTurnList.Count > 0)
                {
                    survivorTurnList[0].transform.Find("arrow").gameObject.SetActive(true);
                    survivorTurnList[0].GetComponent<SurvivorStateMachine>().isSelected = true;
                    playerGUI = PlayerInput.WAITING;
                }
                break;
            case (PlayerInput.WAITING):
                //idle
                break;
            
        }

    }

    public void AutoAttackTogglePressed()
    {
        autoAttackIsOn = autoAttackToggle.GetComponent<Toggle>().isOn;
    }

    public void AttackButtonPressed()
    {
        TurnHandler myAttack = new TurnHandler();
        myAttack.attacker = survivorTurnList[0].name;
        myAttack.type = "survivor";
        myAttack.AttackersGameObject = survivorTurnList[0];
        myAttack.TargetsGameObject = boss; //always target the boss

        CollectAction(myAttack);
        survivorTurnList.RemoveAt(0);
    }

    public void CollectAction(TurnHandler myTurn)
    {
        turnList.Add(myTurn);
    }

    public void ResetAllTurns()
    {
        //clear and reset the lists
        turnList.Clear();
       
        survivorList.Clear();
        survivorList.AddRange(GameObject.FindGameObjectsWithTag("survivor"));


        //tell the boss to pick his turn
        BossStateMachine BossSM = boss.GetComponent<BossStateMachine>();
        BossSM.bossTurnState = BossStateMachine.TurnState.CHOOSEACTION;
        
        //let all the players add their turn to the list
        foreach (GameObject survivor in survivorList)
        {
            SurvivorStateMachine SSM = survivor.GetComponent<SurvivorStateMachine>();
            SSM.currentState = SurvivorStateMachine.TurnState.INITIALIZING;
        }

        battleState = PerformAction.WAIT;
    }

    public void SurvivorHasBeenBit(SurvivorStateMachine bitSurvivor)
    {
        //if it's player character, startup the end-game panel, otherwise just the survivor panel.
        GameObject[] battle_survivors = GameObject.FindGameObjectsWithTag("survivor");
        if (bitSurvivor.teamPos == 5 && battle_survivors.Length == 1)
        {
            //this is end game condition. the player character is bit.
            if (GameManager.instance.blazeOfGloryActive == true)
            {
                GameManager.instance.BuildingIsCleared(false);
                //this will launch the coroutine, which reacts to the string of active building, and notifies the server of game over, then loads game over scene
            }
            else
            {
                StartCoroutine(GameManager.instance.PlayerBit());
            }

            Debug.Log("PLAYER CHARACTER BIT!!!! END GAME SHOULD CALL HERE!~!!!");
        }
        else if (bitSurvivor.teamPos != 5)
        {
            //This is just a normal survivor dying.
            //stop the battlestate machine

            //we have to turn off all the zombie and player sprite renderers, or they will be on top of the panel, because of render order.
            foreach (GameObject survivor in survivorList)
            {
                survivor.GetComponent<SpriteRenderer>().enabled = false;
                survivor.GetComponent<SurvivorStateMachine>().DisableAllWeaponSprites();
            }
            boss.GetComponent<SpriteRenderer>().enabled = false;

            //update and show the panel.
            survivorBitText.text = bitSurvivor.survivor.name + " has been bit by the zombie!\nWhat will you do?";
            float timer = UnityEngine.Random.Range(4.0f, 9.5f);
            amputationButton.StartTheCountdown(timer);
            survivorBitPanel.SetActive(true);
            survivorWithBite = bitSurvivor;
        }
    }

    public void GameOverBiteCallback()
    {
        battleState = PerformAction.BITECASE;
        playerBitPanel.SetActive(true);
        KillYourselfButtonManager KYBM = KillYourselfButtonManager.FindObjectOfType<KillYourselfButtonManager>();
        float timer = UnityEngine.Random.Range(4.0f, 12.0f);
        KYBM.StartTheCountdown(timer);
    }

    public void SuccessfulCombatSuicide()
    {
        StartCoroutine(KillPlayerZombie());
    }

    public IEnumerator KillPlayerZombie()
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

        WWW www = new WWW(combatSuicideURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            JsonData json = JsonMapper.ToObject(www.text);

            if (json[0].ToString() == "Success")
            {
                GameManager.instance.playerIsZombie = false;
                SceneManager.LoadScene("03b Game Over");
            }
        }
        else
        {
            Debug.Log(www.error);
        }

    }

    public void SuccessfulAmputation()
    {

        //send ID to server to add the survivor to the injured list.
        int survivorIDamputated = survivorWithBite.survivor.survivor_id;
        StartCoroutine(SendInjuredSurvivorToServer(survivorIDamputated));

        //re-enable all the game objects, except the injured survivor
        GameObject destroyMe = null;
        foreach (GameObject survivor in survivorList)
        {
            survivor.GetComponent<SpriteRenderer>().enabled = true;
            survivor.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
            //destroy the gameobject of the survivor that has been injured in the scene
            if (survivor.GetComponent<SurvivorStateMachine>().survivor.survivor_id == survivorIDamputated)
            {
                destroyMe = survivor.gameObject;
            }
        }
        survivorTurnList.Remove(destroyMe);
        survivorList.Remove(destroyMe);
        Destroy(destroyMe);
        
        boss.GetComponent<SpriteRenderer>().enabled = true;
        

        //close the decision window & reset turns
        survivorBitPanel.SetActive(false);
        //ResetAllTurns(); //this is to ensure the inactive player is not falsely targeted by a zombie. 

        //instead of resetting all turns- go through the list, and remove the null record
        for (int i = 0; i < survivorList.Count; i++)
        {
            if (survivorList[i] == null)
            {
                survivorList.RemoveAt(i);
                break;
            }
        }


        //pop up text to notify player

        //resume combat
        //battleState = PerformAction.WAIT;
    }

    public void PlayerChooseKillSurvivor()
    {

        //send survivor ID to the server to destoy the record on the server.
        int survivorIDtoDestroy = survivorWithBite.survivor.survivor_id;
        StartCoroutine(SendDeadSurvivorToServer(survivorIDtoDestroy));

        //turn on all disabled survivors + zombies.
        GameObject destroyMe = null;
        foreach (GameObject survivor in survivorList)
        {
            survivor.GetComponent<SpriteRenderer>().enabled = true;
            survivor.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
            //destroy the gameobject of the survivor that has died in the scene
            if (survivor.GetComponent<SurvivorStateMachine>().survivor.survivor_id == survivorIDtoDestroy)
            {
                destroyMe = survivor.gameObject;
                break;
            }
        }
        survivorTurnList.Remove(destroyMe);
        survivorList.Remove(destroyMe);
        Destroy(destroyMe);

        boss.GetComponent<SpriteRenderer>().enabled = true;
        

        //destroy the survivor record on the GameManager.
        GameObject destroyMe2 = null;
        foreach (GameObject surv in GameManager.instance.activeSurvivorCardList)
        {
            if (surv.GetComponent<SurvivorPlayCard>().survivor.survivor_id == survivorIDtoDestroy)
            {
                destroyMe2 = surv;
            }
        }
        GameManager.instance.activeSurvivorCardList.Remove(destroyMe2);
        Destroy(destroyMe2);
        //remove from the survivorlist on battlestatemachine
        //look for a null entry on the survivorList on this object- remove it before resetting turns
        for (int i = 0; i < survivorList.Count; i++)
        {
            if (survivorList[i] == null)
            {
                survivorList.RemoveAt(i);
                break;
            }
        }

        //disable the survivor panel
        survivorBitPanel.SetActive(false);
        ResetAllTurns();

        //resume combat
        //battleState = PerformAction.WAIT;
    }

    //this handles a non-player survivor being bit
    public void PlayerChoosesToFightOn()
    {
        //destroy survivor on the server
        StartCoroutine(SendDeadSurvivorToServer(survivorWithBite.survivor.survivor_id));

        //notify survivor state machine
        survivorWithBite.BiteTimerStart();

        //re-enable the UI
        foreach (GameObject survivor in survivorList)
        {
            survivor.GetComponent<SpriteRenderer>().enabled = true;
            survivor.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
        }

        boss.GetComponent<SpriteRenderer>().enabled = true;
        

        //close window
        survivorBitPanel.SetActive(false);

        //resume combat
        battleState = PerformAction.WAIT;
    }

    //This handles the player character being bit
    public void PlayerChoosesToFightToTheBitterEnd()
    {
        //server has already been updated with the zombie status. 
        //changing these variables will allow combat to end, but GameOver/YouAreAZombie should load instead of a victory screen.
        GameManager.instance.activeBldg_name = "bite_case";
        GameManager.instance.playerIsZombie = true;
        playerBitPanel.SetActive(false);
        battleState = PerformAction.WAIT;
    }

    IEnumerator SendInjuredSurvivorToServer(int idInjured)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");
        form.AddField("survivor_id", idInjured);

        WWW www = new WWW(injuredSurvivorURL, form);
        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
        //ResetAllTurns();
    }

    IEnumerator SendDeadSurvivorToServer(int idToDestroy)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");
        form.AddField("survivor_id", idToDestroy);

        WWW www = new WWW(destroySurvivorURL, form);
        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void PlayerChoosePurchaseSurvivorSave()
    {
        int survIDtoRestore = survivorWithBite.survivor.survivor_id;
        //disable the bite panel
        survivorBitPanel.SetActive(false);
        //turn zombies and survivors back on
        foreach (GameObject surv in survivorList)
        {
            surv.SetActive(true);
            if (surv.GetComponent<SurvivorStateMachine>().survivor.survivor_id == survIDtoRestore)
            {
                //send full stam to server, and set in UI
                SurvivorStateMachine mySSM = surv.GetComponent<SurvivorStateMachine>();
                mySSM.survivor.curStamina = mySSM.survivor.baseStamina;
                mySSM.UpdateStaminaBar();
                StartCoroutine(SendRestoreSurvivorToServer(survIDtoRestore));
            }
        }
        boss.GetComponent<SpriteRenderer>().enabled = true;
        
        foreach (GameObject surv in survivorList)
        {
            surv.GetComponent<SpriteRenderer>().enabled = true;
            surv.GetComponent<SurvivorStateMachine>().UpdateWeaponSprite();
        }


        //resume combat
        battleState = PerformAction.WAIT;
    }

    IEnumerator SendRestoreSurvivorToServer(int idToRestore)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");
        form.AddField("survivor_id", idToRestore);

        WWW www = new WWW(restoreSurvivorURL, form);
        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void PlayerPartiallyWatchedAD()
    {
        //no stamina gain is assessed- no need to update server.
        survivorWithBite = null; //off the chopping block
        playerBitPanel.SetActive(false); //panel gone
        battleState = PerformAction.WAIT; // resume battle state machine
    }

    //this is called from the run away failed panel
    public void TurnAndFight()
    {
        failedRunAwayPanel.SetActive(false);
        battleState = PerformAction.WAIT;
    }

    //this call starts the coroutine to check 'run result' with the server, and load story elements upon load


    //this function decides the result of run-away in client, and does not load any story elements.
    public void PlayerChoosesRunAway()
    {

        DumpTurnsToServer(false); //send all stored turn data to server

        survivorList.RemoveAll(GameObject => GameObject == null);

        int got_away = 0;
        int running_away = survivorList.Count;

        foreach (GameObject survivor in survivorList)
        {
            float odds = 0;
            SurvivorStateMachine mySSM = survivor.GetComponent<SurvivorStateMachine>();
            float healthPercent = 100 * (mySSM.survivor.curStamina / mySSM.survivor.baseStamina);
            if (healthPercent < 10)
            {
                odds += 1;
            }
            if (mySSM.survivor.curStamina <= 0)
            {
                odds += 3;
            }
			if (mySSM.survivor.curStamina <= -1*mySSM.survivor.baseStamina) {
				odds = odds * 1.3f; 
			}
			if (mySSM.survivor.curStamina <= -2*mySSM.survivor.baseStamina) {
				odds = odds * 1.6f; 
			}
			if (mySSM.survivor.curStamina <= -3*mySSM.survivor.baseStamina) {
				odds = odds + 2.2f; 
			}

            float roll = UnityEngine.Random.Range(0.0f, 100.0f);
            if (roll <= odds)
            {
                //check that it was not the player character
                if (mySSM.teamPos == 5)
                {
                    GameObject[] battle_survivors = GameObject.FindGameObjectsWithTag("survivor");
                    if (battle_survivors.Length <= 1)
                    {
                        //if the player is the last one alive, then they died...
                        StartCoroutine(GameManager.instance.PlayerBit());
                    }
                    else
                    {
                        continue; //no game over, just go to next survivor
                    }
                }

                //this survivor has failed to run away.
                StartCoroutine(SendDeadSurvivorToServer(mySSM.survivor.survivor_id));
                failedToRunText.text = mySSM.survivor.name + " fell behind and was overcome by zombies. they didn't make it.\n what will you do?";
                failedRunAwayPanel.SetActive(true);
                ContinueRunningButtonManager myContRunButMgr = ContinueRunningButtonManager.FindObjectOfType<ContinueRunningButtonManager>();
                float tmr = UnityEngine.Random.Range(8.0f, 25.0f);
                myContRunButMgr.StartTheCountdown(tmr);
                break;
            }
            else
            {
                Debug.Log(mySSM.survivor.name + " successfully ran away.");
                got_away++;
                continue;
            }
        }

        //if everyone got away, go to map level. Otherwise the above loop has been broken with a dead survivor.
        if (got_away == running_away)
        {
            SceneManager.LoadScene("02a Map Level");
            StartCoroutine(GameManager.instance.LoadAllGameData());
        }
    }

   

    public void WeaponDestroyed(int surv_id)
    {
        brokenWep_surv_id = surv_id;
        battleState = PerformAction.BITECASE; //this is used to temporarily pause the BSM

        if (my_CWLP != null)
        {
            weaponBrokenPanel.SetActive(true);
            my_CWLP.PopulateUnequippedWeapons();
        }
        else
        {
            Debug.Log("Unable to locate Weapon list populator");
        }

    }

    public void DontEquipNewWeapon()
    {
        foreach (GameObject surv in survivorList)
        {
            SurvivorStateMachine my_SSM = surv.GetComponent<SurvivorStateMachine>();
            if (my_SSM != null)
            {
                my_SSM.UpdateWeaponSprite();
            }
            else
            {
                Debug.Log("No SurvivorStateMachine able to be located for " + surv.name);
            }
        }
        Time.timeScale = 1.0f; //resume normal speed
        weaponBrokenPanel.SetActive(false);
        battleState = PerformAction.WAIT;
    }

    public void EquipNewWeapon(int wep_id)
    {
        Time.timeScale = 1.0f;//resume normal speed
        StartCoroutine(EquipWeaponToSurvivor(brokenWep_surv_id, wep_id));
        Debug.Log("Sending weapon equip to server; surv ID: " + brokenWep_surv_id + " wep id: " + wep_id);
    }

    IEnumerator EquipWeaponToSurvivor(int survivor_id, int weapon_id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");
        form.AddField("survivor_id", survivor_id);
        form.AddField("weapon_id", weapon_id);

        WWW www = new WWW(equipWeaponURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            string serverString = www.text.ToString();
            JsonData weaponEquipJson = JsonMapper.ToObject(serverString);

            if (weaponEquipJson[0].ToString() == "Success")
            {
                Debug.Log(weaponEquipJson[1].ToString());
                GameManager.instance.weaponCardList.Clear();
                GameManager.instance.weaponCardList.AddRange(GameObject.FindGameObjectsWithTag("weaponcard"));


                //find the weapon to be equipped.
                foreach (GameObject weapon in GameManager.instance.weaponCardList)
                {
                    BaseWeapon my_wep = weapon.GetComponent<BaseWeapon>();
                    if (my_wep.weapon_id == weapon_id)
                    {
                        //find the survivor, and update and associate the objects in game data
                        my_wep.equipped_id = survivor_id;
                        foreach (GameObject survivor in survivorList)
                        {
                            SurvivorStateMachine my_surv = survivor.GetComponent<SurvivorStateMachine>();
                            if (my_surv.survivor.survivor_id == survivor_id)
                            {
                                my_surv.survivor.weaponEquipped = my_wep.gameObject;
                                my_surv.UpdateWeaponSprite();
                                weaponBrokenPanel.SetActive(false);
                                Debug.Log("New weapon sucessfully equipped");
                                battleState = PerformAction.WAIT;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                Debug.Log("Unable to associate weapon and survivor records");

            }
            else if (weaponEquipJson[0].ToString() == "Failed")
            {
                Debug.Log(weaponEquipJson[1].ToString());
            }


        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void PlayWeaponSound(BaseWeapon.WeaponType myWepType, string wepName, int dmg)
    {
        if (dmg == 0)
        {
            myAudioSource.PlayOneShot(missSound);
            return;//play the miss and exit
        }

        if (myWepType == BaseWeapon.WeaponType.KNIFE)
        {
            //play the knife stab
            myAudioSource.PlayOneShot(knifeSound);
        }
        else if (myWepType == BaseWeapon.WeaponType.CLUB)
        {
            //play the club swing
            myAudioSource.PlayOneShot(clubSound);
        }
        else if (myWepType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo > 0)
        {
            //play the correct gunshot noise
            if (wepName == "shotgun")
            {
                myAudioSource.PlayOneShot(shotgunSound);
            }
            else
            {
                myAudioSource.PlayOneShot(pistolSound);
            }
        }
        else if (myWepType == BaseWeapon.WeaponType.GUN && GameManager.instance.ammo < 1)
        {
            //play the swing noise
            myAudioSource.PlayOneShot(clubSound);
        }
    }

    public void PlayZombieAttackSound()
    {
        int audioPosition = UnityEngine.Random.Range(0, zombieSounds.Length - 1);
        myAudioSource.PlayOneShot(zombieSounds[audioPosition]);
    }

    public void PlayUnarmedSurvivorAttackSound()
    {
        int audioPosition = UnityEngine.Random.Range(0, zombieSounds.Length - 1);
        myAudioSource.PlayOneShot(survivorUnarmedSounds[audioPosition]);
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
