using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.Unity;
using LitJson;
using System;
using UnityEngine.Analytics;

public class LoginManager : MonoBehaviour {

	[SerializeField]
	//private Text loginPasswordText, loginEmailText, registerEmail, registerPassword, registerPassword2; // no longer used
	private int survivorsDrafted = 0;
    public bool runGameClock = false;
    private DateTime time_game_start;
    private bool confirm_new_char = true;

	public GameObject registrationPanel, loggedInPanel, survivorDraftPanel, newCharConfirmationPanel, aliveDeadChoicePanel, userDataObject;
    public Text currentGameClock, player_name, player_food, player_water, firstTimeText;
    public Image loginProfilePic, tmp_TestProfilePicBlobLoader;
	public Button continueButton;
	public IGraphResult fbFriendsResult;
	public JsonData staticSurvivorData, facebookSurvivorData;
	public SurvivorPlayCard[] survivorDraftCardArray;
	public Sprite genericSurvivorPortrait;

	private string newSurvivorUrl = GameManager.serverURL+"/create_new_survivor.php";
	//private string findUserAcctURL = GameManager.serverURL+"/UserAcctLookup.php";
	private string fetchStaticSurvivorURL = GameManager.serverURL+"/FetchStaticSurvivor.php";
	private string zombieStatusURL = GameManager.serverURL+"/GetZombieStatus.php";
    private string profileImageUploadURL = GameManager.serverURL + "/UploadProfileImage.php";
    private string playAsZombieURL = GameManager.serverURL + "/PlayAsZombie.php";

    // Use this for initialization
    void Start () { 
        if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
        //Handle FB.Init
            FB.Init(SetInit, OnHideUnity);
        }
        
        //if the game has already been loaded, then make sure the clock is running.
        if (GameManager.instance.gameDataInitialized == true)
        {
            runGameClock = true;
            loginProfilePic.sprite = GameManager.instance.my_profile_pic;
            player_name.text = GameManager.instance.userName;

            if (GameManager.instance.foodCount<1) {
				player_food.text = "0";
            }else{
				player_food.text = GameManager.instance.foodCount.ToString();
            }
            if(GameManager.instance.waterCount<1){
            	player_water.text = "0";
            }else{
				player_water.text = GameManager.instance.waterCount.ToString();
            }


        }
    }

    void Update() {
        if (runGameClock == true)
        {
        	//calculate the ACTUAL time since game started
            TimeSpan time_alive = (DateTime.Now - (GameManager.instance.timeCharacterStarted + GameManager.instance.serverTimeOffset));

            //reset the clock to the gating events
            if (GameManager.instance.firstSurvivorFound==false && time_alive >= TimeSpan.FromDays(2)) {
            	time_alive = TimeSpan.FromDays(2);
            }
			if(GameManager.instance.homebase_set==false && time_alive >= TimeSpan.FromDays(1)) {
            	time_alive = TimeSpan.FromDays(1);
            }
             
            //Debug.Log(time_alive.ToString());
            string my_string = "";

            //days
            if (time_alive > TimeSpan.FromDays(1))
            {
                int total_days = Mathf.FloorToInt((float)time_alive.TotalDays);
                my_string += total_days.ToString().PadLeft(2, '0')+" : ";
                time_alive = time_alive - TimeSpan.FromDays(total_days);
            }
            else
            {
                my_string += "00 : ";
            }
            //hours
            if (time_alive > TimeSpan.FromHours(1))
            {
                int tot_hrs = Mathf.FloorToInt((float)time_alive.TotalHours);
                my_string += tot_hrs.ToString().PadLeft(2, '0') + " : ";
                time_alive = time_alive - TimeSpan.FromHours((float)tot_hrs);
            }else
            {
                my_string += "00 : ";
            }
            //minutes
            if (time_alive > TimeSpan.FromMinutes(1))
            {
                int tot_min = Mathf.FloorToInt((float)time_alive.TotalMinutes);
                my_string += tot_min.ToString().PadLeft(2, '0') /*+ " : "*/;
                time_alive = time_alive - TimeSpan.FromMinutes((float)tot_min);
            }else
            {
                my_string += "00 ";
            }
            /*
            //seconds
            if (time_alive > TimeSpan.FromSeconds(1))
            {
                int tot_sec = Mathf.FloorToInt((float)time_alive.TotalSeconds);
                my_string += tot_sec.ToString().PadLeft(2, '0');
            }else
            {
                my_string += "00";
            }
            */

            currentGameClock.text = my_string;
            //Debug.Log(my_string);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
			//if the game data hasn't been loaded, then attempt to resume character automatically.
            if (GameManager.instance.gameDataInitialized) {
				Debug.Log("loaded Login screen with game data already initialized");
            } else {
            	if (FB.IsLoggedIn) {
                //This was intended to auto-load the game for players already logged in, however, it's just annoying, and I don't want to write the code to fix it 1/26/17
                //Debug.Log("Automatically attempting to resume game");
                //ResumeCharacter();
                Debug.Log("This spot USED TO cause an automatic load to map screen- NOW REMOVED to allow players to go between options+login screens smoothly");
				}
            }
    }

    public void LoadSettingsMenu ()
    {
        Debug.Log("Loading Options menu");
        SceneManager.LoadScene("01c Options");
    }

    public void ConfirmStartNewCharacter () {
    	if(newCharConfirmationPanel.activeInHierarchy) {
    		newCharConfirmationPanel.SetActive(false);
    	} else {
            if (confirm_new_char)
            {
                newCharConfirmationPanel.SetActive(true);
            }else
            {
                GameManager.instance.StartNewCharacter();
            }
            
    	}
    }

    //this is called automatically on mobile- as user is persistently logged in after 1st login
    void SetInit () {
        FB.ActivateApp();
        if (FB.IsLoggedIn) {
            Debug.Log ("FB is logged in- SetInit");
            
            //FB.API("me?fields=id,name,first_name,last_name,picture{height,width,url}", HttpMethod.GET, FBCoreCallback);
            
            
            //fetch the name and ID from the FB API.
            FB.API ("/me?fields=id", HttpMethod.GET, UpdateUserId);
		    FB.API ("/me?fields=first_name", HttpMethod.GET, UpdateUserFirstName);
		    FB.API ("/me?fields=last_name", HttpMethod.GET, UpdateUserLastName);
		    FB.API ("/me", HttpMethod.GET, UpdateUserName);
            
			FB.API ("me?fields=picture.width(200).height(200)", HttpMethod.GET, UpdateProfilePicURL);
            loggedInPanel.SetActive (true);

            Analytics.CustomEvent("Login-SetInit", new Dictionary<string, object> 
            {
            	{"Player ID", GameManager.instance.userId},
            	{"Username", GameManager.instance.userName},
            	{"timeStamp", DateTime.Now.ToString()}
            });

        } else {
            Debug.Log ("FB is not logged in");
            loggedInPanel.SetActive (false);
        }
        
    }
    
    void OnHideUnity (bool isGameShown) {
        
        if (!isGameShown) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }
    
    public void FBlogin ()  {
        
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        permissions.Add("user_friends");
        //permissions.Add("email");
        
        
        FB.LogInWithReadPermissions (permissions, AuthCallBack);
        
    }
    
    //this is only called after manual login
    void AuthCallBack (IResult result) {
        
        if (result.Error != null) {
            Debug.Log (result.Error);
        } else {
            
            if (FB.IsLoggedIn) {
                Debug.Log ("FB is logged in");
                loggedInPanel.SetActive (true);

                //FB.API("me?fields=id,name,first_name,last_name,picture{height,width,url}", HttpMethod.GET, FBCoreCallback);

                FB.API ("/me?fields=id", HttpMethod.GET, UpdateUserId);
		        FB.API ("/me?fields=first_name", HttpMethod.GET, UpdateUserFirstName);
		        FB.API ("/me?fields=last_name", HttpMethod.GET, UpdateUserLastName);
				FB.API ("/me", HttpMethod.GET, UpdateUserName);
                
				FB.API ("me?fields=picture.width(200).height(200)", HttpMethod.GET, UpdateProfilePicURL);

				//
				Analytics.CustomEvent("Login-AuthCallback", new Dictionary<string, object> 
            	{
            		{"Player ID", GameManager.instance.userId},
            		{"Username", GameManager.instance.userName},
            		{"timeStamp", DateTime.Now.ToString()}
            	});

            } else {
                Debug.Log ("FB is NOT logged in");
                loggedInPanel.SetActive (false);
            }
         
        }

    }

    //checks if player is a zombie/dead
    IEnumerator CheckZombieStatus() {

        if (GameManager.instance.userId == "")
        {
            Debug.Log("preventing blank user id from being sent to the server");
            yield break;
        }
    	
    	WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", "12/31/1999 11:59:59");
		form.AddField("client", "mob");

		WWW www = new WWW(zombieStatusURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData zombStatJson = JsonMapper.ToObject(www.text);

			if (zombStatJson[0].ToString() == "Success") {
				int stat = (int)zombStatJson[1];

                if (stat == 0)
                {
                    
                    //problem on first login: no char created date time causes parse error -1/11/17
                    if (zombStatJson[2]["char_created_DateTime"].ToString() != "")
                    {
                        //player is alive, and has a character active on server
                        Debug.Log("player is alive, and has an active character on the server");
                        
                        //SET CLIENT TIME OFFSET
                        GameManager.instance.serverTimeOffset = DateTime.Now - DateTime.Parse(zombStatJson[3]["NOW()"].ToString());
                        Debug.Log("Server offset calculated to be: " + GameManager.instance.serverTimeOffset);

						


                        //Set up the clock for active character.
                        string zombStatJsonString = zombStatJson[2]["char_created_DateTime"].ToString();
                        if (zombStatJsonString != "") {

                            time_game_start = DateTime.Parse(zombStatJsonString);
							GameManager.instance.timeCharacterStarted = time_game_start;

                        }else
                        {
                            confirm_new_char = false;//there is no previous character, we can bypass the confirmation panel on "start new"
                        }

                        

                        GameManager.instance.timeCharacterStarted = time_game_start;
                        runGameClock = true;
                        currentGameClock.gameObject.SetActive(true);

                        //set up the name, stats, and pic
                        userDataObject.SetActive(true);
                        player_name.text = GameManager.instance.userName;
						int food = (int)zombStatJson[2]["food"];
						if(food<1){
                       	 	player_food.text = "0";
                       	}else{
                       		player_food.text = food.ToString();
                       	}
						int water = (int)zombStatJson[2]["water"];
                       	if(water<1){
                        	player_water.text = "0";
                        }else{
                        	player_water.text = water.ToString();
                        }
                        loginProfilePic.sprite = GameManager.instance.my_profile_pic;

                        continueButton.interactable = true;


                        //TEMPORARY!!! ATTEMPT TO LOAD PROFILE IMAGE FROM PHP RETURN
                        /*
                        Texture2D server_image = new Texture2D(200,200) ;
                         zombStatJson[2]["profile_image_blob"].ToString() 
                        server_image.LoadImage();
                        */
                    }
                    else
                    {
                        //blank created date/time means first login. Setup the login panel.
                        runGameClock = false;
                        currentGameClock.gameObject.SetActive(false);
                        continueButton.interactable = false;
                        userDataObject.SetActive(false);


                        // TODO: POST A TEXT OBJECT INFORMING FIRST TIME PLAYERS TO START A NEW CHARACTER.
                        firstTimeText.gameObject.SetActive(true);

                    }
				} else if (stat == 1) {
					Debug.Log("Player is a zombie ==> force loading game over scene");
                    //must load start and end times into game manager before loading Game over scene.
                    GameManager.instance.timeCharacterStarted = DateTime.Parse(zombStatJson[2]["char_created_DateTime"].ToString());
                    string end_time = zombStatJson[2]["game_over_datetime"].ToString();
                    if (end_time != "")
                    {
                        GameManager.instance.gameOverTime = DateTime.Parse(end_time);
                    } else
                    {
                        Debug.Log("Your death did not store a date-time properly.");
                    }
					GameManager.instance.playerIsZombie = true;
					GameManager.instance.zm_zombieHordeCount = int.Parse(zombStatJson[2]["zm_hordeCount"].ToString());
                    SceneManager.LoadScene("02b Zombie Mode");
                    //SceneManager.LoadScene("03b Game Over");
				} else if (stat == 2) {
					Debug.Log("Player is dead, but not a zombie");
                    currentGameClock.gameObject.SetActive(false);
                    userDataObject.SetActive(false);
					continueButton.interactable = false;
				} else {
					Debug.Log("Zombie Check callback returned invalid status code");
				}

			} else if (zombStatJson[0].ToString() == "Failed") {
				Debug.Log(zombStatJson[1].ToString());
				continueButton.interactable = false;
			}

			
		} else {
			Debug.Log(www.error);
		}
    }

    //once logged in, we fetch the core data, and this is the callback function
    private void FBCoreCallback(IResult result)
    {
        if (result.Error == null)
        {
            //load the user data into GameManager
            GameManager.instance.userId = result.ResultDictionary["id"].ToString();
            GameManager.instance.userFirstName = result.ResultDictionary["first_name"].ToString();
            GameManager.instance.userLastName = result.ResultDictionary["last_name"].ToString();
            GameManager.instance.userName = result.ResultDictionary["name"].ToString();
            /*
            string rawResult = result.RawResult;
            JsonData picJson = JsonMapper.ToObject(rawResult);
            GameManager.instance.myProfilePicURL = picJson["picture"]["data"]["url"].ToString();
            StartCoroutine(SetPlayerProfilePic(picJson["picture"]["data"]["url"].ToString()));
            */
            StartCoroutine(CheckZombieStatus()); //this should be the only place we call this, as it is contained in the main FB callback function
        }
        else
        {
            Debug.Log(result.Error);
        }
    }
    
	private void UpdateUserId (IResult result) {
		if (result.Error == null) {
            GameManager.instance.userId = result.ResultDictionary["id"].ToString();
        } else {
            Debug.Log (result.Error);
        }

		//ping the server for forced zombie status
		StartCoroutine(CheckZombieStatus()); //first login bug checking
	}

	private void UpdateUserFirstName(IResult result) {
		if (result.Error == null) {
			GameManager.instance.userFirstName = result.ResultDictionary["first_name"].ToString();
		} else {
			Debug.Log (result.Error);
		}
	}

	private void UpdateUserLastName (IResult result) {
		if (result.Error == null) {
			GameManager.instance.userLastName = result.ResultDictionary["last_name"].ToString();
		}else{
			Debug.Log(result.Error);
		}
	}

	private void UpdateUserName (IResult result) {
		if(result.Error == null) {
			GameManager.instance.userName = result.ResultDictionary["name"].ToString();
		}else {
			Debug.Log(result.Error);
		}
	}

	private void UpdateProfilePicURL (IResult result) {
		if (result.Error == null) {
			string rawResult = result.RawResult;
			JsonData picJson = JsonMapper.ToObject(rawResult);

			GameManager.instance.myProfilePicURL = picJson["picture"]["data"]["url"].ToString();

            StartCoroutine(SetPlayerProfilePic(picJson["picture"]["data"]["url"].ToString()));
		}else{
			Debug.Log(result.Error);
		}
        //StartCoroutine(CheckZombieStatus());
	}

    IEnumerator SetPlayerProfilePic (string pic_url)
    {
        WWW www = new WWW(pic_url);
        yield return www;

        if (www.error == null)
        {
            Debug.Log("setting player Profile Pic");
            int height = www.texture.height;
            int width = www.texture.width;

            var bytes = www.texture.EncodeToPNG();
            Debug.Log("Picture Encoded into PNG reads bytes out as: " + bytes.ToString());
            GameManager.instance.my_profile_pic = Sprite.Create(www.texture, new Rect(0, 0, width, height), new Vector2());
            GameManager.instance.profile_image_texture = www.texture;
            //we will verify this is the correct image when we load into the map level- along with loading all other survivor images.
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void SendPlayerProfilePicToServer ()
    {
        StartCoroutine(UpdateMyProfileImageOnTheServer());
    }

    IEnumerator UpdateMyProfileImageOnTheServer()
    {
        yield return new WaitForEndOfFrame();
        //declare the texture we're going to use
        var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0); //this take a screenshot of the entire screen
        //tex.ReadPixels(loginProfilePic.rectTransform.rect, 0, 0); //this attempts to just take the rect of the player profile picture.
        tex.Apply();
        var bytes = tex.EncodeToPNG()/*GameManager.instance.my_profile_pic.texture.EncodeToPNG()*/;
        Debug.Log("******************************<<<<<<<<<<<<<<<Encoding profile image to binary:" + bytes.ToString());

        GameManager.instance.lastLoginTime = "12/31/1999 11:59:59";
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

        form.AddField("action", "upload image");
        form.AddBinaryData("profileImage", bytes, GameManager.instance.userId + ".png");
        Debug.Log("attempting to send player profile pic to the server as a BLOB " + bytes.ToString());

        //upload to the server
        WWW www = new WWW(profileImageUploadURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            Debug.Log("no web error reported from upload");
            JsonData jsonReturn = JsonMapper.ToObject(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    private void UpdateSurvivorDraftWindow (IGraphResult result) {
		if (result.Error == null) {
			//store the data object for later use in next friend being updated.
			fbFriendsResult = result;
			Debug.Log(result.ToString());
			string data = result.RawResult as string;
			Debug.Log(data);
			JsonData jsonData = JsonMapper.ToObject(data);
			facebookSurvivorData = jsonData;

			/*
			//fill the player data into the play card objects on the draft window.
			for (int i=0; i< jsonData.Count; i++) {
				//set the name from the result.
				survivorDraftCardArray[i].survivor.name = jsonData[i]["name"].ToString();
				// roll and load random stats
				int stam = Random.Range(90, 140);
				survivorDraftCardArray[i].survivor.baseStamina = stam;
				survivorDraftCardArray[i].survivor.curStamina = stam;
				int attk = Random.Range(9, 25);
				survivorDraftCardArray[i].survivor.baseAttack = attk;

				//get and update the photo
				Image survivorPic = survivorDraftCardArray[i].profilePic;
				string imgUrl = jsonData[i]["picture"]["data"]["url"].ToString();
				WWW www = new WWW(imgUrl);
				survivorPic.sprite = Sprite.Create(www.texture, new Rect(0,0,200,200), new Vector2());

				//update the text field.
				string myText = "";
				myText += "name: " + survivorDraftCardArray[i].survivor.name.ToString()+"\n";
				myText += "stamina: " + survivorDraftCardArray[i].survivor.baseStamina.ToString()+"\n";
				myText += "attack: " +survivorDraftCardArray[i].survivor.baseAttack.ToString(); 
				survivorDraftCardArray[i].displayText.text = myText;
			}
			*/

			StartCoroutine(FetchStaticSurvivors());

		}else{
			Debug.Log(result.Error);
		}
	}

	IEnumerator FetchStaticSurvivors() {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		WWW www = new WWW(fetchStaticSurvivorURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
			JsonData staticSurvData = JsonMapper.ToObject(www.text);

			staticSurvivorData = staticSurvData;

			LoadNextSetofSurvivorsToSelect();
		} else {
			Debug.Log(www.error);
		}
	}

	//This retains the total number selected between refreshes, so we know to quit at 12 (3 slots x 4 picks)
	public int survivorSelectCounter = 0;
	private void LoadNextSetofSurvivorsToSelect () {
		int facebookSurvivorCount = facebookSurvivorData["data"].Count;

		//cycle through the core loop 3 times loading new data in each position
		for (int i = 0; i < 3; i++) {
			if (survivorSelectCounter >= facebookSurvivorCount) {
				//we've selected all the FB returns already, use static entries.
				int fbAdjustedCounter = survivorSelectCounter - facebookSurvivorCount;

				survivorDraftCardArray[i].survivor.name = staticSurvivorData[1][fbAdjustedCounter]["name"].ToString();
				survivorDraftCardArray[i].survivor.baseAttack = (int)staticSurvivorData[1][fbAdjustedCounter]["base_attack"];
				survivorDraftCardArray[i].survivor.baseStamina = (int)staticSurvivorData[1][fbAdjustedCounter]["base_stam"];
				survivorDraftCardArray[i].survivor.curStamina = (int)staticSurvivorData[1][fbAdjustedCounter]["base_stam"];
				survivorDraftCardArray[i].profilePicURL = staticSurvivorData[1][fbAdjustedCounter]["profile_pic_url"].ToString();
				if (4-survivorsDrafted >= 0) {
					survivorDraftCardArray[i].team_pos = 4-survivorsDrafted;
				} else {
					survivorDraftCardArray[i].team_pos = 0;
				}

				//for now leave the generic sprite- later I can add custom sprites to load for each character.
				//Image survivorPic = survivorDraftCardArray[i].profilePic;
				//survivorPic.sprite = genericSurvivorPortrait;

				//update the UI text
				string myText = "";
				myText += "name: " + survivorDraftCardArray[i].survivor.name.ToString()+"\n";
				myText += "stamina: " + survivorDraftCardArray[i].survivor.baseStamina.ToString()+"\n";
				myText += "attack: " +survivorDraftCardArray[i].survivor.baseAttack.ToString(); 
				survivorDraftCardArray[i].displayText.text = myText;
				if (survivorDraftCardArray[i].profilePicURL != "") {
					StartCoroutine(SetSurvivorImageFromURL(survivorDraftCardArray[i].profilePicURL, i));
				}

			} else {
				//we have FB entries still available.
				//roll for the random stats
				int stam = UnityEngine.Random.Range(90, 140);
				int attk = UnityEngine.Random.Range(9, 25);
				//set up the survivor play card data
				survivorDraftCardArray[i].survivor.name = facebookSurvivorData["data"][survivorSelectCounter]["name"].ToString();
				survivorDraftCardArray[i].survivor.baseAttack = attk;
				survivorDraftCardArray[i].survivor.baseStamina = stam;
				survivorDraftCardArray[i].survivor.curStamina = stam;
				survivorDraftCardArray[i].profilePicURL = facebookSurvivorData["data"][survivorSelectCounter]["picture"]["data"]["url"].ToString();
				Debug.Log(facebookSurvivorData["data"][survivorSelectCounter]["picture"]["data"]["url"].ToString());
				if (4-survivorSelectCounter >= 0) {
					survivorDraftCardArray[i].team_pos = 4-survivorSelectCounter;
				} else {
					survivorDraftCardArray[i].team_pos = 0;
				}

				//fetch and update the image
				string imgURL = facebookSurvivorData["data"][survivorSelectCounter]["picture"]["data"]["url"].ToString();
				StartCoroutine(SetSurvivorImageFromURL(imgURL, i));


				//update the UI text
				string myText = "";
				myText += "name: " + survivorDraftCardArray[i].survivor.name.ToString()+"\n";
				myText += "stamina: " + survivorDraftCardArray[i].survivor.baseStamina.ToString()+"\n";
				myText += "attack: " +survivorDraftCardArray[i].survivor.baseAttack.ToString(); 
				survivorDraftCardArray[i].displayText.text = myText;
			}

			survivorSelectCounter++;
		}

	}

	IEnumerator SetSurvivorImageFromURL(string URL, int arrayPos) {
				WWW www = new WWW(URL);
				yield return www;

				Image survivorPic = survivorDraftCardArray[arrayPos].profilePic;	
				survivorPic.sprite = Sprite.Create(www.texture, new Rect(0, 0, 200, 200), new Vector2());
	}


	//this is a temporary function to test sending characters to the server.  eventually these choices will be auto-populated from friends, and cycle choices on each pick- creating a Zombie Apocalypse Draft.
	public bool sendInProgress = false;
	public void ChooseSurvivorToSend (int arrayPos) {
		if(sendInProgress == false){
			sendInProgress = true;
			//load the name and stats from the correct survivor
			string nm = survivorDraftCardArray[arrayPos].survivor.name;
			int stam = survivorDraftCardArray[arrayPos].survivor.baseStamina;
			int attk = survivorDraftCardArray[arrayPos].survivor.baseAttack;
			int team_pos = survivorDraftCardArray[arrayPos].team_pos;
			string pic_url = survivorDraftCardArray[arrayPos].profilePicURL;

			StartCoroutine(SendNewSurvivorToServer(nm, stam, attk, team_pos, pic_url));
		} 
	}

	IEnumerator SendNewSurvivorToServer (string name, int stamina, int attack, int teamPosition, string picture_url) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("team_position", teamPosition); //this will need to actually pull
		form.AddField("name", name);
		form.AddField("base_stam", stamina);
		form.AddField("curr_stam", stamina);
		form.AddField("base_attack", attack);
		form.AddField("picture_url", picture_url);
		form.AddField("weapon_equipped", "none");

		WWW www = new WWW(newSurvivorUrl, form);
		yield return www;
		if (www.error == null) {
			Debug.Log(www.text);
			string jsonReturn = www.text.ToString();
			JsonData jsonResult = JsonMapper.ToObject(jsonReturn);

			if(jsonResult[0].ToString() == "Success") {
				survivorsDrafted++;
				Debug.Log(jsonResult[1].ToString());
				if (survivorsDrafted >= 4) {
					GameManager.instance.ResumeCharacter();
					SceneManager.LoadScene("04a Weapon Select");
				} else {
					LoadNextSetofSurvivorsToSelect();
				}
			} else {
				//failed to add survivor
				Debug.Log(jsonResult[1].ToString());
			}

		}else{
			Debug.Log(www.error);
		}
		//regardless of succcess- you are ready to send another survivor up.
		sendInProgress = false;
	}


/*
	public void RegisterAccount () {
		if (registerPassword.ToString() != registerPassword2.ToString()) {
			Debug.Log ("Passwords do not match- not submitting to server");
		} else {
			StartCoroutine(Register());
		}
	}

	IEnumerator Register () {
		WWWForm form = new WWWForm();
		//form.AddField("register", "register");
		form.AddField("email", registerEmail.text);
		form.AddField("password", registerPassword.text);
		WWW www = new WWW(registerUrl, form);
		yield return www;

		//error check
		if (www.error == null) {
			Debug.Log ("WWW OK! result: " + www.text);

			if (www.text == "email not valid") {
				//server returned invalid email
			} else if (www.text == "email address is already registered") {
				//server returned email already registered
			} else if (www.text == "account successfully created") {
				Debug.Log ("server successfully created new account");
				ToggleRegistrationPanel();
				//will also have to log user in
			}
		}else{
			Debug.Log ("WWW error: " + www.error);
		}
	}

	public void LoginCheck () {
		StartCoroutine(Login());
	}

	IEnumerator Login () {
		WWWForm form = new WWWForm();
		//form.AddField("register", "register");
		form.AddField("email", loginEmailText.text);
		form.AddField("password", loginPasswordText.text);
		WWW www = new WWW(loginUrl, form);
		yield return www;

		if (www.error == null) {
			Debug.Log ("WWW OK! result: " + www.text);

			if (www.text == "email not valid") {
				//server returned invalid email
			} else if (www.text == "Incorrect email or password") {
				//server returned email already registered
			} else if (www.text == "successfully logged in") {
				Debug.Log ("You have successfully logged in");
				FakeLoggedInSuccess();//this just activates the panel for character new/continue
			}
		}else{
			Debug.Log ("WWW error: " + www.error);
		}
	}
*/
	
	public void ToggleRegistrationPanel () {
		if (registrationPanel.activeInHierarchy == false) {
			registrationPanel.SetActive(true);
		} else if (registrationPanel.activeInHierarchy == true) {
			registrationPanel.SetActive(false);
		}
	}

	public void FakeLoggedInSuccess () {
		if (loggedInPanel.activeInHierarchy == false){
			loggedInPanel.gameObject.SetActive(true);

			if (FB.IsLoggedIn == true) {
				GameManager.instance.ResumeCharacter();

			}

		} else {
			loggedInPanel.gameObject.SetActive(false);
		}

	}

	public void ResumeCharacter () {
			GameManager.instance.ResumeCharacter();



			//SceneManager.LoadScene ("02a Map Level");
	}

	public void StartNewCharacter () {
		//this should warn the  player that they are going to erase old game data, and verify before executing

        //removed this to remove the draft from the beginning of the game.
        /*
		survivorDraftPanel.SetActive(true);
        FB.API("me/friends?fields=name,picture.width(200).height(200)", HttpMethod.GET, UpdateSurvivorDraftWindow);
        */
        GameManager.instance.StartNewCharacter();
			
	}

    public void ToggleAliveDeadChoicePanel()
    {
        if (aliveDeadChoicePanel.activeInHierarchy)
        {
            aliveDeadChoicePanel.SetActive(false);
        }
        else
        {
            aliveDeadChoicePanel.SetActive(true);
        }
    }

    public void PlayAsZombie()
    {
        //just start the coroutine 
        StartCoroutine(StartZombieGame());
    }

    IEnumerator StartZombieGame()
    {
        GameManager.instance.lastLoginTime = "12/31/1999 11:59:59";
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

		Debug.Log ("Sending request to: "+ playAsZombieURL);
        WWW www = new WWW(playAsZombieURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            //parse json
            JsonData jsonResult = JsonMapper.ToObject(www.text);

            if (jsonResult[0].ToString() == "Success")
            {
                Debug.Log("Player has successfully started a game as a zombie");
                GameManager.instance.playerIsZombie = true;
				GameManager.instance.zm_zombieHordeCount = int.Parse(jsonResult[1]["zm_hordeCount"].ToString());
                SceneManager.LoadScene("02b Zombie Mode");

            }
            else if (jsonResult[0].ToString() == "Failed")
            {
                Debug.Log(jsonResult[1].ToString());
            }
        }
        else
        {
            Debug.LogWarning(www.error);
        }

    }

}
