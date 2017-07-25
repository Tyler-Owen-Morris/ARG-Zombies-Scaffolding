using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class ContinueRunningButtonManager : MonoBehaviour {

	public Slider mySlider;
	public Button myButton;
	public BattleStateMachine myBSM;
	public GameObject myPanel;

	private float countdownTimer;
	private float totalTime;
	private bool countdownStarted = false;

	public void StartTheCountdown (float time) {
		totalTime = time;
		countdownTimer = time;
		mySlider.value = 1.0f;
		myButton.interactable = true;
		countdownStarted = true;
	}

	public void KeepRunning () {
		string choice = "Keep Running";
		Analytics.CustomEvent("Survivor Run- failed", new Dictionary<string, object>
			{
				{"userID", GameManager.instance.userId},
				{"bldg_name", GameManager.instance.activeBldg_name},
				{"bldg_id", GameManager.instance.activeBldg_id},
				{"player_choice", choice},
				{"time_alive", GameManager.instance.GetCurrentTimeAlive()}
			});

		myBSM.PlayerChoosesRunAway();
		myPanel.SetActive(false);
	}

	public void TurnAndFight () {

		string choice = "Turn And Fight";
		Analytics.CustomEvent("Survivor Run- failed", new Dictionary<string, object>
			{
				{"userID", GameManager.instance.userId},
				{"bldg_name", GameManager.instance.activeBldg_name},
				{"bldg_id", GameManager.instance.activeBldg_id},
				{"player_choice", choice},
				{"time_alive", GameManager.instance.GetCurrentTimeAlive()}
			});

		GameObject parent = gameObject.GetComponentInParent<GameObject> ();
		parent.SetActive (false);
	}

	void Update () {
		if (countdownTimer > 0 ) {
			countdownTimer = countdownTimer - Time.deltaTime;
			mySlider.value = countdownTimer/totalTime;
		} else if (countdownStarted==true) {
			//if the timer goes off, disable the panel, resume combat, and stop the timer.
			mySlider.value = 0;
			myPanel.SetActive(false);
			myBSM.battleState = BattleStateMachine.PerformAction.WAIT;
			countdownStarted = false;
		}
	}
}
