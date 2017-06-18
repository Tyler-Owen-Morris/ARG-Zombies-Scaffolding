using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class ZombieModeManager : MonoBehaviour {

    
    public GameObject zombieQRpanel, zombieAdPanel, buildingPanel, buildingSpawnerObject;
    public Text ad_countText, qrPanelText, bldgDistanceText, bldgNameText, hordeCountText;
    public int adsToRevive, zombieHorde, brainsEaten, metersPerZombieGather;
    public string[] zStatusFailedStrings, zStatusProcessingStrings;
	public ZM_BuildingSpawner buildingSpawner;
	public BrainSpawner brainSpawner;

	private float lastUpdateLat = 0.0f, lastUpdateLng = 0.0f, lastZombieGatherLat =0, lastZombieGatherLng=0;
    private int ads_watched = 0;

	private string zombieUpdateURL = GameManager.serverURL + "/GatherZombies.php";
	private string zombieStatusURL = GameManager.serverURL + "/GetZombieStatus.php";
	private string brainEatingURL = GameManager.serverURL + "/BrainsEaten.php";

    void Start ()
    {
		zombieHorde = GameManager.instance.zm_zombieHordeCount;
        UpdateAdCountText();
		buildingSpawner = buildingSpawnerObject.GetComponent<ZM_BuildingSpawner> ();
		if (buildingSpawner != null) {
			buildingSpawner.UpdateBuildings ();
		}
		InvokeRepeating ("CheckAndUpdateMap", 10.0f, 10.0f);
		InvokeRepeating ("GatherZombies", 30f, 60f);
		UpdateTheUI ();
    }

	void UpdateTheUI () {
		hordeCountText.text = zombieHorde.ToString ();
	}

    private bool requestInProgress = false;
    public void RequestToRevive ()
    {
        if (requestInProgress)
        {
            int num = Random.Range(0, zStatusProcessingStrings.Length);//minus 1 to convert to index #
            int index = num - 1;
            if (index < 0)
            {
                index = 0;
            }
            StartCoroutine(PostTempQRPanelText(zStatusProcessingStrings[index], 3.0f));
            return;
        }else
        {
            StartCoroutine(ZombieChecker());
        }
    }

    public void BrainEaten (GameObject brain) {
    	brainsEaten ++;
    	Destroy(brain);

    	StartCoroutine(EatenBrain());
    }

    IEnumerator EatenBrain () {
		WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

        WWW www = new WWW(brainEatingURL, form);
        yield return www;

        if (www.error == null){
        	Debug.Log(www.text);


        }else{
        	Debug.LogError(www.error);
        }
    }

    IEnumerator ZombieChecker()
    {
        requestInProgress = true;
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", "12/31/1999 11:59:59");
        form.AddField("client", "mob");

        WWW www = new WWW(zombieStatusURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            JsonData zombStatJson = JsonMapper.ToObject(www.text);

            if (zombStatJson[0].ToString() == "Success")
            {
                int stat = (int)zombStatJson[1];

                if (stat == 0)
                {
                    //player is alive, and has a character active on server
                    Debug.Log("player is alive, and has an active character on the server, since we're in the zombie scene, and they haven't been allowed to restart, this is broken");

                }
                else if (stat == 1)
                {
                    Debug.Log("Player is still a zombie, checking again in 15 seconds");

                }
                else if (stat == 2)
                {
                    Debug.Log("Someone has successfully killed this player's zombie!");
                    StartCoroutine(PostTempQRPanelText("SWEET RELEASE! STARTING NEW GAME!", 3.0f));
                    yield return new WaitForSeconds(2.9f);
                    GameManager.instance.playerIsZombie = false;
                    // zombieQRpanel.SetActive(false);
                    GameManager.instance.StartNewCharacter();
                    
                }
                else
                {
                    Debug.Log("Zombie Check callback returned invalid status code");
                }

            }
            else if (zombStatJson[0].ToString() == "Failed")
            {
                Debug.Log(zombStatJson[1].ToString());
                Debug.Log("This implies that the player entry was deleted from the server... WTF, how are we on the zombie scene tho?");
            }


        }
        else
        {
            Debug.Log(www.error);
        }

        yield return new WaitForSeconds(7.0f);//after checking and posting- wait to allow another check.
        requestInProgress = false;
    }

    IEnumerator PostTempQRPanelText(string txt, float dur)
    {
        qrPanelText.text = txt;
        qrPanelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(dur);
        qrPanelText.text = "";
    }

    public void ToggleZombieQRPanel ()
    {
        if (zombieQRpanel.activeInHierarchy)
        {
            zombieQRpanel.SetActive(false);
        }else
        {
            zombieQRpanel.SetActive(true);
        }
    }
	
    public void ToggleZombieAdPanel ()
    {
        if (zombieAdPanel.activeInHierarchy)
        {
            zombieAdPanel.SetActive(false);
        }else
        {
            zombieAdPanel.SetActive(true);
        }
    }

    void UpdateAdCountText ()
    {
        string my_string = ads_watched + " / "+adsToRevive;
        ad_countText.text = my_string;
    }

    public void ZombieAdFinished ()
    {
        ads_watched++;
        if (ads_watched >= adsToRevive)
        {
            GameManager.instance.StartNewCharacter();
            //StartCoroutine(TrickyBullshitToStartNewGame());
        }else
        {
            UpdateAdCountText();
        }
    }

	public float CalculateDistanceToTarget (float lat, float lng) {
		if (Input.location.status == LocationServiceStatus.Running) {
			float myLat = Input.location.lastData.latitude;
			float myLon = Input.location.lastData.longitude;
			float targetLat = lat;
			float targetLng = lng;

			float latMid = (myLat + targetLat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			double deltaLatitude = (myLat - targetLat);
			double deltaLongitude = (myLon - targetLng);
			double latDistMeters = deltaLatitude * m_per_deg_lat;
			double lonDistMeters = deltaLongitude * m_per_deg_lon;
			double directDistMeters = Mathf.Sqrt(Mathf.Pow((float)latDistMeters, 2f)+Mathf.Pow((float)lonDistMeters, 2f));

			string myText = "You are "+(float)directDistMeters+" meters from your target";
			Debug.Log (myText);

			return (float)directDistMeters;

		} else {
			Debug.Log ("Location services not running");
			//StartCoroutine(PostTempLocationText("Location Services Not Running"));
			return 1000.0f;
		}
	}


	//this function is called from an InvokeRepeating.
	void CheckAndUpdateMap () {
		//check if location services are active
		if (Input.location.status == LocationServiceStatus.Running) {

			//if a last location has NOT been logged
			if (lastUpdateLat != 0 && lastUpdateLng != 0) {
				//if you've moved enough, then do the update, otherwise do nothing
				if (CalculateDistanceToTarget(lastUpdateLat, lastUpdateLng)>= 20.0f) {

					//if player has traveled 250+meters since google api was last pinged, then change the boolean so that it hits the api for new json data.
					if (CalculateDistanceToTarget(buildingSpawner.lastGoogleLat, buildingSpawner.lastGoogleLng) >= 250.0f) {
						buildingSpawner.bldgsNeedUpdate = true;
					}
					//log the last location updated from
					lastUpdateLat = Input.location.lastData.latitude;
					lastUpdateLng = Input.location.lastData.longitude;
				}
			} else {
				//store current location as last updated location and do the update
				lastUpdateLat = Input.location.lastData.latitude;
				lastUpdateLng = Input.location.lastData.longitude;
			}

			//update the map background
			GoogleMap my_googleMap = FindObjectOfType<GoogleMap>();
			if (my_googleMap != null)
			{
				my_googleMap.Refresh();
			}
			else
			{
				Debug.Log("UI updater could not locate Google Map clas to refresh map image");
			}
		}
		//update building locations
		buildingSpawner.UpdateBuildings();

	}

    public void AdPartialWatch()
    {
        Debug.Log("Player didn't finish the ad- they get no credit");
    }

    //good news, tricky bullshit works
    IEnumerator TrickyBullshitToStartNewGame()
    {
  
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("01a Login");
        yield return new WaitForSeconds(0.2f);
        LoginManager myLoginManager = GameObject.Find("Login Manager").GetComponent<LoginManager>();
        myLoginManager.StartNewCharacter();
        Destroy(this.gameObject);
        
    }

	private float distToActiveBldg = 0.0f;
	public void ActivateBuildingInspector (ZM_Building myBuilding) {
		CancelInvoke ("CheckAndUpdateMap"); //stop the background map from updating

		//calculate the distance to this building.
		float distToBldg = (int)CalculateDistanceToTarget (myBuilding.myLattitude, myBuilding.myLongitude);
		distToActiveBldg = distToBldg;

		//set the panel text
		bldgNameText.text = myBuilding.name;
		bldgDistanceText.text = distToActiveBldg.ToString ();

		buildingPanel.SetActive (true); //turn the panel on
	}

	public void DeactivateBuildingInspector () {
		InvokeRepeating ("CheckAndUpdateMap", 10f, 10f);
		buildingPanel.SetActive (false);
	}

	//based on RegenerateStamina from MapLevelManager.cs
	public void GatherZombies () {
		int zombCount = 0;

		//calculate any distance bonus from last regen
		if (Input.location.status == LocationServiceStatus.Running) {
			if (lastZombieGatherLat != 0 && lastZombieGatherLng != 0) {
				float distanceFromLastUpdate = CalculateDistanceToTarget(lastZombieGatherLat, lastZombieGatherLng);
				if (distanceFromLastUpdate > 25) {
					if(distanceFromLastUpdate >= 300) {
						distanceFromLastUpdate=300;
					}
					//award zombies for every metersPerZombieGather the player has covered
					int intervals = Mathf.RoundToInt(distanceFromLastUpdate/metersPerZombieGather);
					zombCount += intervals;
					lastZombieGatherLat = Input.location.lastData.latitude;
					lastZombieGatherLng = Input.location.lastData.longitude;
				}
			} else {
				lastZombieGatherLat = Input.location.lastData.latitude;
				lastZombieGatherLng = Input.location.lastData.longitude;
			}
				
		} else {
			Debug.Log("location services not running, no distance bonus for stamina regen");
		}
	
		//Send any new zombies to the server
		if (zombCount > 0) {
			
			StartCoroutine (SendZombieGatherToServer(zombCount));
		
		}

	}

	IEnumerator SendZombieGatherToServer(int zomb) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("zomb_gathered", zomb);

		WWW www = new WWW(zombieUpdateURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
			zombieHorde += zomb;
			UpdateTheUI ();
		} else {
			Debug.Log(www.error);
		}
	}
}
