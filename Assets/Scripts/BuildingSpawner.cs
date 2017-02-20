using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System;
using UnityEngine.UI;


public class BuildingSpawner : MonoBehaviour {

	public GameObject homebase;
	public GameObject bldgHolder;
	public GameObject outpostPrefab;
	public float lastGoogleLat = 0, lastGoogleLng = 0;
    public Sprite non_bldg_sprite;

	private float minX, maxX, minY, maxY;
	public double m_per_deg_lat, m_per_deg_lon;

	private Vector3 screenCenter = new Vector3((Screen.width*0.5f), (Screen.height*0.5f), 0.0f);
	private string googlePlacesAPIURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
    private string googleNextPagePlacesURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?pagetoken=";
	private string googleAPIKey = "AIzaSyC0Ly6W_ljFCk5sV4n-T73e-rhRdNpiEe4";

	public bool googleBldgsNeedUpdate;

	private static GameObject gameCanvas;
    private static PopulatedBuilding populatedBuildingPrefab;


    void Start () {
		googleBldgsNeedUpdate = true;
        gameCanvas = GameObject.Find("Canvas");
        populatedBuildingPrefab = Resources.Load<PopulatedBuilding>("Prefabs/Populated Building");

		//StartCoroutine(GetNearbyBuildingsGoogle());
        //StartCoroutine(Get60NearbyBuildingsGoogle());
    }

	public void UpdateBuildings () {

        //each pair of fucntions goes together. 60 is supposed to accomodate page refresh, and no number is the default 20 results.
		if (googleBldgsNeedUpdate == true) {
			//StartCoroutine(GetNearbyBuildingsGoogle());
            StartCoroutine(Get60NearbyBuildingsGoogle());
		}  else {
			//TurnGoogleJsonIntoBuildings();
            Turn60GoogleJsonIntoBuildings();
		}
	}

    //this query will only return 20 results. SEE: Get60NearbyBuildingsGoogle()
	IEnumerator GetNearbyBuildingsGoogle () {
		
			string myWwwString = googlePlacesAPIURL;
			myWwwString += "?location=";
			if (Input.location.status == LocationServiceStatus.Running) {
				myWwwString += Input.location.lastData.latitude +","+Input.location.lastData.longitude;
				lastGoogleLat = Input.location.lastData.latitude;
				lastGoogleLng = Input.location.lastData.longitude;

			}  else {
				myWwwString += "37.70897,-122.4292";
				//this is assuming my home location
			}
			myWwwString += "&radius=400";
			//myWwwString += "&keyword=things";
			myWwwString += "&key="+ googleAPIKey;

			Debug.Log(myWwwString);
			WWW www = new WWW(myWwwString);
			yield return www;
			//Debug.Log(www.text);

			//File.WriteAllText(Application.dataPath + "/Resources/googlelocations.json", www.text.ToString());
			//googleJsonReturn = www.text;
			GameManager.instance.locationJsonText = www.text;
			TurnGoogleJsonIntoBuildings();
			googleBldgsNeedUpdate = false;

	}


    IEnumerator Get60NearbyBuildingsGoogle()
    {

        string myWwwString = googlePlacesAPIURL;
        myWwwString += "?location=";
        if (Input.location.status == LocationServiceStatus.Running)
        {
            myWwwString += Input.location.lastData.latitude + "," + Input.location.lastData.longitude;
            lastGoogleLat = Input.location.lastData.latitude;
            lastGoogleLng = Input.location.lastData.longitude;

        }
        else
        {
            myWwwString += "37.70897,-122.4292";
            //this is assuming my home location
        }
        myWwwString += "&radius=400";
        //myWwwString += "&keyword=things";
        myWwwString += "&key=" + googleAPIKey;

        Debug.Log(myWwwString);
        WWW www = new WWW(myWwwString);
        yield return www;
        
        if (www.error == null)
        {
            JsonData googleJsonReturn = JsonMapper.ToObject(www.text);
            GameManager.instance.googleBldgJsonTextpg1 = www.text;

            if (googleJsonReturn.Keys.Contains("next_page_token"))
            {
                string next_pg_tkn = googleJsonReturn["next_page_token"].ToString();
                string nextPgURL = googleNextPagePlacesURL + next_pg_tkn+"&key="+googleAPIKey;

                yield return new WaitForSeconds(1.0f); //google has warnings about hitting next too fast in code. so delay 1 sec.
                Debug.Log("second query to places executing with URL:" + nextPgURL);
                WWW www2 = new WWW(nextPgURL);
                yield return www2;

                if (www2.error == null)
                {
                    Debug.Log(www2.text);
                    JsonData googleSecondJsonReturn = JsonMapper.ToObject(www2.text);
                    GameManager.instance.googleBldgJsonTextpg2 = www2.text;

                    if (googleSecondJsonReturn.Keys.Contains("next_page_token"))
                    {

                        string last_pg_token = googleSecondJsonReturn["next_page_token"].ToString();
                        string lastPgURL = googleNextPagePlacesURL + last_pg_token + "&key=" + googleAPIKey;

                        yield return new WaitForSeconds(1.0f);
                        Debug.Log("last page of google responses loading with URL:" + lastPgURL);
                        WWW www3 = new WWW(lastPgURL);
                        yield return www3;

                        if (www3.error == null)
                        {
                            GameManager.instance.googleBldgJsonTextpg3 = www3.text;
                        }
                        else
                        {
                            Debug.Log(www3.error);
                        }

                    }
                    else
                    {
                        GameManager.instance.googleBldgJsonTextpg3 = "";
                        Debug.Log("There should be no third page loading");
                    }

                    //call the building constructor from here for multiple pages of bldgs
                    
                }
                else
                {
                    Debug.Log(www2.error);
                }
                //Turn60GoogleJsonIntoBuildings();
            }
            else
            {
                GameManager.instance.locationJsonText = www.text;
                //Turn60GoogleJsonIntoBuildings();
            }
            

        }else
        {
            Debug.Log(www.error);
        }

        //        GameManager.instance.locationJsonText = www.text;
        //        TurnGoogleJsonIntoBuildings();
        Turn60GoogleJsonIntoBuildings();
        googleBldgsNeedUpdate = false;

    }

    public void TurnGoogleJsonIntoBuildings () {

		//destroy the existing buildings
		GameObject[] oldBldgs = GameObject.FindGameObjectsWithTag("building");
		foreach (GameObject oldBldg in oldBldgs) {
			Destroy(oldBldg.gameObject);
		}
    	
		//string jsonString = File.ReadAllText(Application.dataPath + "/Resources/googlelocations.json");
		Debug.Log(GameManager.instance.locationJsonText);
		JsonData bldgJson = JsonMapper.ToObject(GameManager.instance.locationJsonText);
		Debug.Log(JsonMapper.ToJson(bldgJson));
        //JsonData foursquareJson = JsonMapper.ToObject(jsonReturn);

        double m_per_pixel_mapBG = GetMetersPerPixelOfGoogleMapImage();
        GoogleMap my_GM = FindObjectOfType<GoogleMap>();
        //int map_img_size = 560;//this worked for iPhone 5
        int map_img_size = Mathf.FloorToInt(Screen.height/2);
        double map_height_in_meters = (m_per_pixel_mapBG*map_img_size);
        double m_per_screen_pixel = map_height_in_meters / Screen.height;
        Debug.Log("Calculating m/px-BG: " + m_per_pixel_mapBG + "  Map width in meters: "+map_height_in_meters+"  and meters/pixel for building placement: " + m_per_screen_pixel);
	        
        for (int i = 0; i < bldgJson["results"].Count; i++) {
			string myName = (string)bldgJson["results"][i]["name"];
			string myBldgID = (string)bldgJson["results"][i]["id"];
            JsonData thisEntry = JsonMapper.ToObject(JsonMapper.ToJson(bldgJson["results"][i]));
            string my_photo_ref = "";
            if (thisEntry.Keys.Contains("photos")) {
                my_photo_ref = (string)bldgJson["results"][i]["photos"][0]["photo_reference"];
            }
        	float lat = (float)(double)bldgJson["results"][i]["geometry"]["location"]["lat"];
			float lng = (float)(double)bldgJson["results"][i]["geometry"]["location"]["lng"];

			
			//Debug.Log (name + lat + lng);

			//calculate the average latitude between the two locations, and then calculate the meters/DEGREE lat/lon
			float latMid =(Input.location.lastData.latitude + lat)/2f;
			m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			//Debug.Log ("for the " + name + " building, meters per degree calculated as " + m_per_deg_lat + " m/deg lat, and " + m_per_deg_lon +" m/deg lon");
			double deltaLatitude = 0;
			double deltaLongitude = 0;
			if (Input.location.status == LocationServiceStatus.Running){
				deltaLatitude = (Input.location.lastData.latitude - lat);
				deltaLongitude = (Input.location.lastData.longitude - lng);
			}  else {
				deltaLatitude = (37.70883f - lat);
				deltaLongitude = (-122.4293 - lng);
			}
			double xDistMeters = deltaLongitude * m_per_deg_lon;
			double yDistMeters = deltaLatitude * m_per_deg_lat;
            float xScreenDist = (float)(xDistMeters * m_per_screen_pixel);
            float yScreenDist = (float)(yDistMeters * m_per_screen_pixel);
			

			
			PopulatedBuilding instance = Instantiate(populatedBuildingPrefab);
			instance.GenerateZombies();
            instance.SetToUnknown();
			instance.name = myName;
			instance.buildingName = myName;
			instance.buildingID = myBldgID;
            instance.photo_reference = my_photo_ref;
			instance.myLat = lat;
			instance.myLng = lng;
			float xCoord = (float)(screenCenter.x - (xScreenDist));
			float yCoord = (float)(screenCenter.y - (yScreenDist));
			Vector3 pos = new Vector3 (xCoord, yCoord, 0);

			instance.transform.SetParent(bldgHolder.transform);
			instance.transform.position = pos;

			//determine the loot class of the building
			string type_bldg = bldgJson["results"][i]["types"][0].ToString();
            instance.google_type = type_bldg;
			if (type_bldg == "bakery" || type_bldg == "cafe" || type_bldg == "convenience_store" || type_bldg == "food" || type_bldg == "grocery_or_supermarket" || type_bldg == "restaurant") {
				//food likely
				instance.loot_code = "F";
			} else if (type_bldg == "aquarium" || type_bldg == "bar" || type_bldg == "liquor_store" || type_bldg == "spa" || type_bldg == "zoo") {
				//water likely
				instance.loot_code = "W";
			} else if (type_bldg == "bicycle_store" || type_bldg == "bowling_alley" || type_bldg == "car_repair" || type_bldg == "electrician" || type_bldg == "general_contractor" || type_bldg == "hardware_store" || type_bldg == "hospital" || type_bldg == "police" || type_bldg == "plumber") {
				//supply likely
				instance.loot_code = "S";
			} else {
				//generic loot class
				instance.loot_code = "G";
			}

			//instance.CheckForDeactivation();
			//Debug.Log("placed "+instance.name+" at coords: "+xCoord+" x and "+yCoord+" y");
		}

		//activate all buildings without inactive entries
		GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
		if (GameManager.instance.clearedBldgJsonText != "") {
			JsonData clearedJson = JsonMapper.ToObject(GameManager.instance.clearedBldgJsonText);

			foreach (GameObject building in buildings) {
				int cleared = 0;
				PopulatedBuilding myBuilding = building.GetComponent<PopulatedBuilding>();
				//Set all buildings to the "unentered" datetime by default
				myBuilding.last_cleared = DateTime.Parse("11:59pm 12/31/1999");
                
                //go through the player record of buildings they've cleared
				for (int i=0; i<clearedJson.Count; i++) {
                    //Debug.Log(myBuilding.buildingName+" comparing to "+clearedJson[i]["bldg_name"].ToString());
                    //if the spawned building matches the record, populate it
                    if (myBuilding.buildingName == clearedJson[i]["bldg_name"].ToString()) {
                        //load in the saved building data
                        myBuilding.wood_inside = (int)clearedJson[i]["wood"];
                        myBuilding.metal_inside = (int)clearedJson[i]["metal"];
                        myBuilding.food_inside = (int)clearedJson[i]["food"];
                        myBuilding.water_inside = (int)clearedJson[i]["water"];
                        myBuilding.zombiePopulation = (int)clearedJson[i]["zombies"];
                        myBuilding.last_cleared = DateTime.Parse(clearedJson[i]["time_cleared"].ToString());

                        if (clearedJson[i]["last_looted_food"].ToString() != "0000-00-00 00:00:00") //this is a blank entry in current DB config
                        {
                            myBuilding.last_looted_food = DateTime.Parse(clearedJson[i]["last_looted_food"].ToString());
                        }
                        if (clearedJson[i]["last_looted_water"].ToString() != "0000-00-00 00:00:00") //this is a blank entry in current DB config
                        {
                            myBuilding.last_looted_water = DateTime.Parse(clearedJson[i]["last_looted_water"].ToString());
                        }

                        //CHECK ITEMS
                        if (clearedJson[i]["has_trap"].ToString() == "1"){
                            myBuilding.has_traps = true;
                        }else
                        {
                            myBuilding.has_traps = false;
                        }
                        if (clearedJson[i]["has_barrel"].ToString() == "1")
                        {
                            myBuilding.has_barrel = true;
                        }else
                        {
                            myBuilding.has_barrel = false;
                        }
                        if (clearedJson[i]["has_greenhouse"].ToString() == "1")
                        {
                            myBuilding.has_greenhouse = true;
                        }else
                        {
                            myBuilding.has_greenhouse = false;
                        }

						//Debug.Log(myBuilding.buildingName+" Matched buildings, Active: "+clearedJson[i]["active"].ToString());
						if (clearedJson[i]["active"].ToString()=="0") {
                            //Debug.Log(myBuilding.buildingName+" has been found to be clear- not being activated");
                            myBuilding.ActivateMe(); //all buildings should now be active
							cleared = 1;
							break;
						} 
					} else {
                        continue;
					}
				}

                //ActivateMe is now the catch all building initilizer.
                myBuilding.ActivateMe();

                /*
                if (cleared == 0) {
					myBuilding.ActivateMe();
				}
                */
			}
		} else {
			foreach (GameObject building in buildings) {
				PopulatedBuilding myBuilding = building.GetComponent<PopulatedBuilding>();
				myBuilding.last_cleared = DateTime.Parse("11:59pm 12/31/1999");
				myBuilding.ActivateMe();
			}
		}

		/*
      	//deactivate any cleared buildings
    	if (GameManager.instance.clearedBldgJsonText != "") {
    		JsonData clearedJson = JsonMapper.ToObject(GameManager.instance.clearedBldgJsonText);

	    	//ensure there are more than 0 buildings returned
	    	if (clearedJson.Count > 0) {
		    	for (int i = 0; i < clearedJson.Count; i++) {
		    		//if the building is still considered inactive by the server
		    		if (clearedJson[i]["active"].ToString() == "0") {
		    			//Debug.Log ("Coroutine has found "+ clearedJson[i]["bldg_name"].ToString()+" to be inactive");
						GameObject thisBuilding = GameObject.Find(clearedJson[i]["bldg_name"].ToString());
						if (thisBuilding != null) {
		    				PopulatedBuilding populatedBldg = thisBuilding.GetComponent<PopulatedBuilding>();
							Debug.Log ("GameManager is attempting to deactivate "+populatedBldg.gameObject.name);
							populatedBldg.DeactivateMe();
		    			}  else {
		    				Debug.Log("Couldn't find "+clearedJson[i]["bldg_name"].ToString()+" to deactivate");
		    				continue;
		    			}

		    		}  
		    	}
			}else {
	    		Debug.Log ("Player has not cleared any buildings yet");
	    	}  

		}
		*/

		PlaceHomebaseGraphic();
      	//StartCoroutine(GameManager.instance.FetchOutpostData());
      	SpawnOutpostsToMap(); 
    }

    public bool CurrentlyPlacing60Bldgs = false;
    public void Turn60GoogleJsonIntoBuildings()
    {
        if (CurrentlyPlacing60Bldgs)
        {
            return;//if this function is already running, exit
        }
        else
        {
            CurrentlyPlacing60Bldgs = true;

            if (GameManager.instance.googleBldgJsonTextpg1 == "")
            {
                Debug.Log("invalid JSON stored on GameManager");
                CurrentlyPlacing60Bldgs = false;
                return;//exit the function
            }

            //destroy the existing buildings
            GameObject[] oldBldgs = GameObject.FindGameObjectsWithTag("building");
            foreach (GameObject oldBldg in oldBldgs)
            {
                Destroy(oldBldg.gameObject);
            }

            //destroy the existing walls
            GameObject[] oldWalls = GameObject.FindGameObjectsWithTag("walllocation");
            foreach (GameObject wall in oldWalls)
            {
                Destroy(wall.gameObject);
            }


            Debug.Log(GameManager.instance.googleBldgJsonTextpg1);
            JsonData GoogleBldgJsonpg1 = JsonMapper.ToObject(GameManager.instance.googleBldgJsonTextpg1);

            double m_per_pixel_mapBG = GetMetersPerPixelOfGoogleMapImage();
            GoogleMap my_GM = FindObjectOfType<GoogleMap>();
            int map_img_size = Mathf.FloorToInt(Screen.height / 2);
            double map_height_in_meters = (m_per_pixel_mapBG * map_img_size);
            double m_per_screen_pixel = map_height_in_meters / Screen.height;
            Debug.Log("Calculating m/px-BG: " + m_per_pixel_mapBG + "  Map width in meters: " + map_height_in_meters + "  and meters/pixel for building placement: " + m_per_screen_pixel);

            #region first 20 bldgs
            for (int i = 0; i < GoogleBldgJsonpg1["results"].Count; i++)
            {
                string myName = (string)GoogleBldgJsonpg1["results"][i]["name"];
                string myBldgID = (string)GoogleBldgJsonpg1["results"][i]["id"];
                JsonData thisEntry = JsonMapper.ToObject(JsonMapper.ToJson(GoogleBldgJsonpg1["results"][i]));
                string my_photo_ref = "";
                if (thisEntry.Keys.Contains("photos"))
                {
                    my_photo_ref = (string)GoogleBldgJsonpg1["results"][i]["photos"][0]["photo_reference"];
                }
                float lat = (float)(double)GoogleBldgJsonpg1["results"][i]["geometry"]["location"]["lat"];
                float lng = (float)(double)GoogleBldgJsonpg1["results"][i]["geometry"]["location"]["lng"];


                //Debug.Log (name + lat + lng);

                //calculate the average latitude between the two locations, and then calculate the meters/DEGREE lat/lon
                float latMid = (Input.location.lastData.latitude + lat) / 2f;
                m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos(2 * latMid) + 1.175 * Mathf.Cos(4 * latMid);
                m_per_deg_lon = 111132.954 * Mathf.Cos(latMid);

                //Debug.Log ("for the " + name + " building, meters per degree calculated as " + m_per_deg_lat + " m/deg lat, and " + m_per_deg_lon +" m/deg lon");
                double deltaLatitude = 0;
                double deltaLongitude = 0;
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    deltaLatitude = (Input.location.lastData.latitude - lat);
                    deltaLongitude = (Input.location.lastData.longitude - lng);
                }
                else
                {
                    deltaLatitude = (37.70883f - lat);
                    deltaLongitude = (-122.4293 - lng);
                }
                double xDistMeters = deltaLongitude * m_per_deg_lon;
                double yDistMeters = deltaLatitude * m_per_deg_lat;
                float xScreenDist = (float)(xDistMeters * m_per_screen_pixel);
                float yScreenDist = (float)(yDistMeters * m_per_screen_pixel);



                PopulatedBuilding instance = Instantiate(populatedBuildingPrefab);
                instance.GenerateZombies();
                instance.SetToUnknown();
                instance.name = myName;
                instance.buildingName = myName;
                instance.buildingID = myBldgID;
                instance.photo_reference = my_photo_ref;
                instance.myLat = lat;
                instance.myLng = lng;
                float xCoord = (float)(screenCenter.x - (xScreenDist));
                float yCoord = (float)(screenCenter.y - (yScreenDist));
                Vector3 pos = new Vector3(xCoord, yCoord, 0);

                instance.transform.SetParent(bldgHolder.transform);
                instance.transform.position = pos;

                //determine the loot class of the building
                string type_bldg = GoogleBldgJsonpg1["results"][i]["types"][0].ToString();
                instance.google_type = type_bldg;
                if (type_bldg == "locality" || type_bldg == "bus_station" || type_bldg == "point_of_interest")
                {
                    //these are NOT buildings- delete the instance and continue the loop to the next entry.
                    //Destroy(instance.gameObject);
                    Image theImage = instance.gameObject.GetComponent<Image>();
                    theImage.sprite = non_bldg_sprite;
                    instance.gameObject.tag = "walllocation"; //ensure that this PopulatedBuilding gameobject does not get filtered with building location updates
                    instance.loot_code = "A";
                    continue;
                }
                else if (type_bldg == "bakery" || type_bldg == "cafe" || type_bldg == "convenience_store" || type_bldg == "food" || type_bldg == "grocery_or_supermarket" || type_bldg == "restaurant")
                {
                    //food likely
                    instance.loot_code = "F";
                }
                else if (type_bldg == "aquarium" || type_bldg == "bar" || type_bldg == "liquor_store" || type_bldg == "spa" || type_bldg == "zoo")
                {
                    //water likely
                    instance.loot_code = "W";
                }
                else if (type_bldg == "bicycle_store" || type_bldg == "bowling_alley" || type_bldg == "car_repair" || type_bldg == "electrician" || type_bldg == "general_contractor" || type_bldg == "hardware_store" || type_bldg == "hospital" || type_bldg == "police" || type_bldg == "plumber")
                {
                    //supply likely
                    instance.loot_code = "S";
                }
                else
                {
                    //generic loot class
                    instance.loot_code = "G";
                }

                //instance.CheckForDeactivation();
                //Debug.Log("placed "+instance.name+" at coords: "+xCoord+" x and "+yCoord+" y");
            }
            #endregion

            #region second 20 bldgs
            if (GameManager.instance.googleBldgJsonTextpg2 != "")
            {
                JsonData GoogleBldgJsonpg2 = JsonMapper.ToObject(GameManager.instance.googleBldgJsonTextpg2);
                if (GoogleBldgJsonpg2["status"].ToString() != "INVALID_REQUEST")
                {
                    for (int i = 0; i < GoogleBldgJsonpg2["results"].Count; i++)
                    {
                        string myName = (string)GoogleBldgJsonpg2["results"][i]["name"];
                        string myBldgID = (string)GoogleBldgJsonpg2["results"][i]["id"];
                        JsonData thisEntry = JsonMapper.ToObject(JsonMapper.ToJson(GoogleBldgJsonpg2["results"][i]));
                        string my_photo_ref = "";
                        if (thisEntry.Keys.Contains("photos"))
                        {
                            my_photo_ref = (string)GoogleBldgJsonpg2["results"][i]["photos"][0]["photo_reference"];
                        }
                        float lat = (float)(double)GoogleBldgJsonpg2["results"][i]["geometry"]["location"]["lat"];
                        float lng = (float)(double)GoogleBldgJsonpg2["results"][i]["geometry"]["location"]["lng"];


                        //Debug.Log (name + lat + lng);

                        //calculate the average latitude between the two locations, and then calculate the meters/DEGREE lat/lon
                        float latMid = (Input.location.lastData.latitude + lat) / 2f;
                        m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos(2 * latMid) + 1.175 * Mathf.Cos(4 * latMid);
                        m_per_deg_lon = 111132.954 * Mathf.Cos(latMid);

                        //Debug.Log ("for the " + name + " building, meters per degree calculated as " + m_per_deg_lat + " m/deg lat, and " + m_per_deg_lon +" m/deg lon");
                        double deltaLatitude = 0;
                        double deltaLongitude = 0;
                        if (Input.location.status == LocationServiceStatus.Running)
                        {
                            deltaLatitude = (Input.location.lastData.latitude - lat);
                            deltaLongitude = (Input.location.lastData.longitude - lng);
                        }
                        else
                        {
                            deltaLatitude = (37.70883f - lat);
                            deltaLongitude = (-122.4293 - lng);
                        }
                        double xDistMeters = deltaLongitude * m_per_deg_lon;
                        double yDistMeters = deltaLatitude * m_per_deg_lat;
                        float xScreenDist = (float)(xDistMeters * m_per_screen_pixel);
                        float yScreenDist = (float)(yDistMeters * m_per_screen_pixel);



                        PopulatedBuilding instance = Instantiate(populatedBuildingPrefab);
                        instance.GenerateZombies();
                        instance.SetToUnknown();
                        instance.name = myName;
                        instance.buildingName = myName;
                        instance.buildingID = myBldgID;
                        instance.photo_reference = my_photo_ref;
                        instance.myLat = lat;
                        instance.myLng = lng;
                        float xCoord = (float)(screenCenter.x - (xScreenDist));
                        float yCoord = (float)(screenCenter.y - (yScreenDist));
                        Vector3 pos = new Vector3(xCoord, yCoord, 0);

                        instance.transform.SetParent(bldgHolder.transform);
                        instance.transform.position = pos;

                        //determine the loot class of the building
                        string type_bldg = GoogleBldgJsonpg2["results"][i]["types"][0].ToString();
                        instance.google_type = type_bldg;
                        if (type_bldg == "locality" || type_bldg == "bus_station" || type_bldg == "point_of_interest")
                        {
                            //these are NOT buildings- delete the instance and continue the loop to the next entry.
                            //Destroy(instance.gameObject);
                            Image theImage = instance.gameObject.GetComponent<Image>();
                            theImage.sprite = non_bldg_sprite;
                            instance.gameObject.tag = "walllocation"; //ensure that this PopulatedBuilding gameobject does not get filtered with building location updates
                            instance.loot_code = "A";
                            continue;
                        }
                        else if (type_bldg == "bakery" || type_bldg == "cafe" || type_bldg == "convenience_store" || type_bldg == "food" || type_bldg == "grocery_or_supermarket" || type_bldg == "restaurant")
                        {
                            //food likely
                            instance.loot_code = "F";
                        }
                        else if (type_bldg == "aquarium" || type_bldg == "bar" || type_bldg == "liquor_store" || type_bldg == "spa" || type_bldg == "zoo")
                        {
                            //water likely
                            instance.loot_code = "W";
                        }
                        else if (type_bldg == "bicycle_store" || type_bldg == "bowling_alley" || type_bldg == "car_repair" || type_bldg == "electrician" || type_bldg == "general_contractor" || type_bldg == "hardware_store" || type_bldg == "hospital" || type_bldg == "police" || type_bldg == "plumber")
                        {
                            //supply likely
                            instance.loot_code = "S";
                        }
                        else
                        {
                            //generic loot class
                            instance.loot_code = "G";
                        }

                        //instance.CheckForDeactivation();
                        //Debug.Log("placed "+instance.name+" at coords: "+xCoord+" x and "+yCoord+" y");
                    }
                }
                else
                {
                    Debug.Log("Second page of Google Map Results returned INVALID_REQUEST- Likely we are refreshing too fast");
                }
            }
            else { Debug.Log("no second page of results loaded onto game manager, building load failed"); }
            #endregion

            #region last 20 bldgs

            if (GameManager.instance.googleBldgJsonTextpg3 != "")
            {

                JsonData googleBldgJsonpg3 = JsonMapper.ToObject(GameManager.instance.googleBldgJsonTextpg3);
                if (googleBldgJsonpg3["status"].ToString() != "INVALID_REQUEST")
                {
                    for (int i = 0; i < googleBldgJsonpg3["results"].Count; i++)
                    {
                        string myName = (string)googleBldgJsonpg3["results"][i]["name"];
                        string myBldgID = (string)googleBldgJsonpg3["results"][i]["id"];
                        JsonData thisEntry = JsonMapper.ToObject(JsonMapper.ToJson(googleBldgJsonpg3["results"][i]));
                        string my_photo_ref = "";
                        if (thisEntry.Keys.Contains("photos"))
                        {
                            my_photo_ref = (string)googleBldgJsonpg3["results"][i]["photos"][0]["photo_reference"];
                        }
                        float lat = (float)(double)googleBldgJsonpg3["results"][i]["geometry"]["location"]["lat"];
                        float lng = (float)(double)googleBldgJsonpg3["results"][i]["geometry"]["location"]["lng"];


                        //Debug.Log (name + lat + lng);

                        //calculate the average latitude between the two locations, and then calculate the meters/DEGREE lat/lon
                        float latMid = (Input.location.lastData.latitude + lat) / 2f;
                        m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos(2 * latMid) + 1.175 * Mathf.Cos(4 * latMid);
                        m_per_deg_lon = 111132.954 * Mathf.Cos(latMid);

                        //Debug.Log ("for the " + name + " building, meters per degree calculated as " + m_per_deg_lat + " m/deg lat, and " + m_per_deg_lon +" m/deg lon");
                        double deltaLatitude = 0;
                        double deltaLongitude = 0;
                        if (Input.location.status == LocationServiceStatus.Running)
                        {
                            deltaLatitude = (Input.location.lastData.latitude - lat);
                            deltaLongitude = (Input.location.lastData.longitude - lng);
                        }
                        else
                        {
                            deltaLatitude = (37.70883f - lat);
                            deltaLongitude = (-122.4293 - lng);
                        }
                        double xDistMeters = deltaLongitude * m_per_deg_lon;
                        double yDistMeters = deltaLatitude * m_per_deg_lat;
                        float xScreenDist = (float)(xDistMeters * m_per_screen_pixel);
                        float yScreenDist = (float)(yDistMeters * m_per_screen_pixel);



                        PopulatedBuilding instance = Instantiate(populatedBuildingPrefab);
                        instance.GenerateZombies();
                        instance.SetToUnknown();
                        instance.name = myName;
                        instance.buildingName = myName;
                        instance.buildingID = myBldgID;
                        instance.photo_reference = my_photo_ref;
                        instance.myLat = lat;
                        instance.myLng = lng;
                        float xCoord = (float)(screenCenter.x - (xScreenDist));
                        float yCoord = (float)(screenCenter.y - (yScreenDist));
                        Vector3 pos = new Vector3(xCoord, yCoord, 0);

                        instance.transform.SetParent(bldgHolder.transform);
                        instance.transform.position = pos;

                        //determine the loot class of the building
                        string type_bldg = googleBldgJsonpg3["results"][i]["types"][0].ToString();
                        instance.google_type = type_bldg;
                        if (type_bldg == "locality" || type_bldg == "bus_station" || type_bldg == "point_of_interest")
                        {
                            //these are NOT buildings- delete the instance and continue the loop to the next entry.
                            //Destroy(instance.gameObject);
                            Image theImage = instance.gameObject.GetComponent<Image>();
                            theImage.sprite = non_bldg_sprite;
                            instance.gameObject.tag = "walllocation"; //ensure that this PopulatedBuilding gameobject does not get filtered with building location updates
                            instance.loot_code = "A";
                            continue;
                        }
                        else if (type_bldg == "bakery" || type_bldg == "cafe" || type_bldg == "convenience_store" || type_bldg == "food" || type_bldg == "grocery_or_supermarket" || type_bldg == "restaurant")
                        {
                            //food likely
                            instance.loot_code = "F";
                        }
                        else if (type_bldg == "aquarium" || type_bldg == "bar" || type_bldg == "liquor_store" || type_bldg == "spa" || type_bldg == "zoo")
                        {
                            //water likely
                            instance.loot_code = "W";
                        }
                        else if (type_bldg == "bicycle_store" || type_bldg == "bowling_alley" || type_bldg == "car_repair" || type_bldg == "electrician" || type_bldg == "general_contractor" || type_bldg == "hardware_store" || type_bldg == "hospital" || type_bldg == "police" || type_bldg == "plumber")
                        {
                            //supply likely
                            instance.loot_code = "S";
                        }
                        else
                        {
                            //generic loot class
                            instance.loot_code = "G";
                        }

                        //instance.CheckForDeactivation();
                        //Debug.Log("placed "+instance.name+" at coords: "+xCoord+" x and "+yCoord+" y");
                    }
                }
                else
                {
                    Debug.Log("Third page of Google Map Results returned INVALID_REQUEST- Likely we are refreshing too fast");
                }
            }
            else
            {
                Debug.Log("GameManager did not load a third page of building results from google");
            }
            #endregion

            #region updated buildings against game-data
            //activate all buildings without inactive entries
            GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
            /*
            PopulatedBuilding[] populated_buildings = new PopulatedBuilding[buildings.Length];
            int pos = 0;
            foreach (GameObject bldg in buildings)
            {
                //load the populated buildings into the array we just created
                populated_buildings[pos] = bldg.GetComponent<PopulatedBuilding>();
                pos++;
            }*/

            if (GameManager.instance.clearedBldgJsonText != "") //if we have cleared bldg records
            {
                JsonData clearedJson = JsonMapper.ToObject(GameManager.instance.clearedBldgJsonText);
                
                foreach (GameObject building in buildings)
                {
                    int cleared = 0;
                    PopulatedBuilding myBuilding = building.GetComponent<PopulatedBuilding>();
                    //Set all buildings to the "unentered" datetime by default
                    myBuilding.last_cleared = DateTime.Parse("11:59pm 12/31/1999");

                    //go through the player record of buildings they've cleared
                    for (int i = 0; i < clearedJson.Count; i++)
                    {
                        //Debug.Log(myBuilding.buildingName+" comparing to "+clearedJson[i]["bldg_name"].ToString());
                        //if the spawned building matches the record, populate it
                        if (myBuilding.buildingName == clearedJson[i]["bldg_name"].ToString())
                        {
                            //Debug.Log("found cleared building data for building loaded to map- JSON output:  " + JsonMapper.ToJson(clearedJson[i]));
                            //load in the saved building data
                            myBuilding.wood_inside = (int)clearedJson[i]["wood"];
                            myBuilding.metal_inside = (int)clearedJson[i]["metal"];
                            myBuilding.food_inside = (int)clearedJson[i]["food"];
                            myBuilding.water_inside = (int)clearedJson[i]["water"];
                            myBuilding.zombiePopulation = (int)clearedJson[i]["zombies"];
                            myBuilding.zombies_across = (int)clearedJson[i]["zombies_across"];
                            myBuilding.last_cleared = DateTime.Parse(clearedJson[i]["time_cleared"].ToString());

                            if (clearedJson[i]["last_looted_food"].ToString() != "0000-00-00 00:00:00") //this is a blank entry in current DB config
                            {
                                myBuilding.last_looted_food = DateTime.Parse(clearedJson[i]["last_looted_food"].ToString());
                            }
                            if (clearedJson[i]["last_looted_water"].ToString() != "0000-00-00 00:00:00") //this is a blank entry in current DB config
                            {
                                myBuilding.last_looted_water = DateTime.Parse(clearedJson[i]["last_looted_water"].ToString());
                            }

                            //CHECK ITEMS
                            if (clearedJson[i]["has_trap"].ToString() == "1")
                            {
                                myBuilding.has_traps = true;
                            }
                            else
                            {
                                myBuilding.has_traps = false;
                            }
                            if (clearedJson[i]["has_barrel"].ToString() == "1")
                            {
                                myBuilding.has_barrel = true;
                            }
                            else
                            {
                                myBuilding.has_barrel = false;
                            }
                            if (clearedJson[i]["has_greenhouse"].ToString() == "1")
                            {
                                myBuilding.has_greenhouse = true;
                            }
                            else
                            {
                                myBuilding.has_greenhouse = false;
                            }

                            //Debug.Log(myBuilding.buildingName+" Matched buildings, Active: "+clearedJson[i]["active"].ToString());
                            if (clearedJson[i]["active"].ToString() == "0")
                            {
                                //Debug.Log(myBuilding.buildingName+" has been found to be clear- not being activated");
                                myBuilding.ActivateMe(); //all buildings should now be active
                                cleared = 1;
                                break;
                            }

                            
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //ActivateMe is now the catch all building initilizer.
                    myBuilding.ActivateMe();

                    /*
                    if (cleared == 0) {
                        myBuilding.ActivateMe();
                    }
                   */
                }

                
            }
            else
            {
                foreach (GameObject building in buildings)
                {
                    PopulatedBuilding myBuilding = building.GetComponent<PopulatedBuilding>();
                    myBuilding.last_cleared = DateTime.Parse("11:59pm 12/31/1999");
                    myBuilding.ActivateMe();
                }
            }
            #endregion

            #region social location icon updater

            GameObject[] walls = GameObject.FindGameObjectsWithTag("walllocation");

            if (GameManager.instance.myWallsJsonText != "")
            {
                //load json from GameManager for walls the player has tagged
                JsonData wallJson = JsonMapper.ToObject(GameManager.instance.myWallsJsonText);

                foreach (GameObject wall in walls) //cycling through the walls in the currently loaded scene
                {
                    PopulatedBuilding myWall = wall.gameObject.GetComponent<PopulatedBuilding>();
                    myWall.ActivateMe();//initialize the building
                    string my_name = myWall.buildingName;
                    DateTime min_time = DateTime.Now - TimeSpan.FromHours(24);
                    bool match = false;

                    for (int i = 0; i < wallJson.Count; i++)  //cycling through the buildings tagged by the player
                    {
                        DateTime my_tagged_time = DateTime.Parse(wallJson[i]["tag_time"].ToString());
                        
                        if (my_name == wallJson[i]["bldg_name"].ToString() && my_tagged_time >= min_time) //has this player already tagged this location
                        {
                            myWall.Tagged(); //activates the checkmark icon on the non-building locations
                            match = true; //set the local bool
                            Debug.Log(this.gameObject.name + " FOUND A WALL ENTRY MATCH @ " + wallJson[i]["bldg_name"].ToString());
                            break;// exit the for loop
                        }
                    }
                    //if we get through the json loop w/o a match, then we need to be untagged
                    if (match == true)
                    {
                        Debug.Log("There should be a Debug line DIRECTLY ABOVE this- showing the matched/tagged wall object");
                        continue; //go to the next wall from the scene.
                    }
                    else
                    {
                        myWall.UnTagged();
                        Debug.Log(myWall.gameObject.name + " WALL found no matching walls on player JSON- Setting to UNTAGGED");
                    }
                }
            }
            else
            {
                //if there are no tagged buildings returning from the server- then turn all buildings to untagged.
                foreach (GameObject wall in walls)
                {
                    PopulatedBuilding myWall = wall.gameObject.GetComponent<PopulatedBuilding>();
                    myWall.ActivateMe();
                    myWall.UnTagged();
                }
            }

            #endregion

            PlaceHomebaseGraphic();
            //StartCoroutine(GameManager.instance.FetchOutpostData());
            SpawnOutpostsToMap();

            MapLevelManager myMapMgr = FindObjectOfType<MapLevelManager>();
            myMapMgr.levelLoadingPanel.SetActive(false); //at the end of loading main map objects- turn off "loading panel"
            CurrentlyPlacing60Bldgs = false;//turn off the boolean so this may be run again.
        } //this was to prevent multiple calls of this fucntion happening 
    }


    double GetMetersPerPixelOfGoogleMapImage ()
    {
        GoogleMap my_GoogleMap = FindObjectOfType<GoogleMap>();
        int zoom = 0;
        if (my_GoogleMap != null)
        {
            zoom = my_GoogleMap.zoom;
        }else
        {
            zoom = 16;
        }
        float my_lat = 0.0f;
        if (Input.location.status==LocationServiceStatus.Running)
        {
            my_lat = Input.location.lastData.latitude;
        }else
        {
            my_lat = 37.70897f;
        }
        //double my_value = 156543.03392f *( Mathf.Cos((my_lat*Mathf.PI) / 180 ) / Mathf.Pow(2,zoom));
        double my_value = (Mathf.Cos(my_lat*Mathf.PI/180)*2*Mathf.PI*6378137)/(256*Mathf.Pow(2, zoom));
        my_value = my_value * FindObjectOfType<MapLevelManager>().zoomSlider.value; //scale the value to the current zoom level
        //my_value = 2.38865f;
        Debug.Log("Calculating m/pixel of original google image to be: " + my_value);
        return my_value;
    }



    public void PlaceHomebaseGraphic () {
    	float homeLat = GameManager.instance.homebaseLat;
    	float homeLon = GameManager.instance.homebaseLong;

    	if (homeLat != 0.0f && homeLon != 0.0f) {
	    	double deltaLatitude = 0;
	    	double deltaLongitude = 0;
			if (Input.location.status == LocationServiceStatus.Running){
					deltaLatitude = (Input.location.lastData.latitude - homeLat);
					deltaLongitude = (Input.location.lastData.longitude - homeLon);
				}  else {
					deltaLatitude = (37.70883f - homeLat);
					deltaLongitude = (-122.4293 - homeLon);
				}
				double xDistMeters = deltaLongitude * m_per_deg_lon;
				double yDistMeters = deltaLatitude * m_per_deg_lat;
				Debug.Log("homebase calculated to be: "+xDistMeters+"m in theX and "+yDistMeters+"m in the Y direction");

				float xCoord = (float)(screenCenter.x - (xDistMeters));
				float yCoord = (float)(screenCenter.y - (yDistMeters));
				Vector3 pos = new Vector3 (xCoord, yCoord,0);
				homebase.gameObject.SetActive(true);
				homebase.transform.SetParent(bldgHolder.transform);
				homebase.transform.position = pos;
				//Debug.Log("Placing homebase graphic at location: "+pos.ToString());
		}  else {
			Debug.Log("Homebase location not set");
			homebase.gameObject.SetActive(false);
		}
    }

    public void SpawnOutpostsToMap () {
    	//find and destroy any existing outposts
    	GameObject[] expiredOutposts = GameObject.FindGameObjectsWithTag("outpost");
    	foreach(GameObject outpost in expiredOutposts) {
    		Destroy(outpost.gameObject);
    	}

        if (GameManager.instance.outpostJsonText != "")
        {
            //pull the json text from the game manager
            string outpostJsonText = GameManager.instance.outpostJsonText;
            Debug.Log("Outpost JsonData: " + GameManager.instance.outpostJsonText);


            JsonData outpostJSON = JsonMapper.ToObject(GameManager.instance.outpostJsonText);

            //plot outposts to game space, if there are any...
            for (int i = 0; i < outpostJSON.Count; i++)
            {
                float outpostLat = float.Parse(outpostJSON[i]["outpost_lat"].ToString());
                float outpostLng = float.Parse(outpostJSON[i]["outpost_lng"].ToString());
                int outpost_id = (int)outpostJSON[i]["outpost_id"];
                int capacity = (int)outpostJSON[i]["capacity"];
                float myLat = float.Parse(outpostJSON[i]["outpost_lat"].ToString());
                float myLng = float.Parse(outpostJSON[i]["outpost_lng"].ToString());

                double deltaLat = 0;
                double deltaLng = 0;

                if (Input.location.status == LocationServiceStatus.Running)
                {
                    //legit math
                    deltaLat = (Input.location.lastData.latitude - outpostLat);
                    deltaLng = (Input.location.lastData.longitude - outpostLng);
                }
                else
                {
                    //dummy math for unity client
                    deltaLat = (37.70883f - outpostLat);
                    deltaLng = (-122.4293f - outpostLng);
                }

                //Debug.Log("change in lat: "+deltaLat+" change in lng: "+deltaLng);
                //Debug.Log(m_per_deg_lat.ToString()+" "+m_per_deg_lon.ToString());
                double xDistMeters = deltaLat * m_per_deg_lat;
                double yDistMeters = deltaLng * m_per_deg_lon;
                //Debug.Log("outpost located at "+xDistMeters.ToString()+" away in the X, and "+yDistMeters+" meters away in the Y");


                float xCoord = (float)(screenCenter.x - xDistMeters);
                float yCoord = (float)(screenCenter.y - yDistMeters);
                Vector3 pos = new Vector3(xCoord, yCoord, 0);
                GameObject instance = Instantiate(outpostPrefab);
                instance.transform.SetParent(this.gameObject.transform);
                instance.transform.position = pos;
                Outpost myOutpost = instance.GetComponent<Outpost>();
                myOutpost.SetMyOutpostData(capacity, outpost_id, myLat, myLng);
                Debug.Log("Placing outpost at map position: " + pos.ToString());
            }
        }else
        {
            Debug.Log("no outposts to spawn to map");
        }
    }
}
