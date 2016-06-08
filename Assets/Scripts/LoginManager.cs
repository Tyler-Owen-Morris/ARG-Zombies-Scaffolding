using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using Facebook.Unity;

public class LoginManager : MonoBehaviour {

	[SerializeField]
	private Text loginPasswordText, loginEmailText, registerEmail, registerPassword, registerPassword2;

	public GameObject registrationPanel, loggedInPanel;

	private string registerUrl = "http://localhost/ARGZ_SERVER/register.php";
	private string playerDataUrl = "http://localhost/ARGZ_SERVER/PlayerData.php";
	private string loginUrl = "http://localhost/ARGZ_SERVER/login.php";
	
	// Use this for initialization
	void Start () { 
        if (FB.IsInitialized) {
            FB.ActivateApp();
        } else {
        //Handle FB.Init
            FB.Init(SetInit, OnHideUnity);
        }
        
        
    }
    
    void SetInit () {
        FB.ActivateApp();
        if (FB.IsLoggedIn) {
            Debug.Log ("FB is logged in");
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
            } else {
                Debug.Log ("FB is NOT logged in");
                loggedInPanel.SetActive (false);
            }
            
        }
        
    }
    
	private void UpdateUserId (IResult result) {
		if (result.Error == null) {
            GameManager.instance.userId = result.ResultDictionary["id"].ToString();
        } else {
            Debug.Log (result.Error);
        }
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

	public void FetchPlayerData () {
		StartCoroutine(NoFormWWWCall());
	}

	IEnumerator NoFormWWWCall () {
		WWW playerData = new WWW(playerDataUrl);
		yield return playerData;
		string playerDataString = playerData.text;
		print (playerDataString);
	}

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
			GameManager.instance.StartNewCharacter();
			
	}
}
