using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.Unity;
using LitJson;

public class LoginManager : MonoBehaviour {

	[SerializeField]
	private Text loginPasswordText, loginEmailText, registerEmail, registerPassword, registerPassword2;
	private int survivorsDrafted = 0;

	public GameObject registrationPanel, loggedInPanel, survivorDraftPanel, newCharConfirmationPanel;
	public Button continueButton;
	public IGraphResult fbFriendsResult;
	public JsonData staticSurvivorData, facebookSurvivorData;
	public SurvivorPlayCard[] survivorDraftCardArray;
	public Sprite genericSurvivorPortrait;



//	private string registerUrl = "http://localhost/ARGZ_SERVER/register.php";
//	private string playerDataUrl = "http://localhost/ARGZ_SERVER/PlayerData.php";
//	private string loginUrl = "http://localhost/ARGZ_SERVER/login.php";

	private string newSurvivorUrl = GameManager.serverURL+"/create_new_survivor.php";
	private string findUserAcctURL = GameManager.serverURL+"/UserAcctLookup.php";
	private string fetchStaticSurvivorURL = GameManager.serverURL+"/FetchStaticSurvivor.php";
	private string zombieStatusURL = GameManager.serverURL+"/GetZombieStatus.php";
	
	// Use this for initialization
	void Start () { 
        if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
        //Handle FB.Init
            FB.Init(SetInit, OnHideUnity);
        }
        
        
    }

    void OnLevelWasLoaded () {
			//if the game data hasn't been loaded, then attempt to resume character automatically.
            if (GameManager.instance.gameDataInitialized) {
				Debug.Log("Why didn't this register as true?!?");
            } else {
            	if (FB.IsLoggedIn) {
            		Debug.Log("Automatically attempting to resume game");
            		ResumeCharacter();
				}
            }
    }

    public void ConfirmStartNewCharacter () {
    	if(newCharConfirmationPanel.activeInHierarchy) {
    		newCharConfirmationPanel.SetActive(false);
    	} else {
    		newCharConfirmationPanel.SetActive(true);
    	}
    }

    
    void SetInit () {
        FB.ActivateApp();
        if (FB.IsLoggedIn) {
            Debug.Log ("FB is logged in");

            //fetch the name and ID from the FB API.
			FB.API ("/me?fields=id", HttpMethod.GET, UpdateUserId);
		    FB.API ("/me?fields=first_name", HttpMethod.GET, UpdateUserFirstName);
		    FB.API ("/me?fields=last_name", HttpMethod.GET, UpdateUserLastName);
		    FB.API ("/me", HttpMethod.GET, UpdateUserName);
			FB.API ("me?fields=picture.width(200).height(200)", HttpMethod.GET, UpdateProfilePicURL);

            loggedInPanel.SetActive (true);

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
        permissions.Add("email");
        
        
        FB.LogInWithReadPermissions (permissions, AuthCallBack);
        
    }
    
    void AuthCallBack (IResult result) {
        
        if (result.Error != null) {
            Debug.Log (result.Error);
        } else {
            
            if (FB.IsLoggedIn) {
                Debug.Log ("FB is logged in");
                loggedInPanel.SetActive (true);
                FB.API ("/me?fields=id", HttpMethod.GET, UpdateUserId);
		        FB.API ("/me?fields=first_name", HttpMethod.GET, UpdateUserFirstName);
		        FB.API ("/me?fields=last_name", HttpMethod.GET, UpdateUserLastName);
				FB.API ("/me", HttpMethod.GET, UpdateUserName);
				FB.API ("me?fields=picture.width(200).height(200)", HttpMethod.GET, UpdateProfilePicURL);

            } else {
                Debug.Log ("FB is NOT logged in");
                loggedInPanel.SetActive (false);
            }
         
        }

    }

    //checks if player is a zombie/dead
    IEnumerator CheckZombieStatus() {
    	
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
	
				if (stat == 0) {
					//player is alive, and has a character active on server
					Debug.Log("player is alive, and has an active character on the server");
					continueButton.interactable = true;
				} else if (stat == 1) {
					Debug.Log("Player is a zombie ==> force loading game over scene");
					GameManager.instance.playerIsZombie = true;
					SceneManager.LoadScene("03b Game Over");
				} else if (stat == 2) {
					Debug.Log("Player is dead, but not a zombie");
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
    
	private void UpdateUserId (IResult result) {
		if (result.Error == null) {
            GameManager.instance.userId = result.ResultDictionary["id"].ToString();
        } else {
            Debug.Log (result.Error);
        }

		//ping the server for forced zombie status
		StartCoroutine(CheckZombieStatus());
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
		}else{
			Debug.Log(result.Error);
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
				survivorDraftCardArray[i].profilePicURL = "";
				if (4-survivorsDrafted >= 0) {
					survivorDraftCardArray[i].team_pos = 4-survivorsDrafted;
				} else {
					survivorDraftCardArray[i].team_pos = 0;
				}

				//for now leave the generic sprite- later I can add custom sprites to load for each character.
				Image survivorPic = survivorDraftCardArray[i].profilePic;
				survivorPic.sprite = genericSurvivorPortrait;

				//update the UI text
				string myText = "";
				myText += "name: " + survivorDraftCardArray[i].survivor.name.ToString()+"\n";
				myText += "stamina: " + survivorDraftCardArray[i].survivor.baseStamina.ToString()+"\n";
				myText += "attack: " +survivorDraftCardArray[i].survivor.baseAttack.ToString(); 
				survivorDraftCardArray[i].displayText.text = myText;

			} else {
				//we have FB entries still available.
				//roll for the random stats
				int stam = Random.Range(90, 140);
				int attk = Random.Range(9, 25);
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


		survivorDraftPanel.SetActive(true);
		FB.API("me/friends?fields=name,picture.width(200).height(200)", HttpMethod.GET, UpdateSurvivorDraftWindow);
		GameManager.instance.StartNewCharacter();
			
	}
}
