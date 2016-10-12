﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreenController : MonoBehaviour {

	[SerializeField]
	private Text text;

	public Button returnToMapButton, abandonTheSurvivorButton, recruitTheSurvivorButton;

	private string destroySurvivorURL = GameManager.serverURL+"/DestroySurvivor.php";

	void Awake () {
		returnToMapButton.gameObject.SetActive(true);
		abandonTheSurvivorButton.gameObject.SetActive(false);
		recruitTheSurvivorButton.gameObject.SetActive(false);
	}

	// Use this for initialization
	void OnLevelWasLoaded () {
		text.text = ConstructWinningTextString();
	}

	string ConstructWinningTextString () {
		string outputText = "";

		outputText += "You earned " + GameManager.instance.reportedSupply + " supply\n";
		outputText += GameManager.instance.reportedFood + " Food\n";
		outputText += GameManager.instance.reportedWater + " water.\n\n";


		if(GameManager.instance.survivorFound){
			returnToMapButton.gameObject.SetActive(false);
			abandonTheSurvivorButton.gameObject.SetActive(true);
			recruitTheSurvivorButton.gameObject.SetActive(true);
			outputText += "\n You found "+GameManager.instance.foundSurvivorName;
			outputText += "\nthey have "+GameManager.instance.foundSurvivorAttack+" attack and "+GameManager.instance.foundSurvivorMaxStam+" stamina";
			outputText += "\nWhat will you do?";
		}

		/*
		if (GameManager.instance.reportedTotalSurvivor == 0) {
			return outputText;
		} else {
			
			outputText += "you found " + GameManager.instance.reportedTotalSurvivor + " people alive.\n";
			if (GameManager.instance.reportedActiveSurvivor == GameManager.instance.reportedTotalSurvivor) {
				outputText += "they are all able bodied, and join you gladly.";
			} else {
				outputText += "but " + (GameManager.instance.reportedTotalSurvivor - GameManager.instance.reportedActiveSurvivor) + " can't fight." ;
			}

		}
		*/
		return outputText;
	}

	public void AbandonTheSurvivor () {
		SendDeadSurvivorToServer(GameManager.instance.foundSurvivorEntryID);
	}

	IEnumerator SendDeadSurvivorToServer(int idToDestroy) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("survivor_id", idToDestroy);

		WWW www = new WWW(destroySurvivorURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			Debug.Log(www.text);
			SceneManager.LoadScene("02a Map Level");
		}else {
			Debug.Log(www.error);
		}
	}
}
