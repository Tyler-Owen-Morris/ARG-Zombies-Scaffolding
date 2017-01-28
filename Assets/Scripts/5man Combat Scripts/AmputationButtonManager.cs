using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AmputationButtonManager : MonoBehaviour {

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

	public void ChopLimb () {
		float odds = 10.0f + (60.0f * mySlider.value); // value of 0 gives 10% odds 1.0 gives 70%

		float roll = Random.Range(0.0f, 100.0f);
		if (roll > odds) {
			//player is added to the injured roster and removed from combat
			myBSM.SuccessfulAmputation();
			Debug.Log("Successful amputation");
		} else {
			//failed amputation. Player dies, and zombies go +1
			Debug.Log("unsuccessful amputation");
            //GameManager.instance.zombiesToFight++;
            GameManager.instance.activeBldg_zombies++;
			myBSM.UpdateUINumbers();
			myBSM.PlayerChooseKillSurvivor();
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
