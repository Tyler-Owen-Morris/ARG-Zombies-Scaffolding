using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using LitJson;

public class MapLevelManager : MonoBehaviour {

	[SerializeField]
	private GameObject inventoryPanel, buildingPanel, qrPanel, homebasePanel;

	[SerializeField]
	private Text supplyText, daysAliveText, survivorsAliveText, shivCountText, clubCountText, gunCountText, currentLatText, currentLonText, locationReportText, foodText, waterText, playerNameText, bldgNameText, zombiePopText, homebaseLatText, homebaseLonText;

	[SerializeField]
	private Slider playerHealthSlider, playerHealthSliderDuplicate;

	private BuildingSpawner bldgSpawner;

	private int zombieCount;
	private string bldgName;
	private string updateHomebaseURL = "http://www.argzombie.com/ARGZ_SERVER/UpdateHomebaseLocation.php";
	private string dropoffAndResupplyURL = "http://www.argzombie.com/ARGZ_SERVER/DropoffAndResupply.php";

	public void InventoryButtonPressed () {
		if (inventoryPanel.activeInHierarchy == false ) {
			inventoryPanel.SetActive(true);
		} else if (inventoryPanel.activeInHierarchy == true) {
			inventoryPanel.SetActive(false);
		}
	}

	public void HomebaseButtonPressed () {
		if (homebasePanel.activeInHierarchy == false ) {
			homebasePanel.SetActive(true);
		} else if (homebasePanel.activeInHierarchy == true) {
			homebasePanel.SetActive(false);
		}
	}

	public void QrScanPressed () {
		if (qrPanel.activeInHierarchy == true) {
			qrPanel.SetActive(false);
		} else {
			qrPanel.SetActive(true);
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
    
    void Awake () {
    	if (FB.IsLoggedIn == true)  {
        	FB.API ("/me?fields=name", HttpMethod.GET, DisplayUsername);
        } else {
        	string name;
        	name = GameManager.instance.userFirstName + " " + GameManager.instance.userLastName;
        	playerNameText.text = name;
        }
        bldgSpawner = GameObject.Find("Building Populator").GetComponent<BuildingSpawner>();
    }

	void OnLevelWasLoaded () {
		UpdateTheUI();
	}

	public void UpdateTheUI () {
        
        
		//left UI panel update
		supplyText.text = "Supply: " + GameManager.instance.supply.ToString();
        foodText.text = "Food: " + GameManager.instance.foodCount.ToString();
        waterText.text = "Water: " + GameManager.instance.waterCount.ToString();
		daysAliveText.text = GameManager.instance.daysSurvived.ToString();
		survivorsAliveText.text = "Survivors: " + GameManager.instance.survivorsActive.ToString();
        

		//inventory panel text updates
		shivCountText.text = GameManager.instance.shivCount.ToString();
		clubCountText.text = GameManager.instance.clubCount.ToString();
		gunCountText.text = GameManager.instance.gunCount.ToString();

		//homebase panel updates
		homebaseLatText.text = "Homebase Latitude: "+GameManager.instance.homebaseLat.ToString();
		homebaseLonText.text = "Homebase Longitude: "+GameManager.instance.homebaseLong.ToString();

		//duplicate health slider updates
		playerHealthSlider.value = (CalculatePlayerHealthSliderValue());
		playerHealthSliderDuplicate.value = (CalculateSurvivorStamina());

		bldgSpawner.PlaceHomebaseGraphic();
        StartCoroutine(SetCurrentLocationText());
	}

	public void ActivateBuildingInspector(int zombiesInBldg, string buildingName) {

		//this sets text, stores the int/string, and activates the panel.
		bldgName = buildingName;
		bldgNameText.text = buildingName;
		zombieCount = zombiesInBldg;
		int rand = Random.Range(1,3);
		zombiePopText.text = "It looks like there are about "+(zombiesInBldg-rand)+"-"+(zombiesInBldg+rand)+" zombies in the way";
		buildingPanel.SetActive(true);

	}

	public void DeactivateBuildingInspector () {
		buildingPanel.SetActive(false);

	}

	public void LoadIntoCombatFromBldgInspector () {
		GameManager.instance.LoadIntoCombat(zombieCount, bldgName);
	}
    
    void DisplayUsername (IResult result) {
        
        if (result.Error == null) {
            playerNameText.text = result.ResultDictionary["name"].ToString();
        } else {
            Debug.Log (result.Error);
        }
        
    }
    
    //this is to get the last location data coroutine, it's part of updating the UI.
    IEnumerator SetCurrentLocationText () {
        if (Input.location.status == LocationServiceStatus.Running) {
            yield return Input.location.lastData;
            currentLatText.text = "Current Latitude: " + Input.location.lastData.latitude.ToString();
            currentLonText.text = "Current Longitude: " + Input.location.lastData.longitude.ToString();
        } else {
            Debug.Log ("Location services not running- can't finish UI update");
        }
    }

	private float CalculatePlayerHealthSliderValue (){
		float health = GameManager.instance.playerCurrentHealth;
		float value = health / 100.0f; 
		//Debug.Log ("Calculating the players health slider value to be " + value );
		return value;//the number 100 is a plceholder for total health possible.
	}

	private float CalculateSurvivorStamina () {
		int totalMaxStam = 0;
		int totalCurrStam = 0;
		foreach (GameObject survivor in GameManager.instance.survivorCardList) {
			SurvivorPlayCard SurvPlayCard = survivor.GetComponent<SurvivorPlayCard>();
			totalMaxStam += SurvPlayCard.survivor.baseStamina;
			totalCurrStam += SurvPlayCard.survivor.curStamina;
			//Debug.Log ("adding "+SurvPlayCard.survivor.baseStamina+" to base of stam counter, and "+SurvPlayCard.survivor.curStamina+" to the current stamina");
		}
		float value = (float)totalCurrStam/(float)totalMaxStam;
		//Debug.Log ("The value of the slider has been calculated to be: "+value);
		return value;
	}

	//This is to attach directly to the drop/resupply button, and verify user is within proximity of their homebase.
	//later it should return a boolean, and if true, start the coroutine to send the transaction signal to the server, and get updated results.
	//for now we're just going to change the button color to confirm it works with current GPS coordinates
	public void AmIHomeTest () {
		if (Input.location.status == LocationServiceStatus.Running) {
			//calculate the distance from current location to stored home location.
			float myLat = Input.location.lastData.latitude;
			float myLon = Input.location.lastData.longitude;
			float homeLat = GameManager.instance.homebaseLat;
			float homeLon = GameManager.instance.homebaseLong;

			float latMid = (myLat + homeLat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			double deltaLatitude = (myLat - homeLat);
			double deltaLongitude = (myLon - homeLon);
			double latDistMeters = deltaLatitude * m_per_deg_lat;
			double lonDistMeters = deltaLongitude * m_per_deg_lon;
			double directDistMeters = Mathf.Sqrt(Mathf.Pow((float)latDistMeters, 2f)+Mathf.Pow((float)lonDistMeters, 2f));

			if (directDistMeters <= 100.0f) {
				//player is within range of home- attempt to drop all supply to homebase client.
				StartCoroutine(DropoffAndResupply());

			} else {
				StartCoroutine(PostTempLocationText("You are more than 100m from home"));
			}

		} else {
			Debug.Log ("location services not running");
			StartCoroutine(PostTempLocationText("location services not running"));
		}

	}

	public void CalculateAndPostDistanceToHome () {
		if (Input.location.status == LocationServiceStatus.Running) {
			float myLat = Input.location.lastData.latitude;
			float myLon = Input.location.lastData.longitude;
			float homeLat = GameManager.instance.homebaseLat;
			float homeLon = GameManager.instance.homebaseLong;

			float latMid = (myLat + homeLat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			double deltaLatitude = (myLat - homeLat);
			double deltaLongitude = (myLon - homeLon);
			double latDistMeters = deltaLatitude * m_per_deg_lat;
			double lonDistMeters = deltaLongitude * m_per_deg_lon;
			double directDistMeters = Mathf.Sqrt(Mathf.Pow((float)latDistMeters, 2f)+Mathf.Pow((float)lonDistMeters, 2f));

			string myText = "You are "+(float)directDistMeters+" meters from your homebase";
			Debug.Log (myText);
			StartCoroutine(PostTempLocationText(myText));

		} else {
			Debug.Log ("Location services not running");
			StartCoroutine(PostTempLocationText("Location Services Not Running"));
		}
	}

	public void SetNewHomebaseLocation () {
		StartCoroutine(UpdateHomebaseLocation());
	}

	IEnumerator UpdateHomebaseLocation () {
		float newLat = 0f;
		float newLon = 0f;
		if (Input.location.status == LocationServiceStatus.Running) {
			newLat = Input.location.lastData.latitude;
			newLon = Input.location.lastData.longitude;
		} else {
			Debug.Log("Location services not running. Attempting to update the server with Dummy data");
			newLat = 37.80897f;
			newLon = -122.4292f;
		}

		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("lat", newLat.ToString());
		form.AddField("lon", newLon.ToString());

		WWW www = new WWW(updateHomebaseURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			string jsonString = www.text;
			JsonData replyJson = JsonMapper.ToObject(jsonString);

			if (replyJson[0].ToString() == "Success") {
				Debug.Log (replyJson[1].ToString());
				GameManager.instance.homebaseLat = newLat;
				GameManager.instance.homebaseLong = newLon;
			} else {
				//this will handle any error responses from the server when attempting to set new homebase
				//I expect there will be a time delay, or cost associated with multiple changes, so this  is
				//where I expect the exctpetions to go.
			}
		} else {
			Debug.Log(www.error);
		}
		UpdateTheUI();
	}


	// this coroutine currently only handles dropoff, but is planned to handle pickup as well in the future.
	IEnumerator DropoffAndResupply () {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("supply", GameManager.instance.supply);

		WWW www = new WWW(dropoffAndResupplyURL ,form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			string dropoffReturnString = www.text;
			JsonData dropoffJson = JsonMapper.ToObject(dropoffReturnString);

			if (dropoffJson[0].ToString() == "Success") {
				GameManager.instance.supply = 0;
				UpdateTheUI();
				StartCoroutine(PostTempLocationText("Dropoff Success!"));
			} else if (dropoffJson[0].ToString() == "Failed") {
				StartCoroutine(PostTempLocationText(dropoffJson[1].ToString()));
				Debug.Log(dropoffJson[1].ToString());
			}

		} else {
			Debug.Log("www error: "+ www.error);
		}
	}

	private bool textPosted = false;

	IEnumerator PostTempLocationText (string text) {
		if (textPosted == false) {
			textPosted = true;
			locationReportText.gameObject.SetActive(true);
			locationReportText.text = text;
			yield return new WaitForSeconds (5);
			locationReportText.text = "";
			locationReportText.gameObject.SetActive(false);
			textPosted = false;
		} else {
			yield break;
		}
	}
}
