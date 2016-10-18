using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KillYourselfButtonManager : MonoBehaviour {

	public Slider mySlider;
	public Button myButton;
	public BattleStateMachine myBSM;

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

	public void KillYourself () {
		float odds = 80.0f + (20.0f * mySlider.value); // value of 0 gives 80% odds 1.0 gives 100%



		float roll = Random.Range(0.0f, 100.0f);
		if (roll > odds) {
			//We need to notify the battlestate maschine that we can kill the players zombie (move isZombie from 1 to 2- allowing for restart, and stopping zombie lockout)
			myBSM.SuccessfulCombatSuicide();
			Debug.Log("Successful amputation");
		} else {
			//failed suicide, ur a zombie now
			//NOTE: Before the player is given this option, they are stored as a zombie on the server, and all final scores are tallied. we just need to load game over scene. the core data updater should have updated the GameManager boolean for isZombie to be set to true
			// this should notify the game over level manager to go straight into zombie mode.
			UnityEngine.SceneManagement.SceneManager.LoadScene("03b Game Over");
		}

	}

	void Update () {
		if (countdownTimer > 0 ) {
			countdownTimer = countdownTimer - Time.deltaTime;
			mySlider.value = countdownTimer/totalTime;
		} else if (countdownStarted==true) {
			//set slider value to 0, and disable button.
			mySlider.value = 0;
			myButton.interactable = false;
			countdownStarted = false;
		}
	}
}
