using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System;
using UnityEngine.SceneManagement;

public class GameOverLevelManager : MonoBehaviour {

	private string zombieStatusURL = GameManager.serverURL+"/GetZombieStatus.php";
    private string twitterUploadURL = "https://api.twitter.com/1.1/statuses/update.json";
    private string twitterNoAuthPostURL = "https://api.twitter.com/1.1/statuses/update_with_media.json?status=I've%20died%20and%20become%20a%20Zombie%20in%20%23ARGZombies%20-Will%20someone%20please%20%23killmyzombie%3F";

    public GameObject zombieQRpanel;
	public Text myScoreText, myZombieText;
    public Texture2D screenie;
	public String[] stillZombieTextArray;

	void Awake () {
		//pop the zombie panel above the UI before doing anything, if the player is a zombie
		if (GameManager.instance.playerIsZombie) {
			zombieQRpanel.SetActive(true);
		} else {
			zombieQRpanel.SetActive(false);
		}

		GameManager.instance.my_score = GameManager.instance.gameOverTime - GameManager.instance.timeCharacterStarted;
	}

	// Use this for initialization
	void Start () {
		string myTextString = "You have lasted: \n\n";
		if (GameManager.instance.my_score > TimeSpan.FromDays(1)) {
			int total_days = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalDays);
			myTextString += total_days.ToString()+" Days ";
			GameManager.instance.my_score = GameManager.instance.my_score-TimeSpan.FromDays(total_days);
		}
		if (GameManager.instance.my_score > TimeSpan.FromHours(1)) {
			int total_hours = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalHours);
			myTextString += total_hours.ToString().PadLeft(2, '0')+" Hr ";
			GameManager.instance.my_score = GameManager.instance.my_score-TimeSpan.FromDays(total_hours);
		} else {
			myTextString += "00 Hr ";
		}
		if (GameManager.instance.my_score > TimeSpan.FromMinutes(1)) {
			int total_minutes = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalMinutes);
			myTextString += total_minutes.ToString().PadLeft(2, '0')+" Min ";
			GameManager.instance.my_score = GameManager.instance.my_score-TimeSpan.FromDays(total_minutes);
		} else {
			myTextString += "00 Min ";
		}
		if (GameManager.instance.my_score > TimeSpan.FromSeconds(1)) {
			int total_seconds = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalSeconds);
			myTextString += total_seconds.ToString().PadLeft(2, '0')+" sec ";
		} else {
			myTextString += "00 sec ";
		}
		myScoreText.text = myTextString;
	}

	public void MainMenuPressed () {
		SceneManager.LoadScene("01a Login");
	}
	
	public void StartOverPressed () {
		StartCoroutine(AttemptAtTrickyBullshit());
	}

	private bool query_active = false;
	private bool text_active = false;
	public void RequestZombieClose () {
		if (query_active == false) {
			query_active = true;
			InvokeRepeating("CheckToCloseZombie", 0.0f, 15.0f);
		}
		if (text_active == false) {
			text_active = true;
			StartCoroutine(StillAZombieTextUpdater());
		}
	}

	IEnumerator StillAZombieTextUpdater () {
		int pos = UnityEngine.Random.Range(0, stillZombieTextArray.Length-1);
		myZombieText.text = stillZombieTextArray[pos];
		myZombieText.gameObject.SetActive(true);
		yield return new WaitForSeconds(3.0f);
		myZombieText.gameObject.SetActive(false);
		text_active = false;
	}

	void CheckToCloseZombie () {
		StartCoroutine(ZombieChecker());
	}

	IEnumerator ZombieChecker() {
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
					Debug.Log("player is alive, and has an active character on the server, since we're in the zombie scene, and they haven't been allowed to restart, this is broken");

				} else if (stat == 1) {
					Debug.Log("Player is still a zombie, checking again in 15 seconds");

				} else if (stat == 2) {
					Debug.Log("Someone has successfully killed this player's zombie!");
					GameManager.instance.playerIsZombie = false;
					zombieQRpanel.SetActive(false);
				} else {
					Debug.Log("Zombie Check callback returned invalid status code");
				}

			} else if (zombStatJson[0].ToString() == "Failed") {
				Debug.Log(zombStatJson[1].ToString());
				Debug.Log("This implies that the player entry was deleted from the server... WTF, how are we on the zombie scene tho?");
			}

			
		} else {
			Debug.Log(www.error);
		}
	}

    public void TweetMyDeath ()
    {
        StartCoroutine(TweetScreenshot());
    }

    IEnumerator TweetScreenshot() {
        //we should only read off the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenie = tex;
        //read contents into rect
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        //encode texture into jpg
        var bytes = tex.EncodeToJPG();
        Destroy(tex);

        WWWForm form = new WWWForm();
        form.AddBinaryData("", bytes,  "-zombieBarcode.jpg");
        form.AddField("media", "media");

        //WWW www = new WWW(twitterUploadURL, form);
        WWW www = new WWW(twitterNoAuthPostURL, form);
        yield return www;
        Debug.Log(www.text);

        
    }

	//good news, tricky bullshit works
	IEnumerator AttemptAtTrickyBullshit () {
		DontDestroyOnLoad(this.gameObject);
		SceneManager.LoadScene("01a Login");
		yield return new WaitForSeconds(0.2f);
		LoginManager myLoginManager = GameObject.Find("Login Manager").GetComponent<LoginManager>();
		myLoginManager.StartNewCharacter();
		Destroy(this.gameObject);
	}
}
