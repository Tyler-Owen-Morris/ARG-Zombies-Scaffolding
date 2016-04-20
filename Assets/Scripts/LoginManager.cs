using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour {

	[SerializeField]
	private Text loginPasswordText, loginEmailText, registerEmail, registerPassword, registerPassword2;

	[SerializeField]
	private GameObject registrationPanel, loggedInPanel;

	private string registerUrl = "http://localhost/ARGZ_SERVER/register.php";
	private string playerDataUrl = "http://localhost/ARGZ_SERVER/PlayerData.php";
	private string loginUrl = "http://localhost/ARGZ_SERVER/login.php";
	
	// Use this for initialization
	void Start () {
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
		} else {
			loggedInPanel.gameObject.SetActive(false);
		}

	}

	public void ResumeCharacter () {
			GameManager.instance.ResumeCharacter();
			SceneManager.LoadScene ("01b Start");
	}

	public void StartNewCharacter () {
			GameManager.instance.StartNewCharacter();
			SceneManager.LoadScene ("01b Start");
	}
}
