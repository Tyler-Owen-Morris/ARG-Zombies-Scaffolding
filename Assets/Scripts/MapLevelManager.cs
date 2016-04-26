using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLevelManager : MonoBehaviour {

	[SerializeField]
	private GameObject inventoryPanel;

	[SerializeField]
	private Text supplyText, daysAliveText, survivorsAliveText, shivCountText, clubCountText, gunCountText, locationReportText;

	[SerializeField]
	private Slider playerHealthSlider, playerHealthSliderDuplicate;



	public void InventoryButtonPressed () {
		if (inventoryPanel.activeInHierarchy == false ) {
			inventoryPanel.SetActive(true);
		} else if (inventoryPanel.activeInHierarchy == true) {
			inventoryPanel.SetActive(false);
		}
	}

	public void PlayerAttemptingPurchaseFullHealth () {
		GameManager.instance.PlayerAttemptingPurchaseFullHealth();
	}

	public void PlayerAttemptingPurchaseNewSurvivor () {
		GameManager.instance.PlayerAttemptingPurchaseNewSurvivor ();
	}

	public void PlayerAttemptingPurchaseShiv () {
		GameManager.instance.PlayerAttemptingPurchaseShiv();
	}

	public void PlayerAttemptingPurchaseClub () {
		GameManager.instance.PlayerAttemtpingPurchaseClub();
	}

	public void PlayerAttemptingPurchaseGun () {
		GameManager.instance.PlayterAttemtptingPurchaseGun();
	}


	public void ResetBuildingsCalled () {
		GameManager.instance.ResetAllBuildings();
	}

	void OnLevelWasLoaded () {
		UpdateTheUI();
	}

	void Start () {
		
	}

	public void UpdateTheUI () {
		//left UI panel update
		supplyText.text = "Supply: " + GameManager.instance.supply.ToString();
		daysAliveText.text = GameManager.instance.daysSurvived.ToString();
		survivorsAliveText.text = "Survivors: " + GameManager.instance.survivorsActive.ToString();

		//inventory panel text updates
		shivCountText.text = GameManager.instance.shivCount.ToString();
		clubCountText.text = GameManager.instance.clubCount.ToString();
		gunCountText.text = GameManager.instance.gunCount.ToString();

		//duplicate health slider updates
		playerHealthSlider.value = (CalculatePlayerHealthSliderValue());
		playerHealthSliderDuplicate.value = (CalculatePlayerHealthSliderValue());

	}

	private float CalculatePlayerHealthSliderValue (){
		float health = GameManager.instance.playerCurrentHealth;
		float value = health / 100.0f; 
		Debug.Log ("Calculating the players health slider value to be " + value );
		return value;//the number 100 is a plceholder for total health possible.
	}


	//This is to attach directly to the drop/resupply button, and verify user is within proximity of their homebase.
	//later it should return a boolean, and if true, start the coroutine to send the transaction signal to the server, and get updated results.
	//for now we're just going to change the button color to confirm it works with current GPS coordinates
	public void AmIHomeTest () {
		
		//We are going to start with testing if stored homebase is within a certain range 
		// the phone appears to return 5 decimal places.  We are going to try 3 of the smallest unit as a range test

		float range = 0.00003f;

		if (Input.location.status == LocationServiceStatus.Running) {

			if ( Input.location.lastData.longitude >= (GameManager.instance.homebaseLong - range)  && Input.location.lastData.longitude <= (GameManager.instance.homebaseLong + range)  ) {

				if ( Input.location.lastData.latitude >= (GameManager.instance.homebaseLat - range)  && Input.location.lastData.latitude <= (GameManager.instance.homebaseLat + range) ) {
					//player is within range of home, and transaction can take place
					StartCoroutine(PostTempLocationText("CONFIRMED"));

				}else {
					Debug.Log ("Lattitude does not fall within range of homebase");
					StartCoroutine(PostTempLocationText("You are not in range of homebase (long)"));
				}


			}else {
				Debug.Log ("Logitude does not fall within range of homebase");
				StartCoroutine(PostTempLocationText("You are not in range of homebase (long)"));
			}

		} else {
			Debug.Log ("location services not running");
			StartCoroutine(PostTempLocationText("location services not running"));
		}

	}

	IEnumerator PostTempLocationText (string text) {
		locationReportText.text = text;
		yield return new WaitForSeconds (5);
		locationReportText.text = "";
	}
}
