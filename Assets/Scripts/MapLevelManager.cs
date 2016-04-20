using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLevelManager : MonoBehaviour {

	[SerializeField]
	private GameObject inventoryPanel;

	[SerializeField]
	private Text supplyText, daysAliveText, survivorsAliveText;

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

	public void ResetBuildingsCalled () {
		GameManager.instance.ResetAllBuildings();


	}

	void OnLevelWasLoaded () {
		UpdateTheUI();
	}

	void Start () {
		
	}

	public void UpdateTheUI () {
		supplyText.text = "Supply: " + GameManager.instance.supply.ToString();
		daysAliveText.text = GameManager.instance.daysSurvived.ToString();
		survivorsAliveText.text = "Survivors: " + GameManager.instance.survivorsActive.ToString();
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
