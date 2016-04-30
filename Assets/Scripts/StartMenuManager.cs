using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour {

	[SerializeField]
	private Text daysSurvived, activeSurvivors, membersAlive, homebaseLat, homebaseLong, supply, food, water, foodExpire, waterExpire;

	//private GameManager gameManager;
	
	void Awake () {
		
		//gameManager = FindObjectOfType<GameManager>();
	}

	void OnLevelWasLoaded () {
		InvokeRepeating("UpdateTheText", 0, 5.0f);
	}

	void UpdateTheText () {
		daysSurvived.text = ( Mathf.Floor (GameManager.instance.daysSurvived)).ToString();
		activeSurvivors.text = "Active Survivors: " + GameManager.instance.survivorsActive.ToString();
		membersAlive.text = "Total Living Survivors: " + GameManager.instance.totalSurvivors.ToString();
		supply.text = "Supply: " + GameManager.instance.supply.ToString();
        food.text = "Food: " + GameManager.instance.foodCount.ToString();
        water.text = "Water: " + GameManager.instance.waterCount.ToString();
        UpdateExpireCounters ();
        
		if (homebaseLat != null && homebaseLong != null) {
			homebaseLat.text = "lat: " + GameManager.instance.homebaseLat.ToString();
			homebaseLong.text = "long: " + GameManager.instance.homebaseLong.ToString();
		}
	}
    
    void UpdateExpireCounters () {
        int food = GameManager.instance.foodCount;
        int water = GameManager.instance.waterCount;
        
        int foodExpireInMeals = (int)Mathf.Floor( food / GameManager.instance.totalSurvivors);
        int waterExpireInMeals = (int)Mathf.Floor( water / GameManager.instance.totalSurvivors);
        
        //2 meals/day means we divide by 2 to convert to days. later this may need to link to a variable.
        foodExpire.text = (foodExpireInMeals / 2).ToString();
        waterExpire.text = (waterExpireInMeals / 2).ToString();
    }

	public void SetHomeLocation () {
		float lat = Input.location.lastData.latitude;
		float lon = Input.location.lastData.longitude;

		GameManager.instance.SetHomebaseLocation(lat, lon);
		//gameManager.homebaseLat = LocationInfo.latitude;
		//gameManager.homebaseLong = LocationInfo.longitude;
		UpdateTheText();
	}
    
    public void AddOneHourToTimePlayed () {
        GameManager.instance.AddTimePlayed();
    }
}
