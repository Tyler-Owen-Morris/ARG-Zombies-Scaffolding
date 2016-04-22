using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLevelManager : MonoBehaviour {

	[SerializeField]
	private GameObject inventoryPanel;

	[SerializeField]
	private Text supplyText, daysAliveText, survivorsAliveText, shivCountText, clubCountText, gunCountText;

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
}
