using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System;


public class BuildingSpawner : MonoBehaviour {

	public GameObject homebase;
	public GameObject bldgHolder;
	public GameObject outpostPrefab;
	public float lastGoogleLat = 0, lastGoogleLng = 0;

	private float minX, maxX, minY, maxY;
	public double m_per_deg_lat, m_per_deg_lon;

	private Vector3 screenCenter = new Vector3((Screen.width*0.5f), (Screen.height*0.5f), 0.0f);
	private string googlePlacesAPIURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
	private string googleAPIKey = "AIzaSyC0Ly6W_ljFCk5sV4n-T73e-rhRdNpiEe4";

	public bool googleBldgsNeedUpdate;

	private static GameObject gameCanvas;
    private static PopulatedBuilding populatedBuildingPrefab;


    void Start () {
		googleBldgsNeedUpdate = true;
        gameCanvas = GameObject.Find("Canvas");
        populatedBuildingPrefab = Resources.Load<PopulatedBuilding>("Prefabs/Populated Building");

		StartCoroutine(GetNearbyBuildingsGoogle());
    }

	public void UpdateBuildings () {

		if (googleBldgsNeedUpdate == true) {
			StartCoroutine(GetNearbyBuildingsGoogle());
		}  else {
			TurnGoogleJsonIntoBuildings();
		}
	}

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
			instance.myLat = lat;
			instance.myLng = lng;
			float xCoord = (float)(screenCenter.x - (xScreenDist));
			float yCoord = (float)(screenCenter.y - (yScreenDist));
			Vector3 pos = new Vector3 (xCoord, yCoord, 0);

			instance.transform.SetParent(bldgHolder.transform);
			instance.transform.position = pos;

			//determine the loot class of the building
			string type_bldg = bldgJson["results"][i]["types"][0].ToString();
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

                        if (clearedJson[i]["last_looted_supply"].ToString() != "0000-00-00 00:00:00") //this is a blank entry in current DB config
                        { 
                            myBuilding.last_looted_supply = DateTime.Parse(clearedJson[i]["last_looted_supply"].ToString());
                        }
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
