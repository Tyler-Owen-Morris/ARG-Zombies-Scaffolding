using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
		myBSM.PlayerChoosesRunAway();
		myPanel.SetActive(false);
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
