using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameOverLevelManager : MonoBehaviour {

	public GameObject zombieQRpanel;
	public Text myScoreText;

	void Awake () {
		//pop the zombie panel above the UI before doing anything, if the player is a zombie
		if (GameManager.instance.playerIsZombie) {
			zombieQRpanel.SetActive(true);
		} else {
			zombieQRpanel.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () {
		string myTextString = "You have lasted: \n\n";
		if (GameManager.instance.my_score > TimeSpan.FromDays(1)) {
			int total_days = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalDays);
			myTextString += total_days.ToString()+" Days";
			GameManager.instance.my_score = GameManager.instance.my_score-TimeSpan.FromDays(total_days);
		}
		if (GameManager.instance.my_score > TimeSpan.FromHours(1)) {
			int total_hours = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalHours);
			myTextString += total_hours.ToString().PadLeft(2, '0')+" Hr";
			GameManager.instance.my_score = GameManager.instance.my_score-TimeSpan.FromDays(total_hours);
		} else {
			myTextString += "00 Hr";
		}
		if (GameManager.instance.my_score > TimeSpan.FromMinutes(1)) {
			int total_minutes = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalMinutes);
			myTextString += total_minutes.ToString().PadLeft(2, '0')+" Min";
			GameManager.instance.my_score = GameManager.instance.my_score-TimeSpan.FromDays(total_minutes);
		} else {
			myTextString += "00 Min";
		}
		if (GameManager.instance.my_score > TimeSpan.FromSeconds(1)) {
			int total_seconds = (int)Mathf.Floor((float)GameManager.instance.my_score.TotalSeconds);
			myTextString += total_seconds.ToString().PadLeft(2, '0')+" sec";
		} else {
			myTextString += "00 sec";
		}
		myScoreText.text = myTextString;
	}

	public void MainMenuPressed () {
		SceneManager.LoadScene("01a Login");
	}
	
	public void StartOverPressed () {
		StartCoroutine(AttemptAtTrickyBullshit());
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
