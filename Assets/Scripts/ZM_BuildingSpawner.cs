using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;
using System;
using UnityEngine.UI;

public class ZM_BuildingSpawner : MonoBehaviour {

	public float lastGoogleLat = 0, lastGoogleLng = 0;
	public double m_per_deg_lat, m_per_deg_lon;
	public bool bldgsNeedUpdate;

	private Vector3 screenCenter = new Vector3((Screen.width*0.5f), (Screen.height*0.5f), 0.0f);
	private string googlePlacesAPIURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
	private string googleNextPagePlacesURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?pagetoken=";
	private string googleAPIKey = "AIzaSyC0Ly6W_ljFCk5sV4n-T73e-rhRdNpiEe4";
	//private static GameObject gameCanvas;

	private ZM_Building buildingPrefab;

	void Awake () {
		bldgsNeedUpdate = true;
		//gameCanvas = GameObject.Find("Canvas");
		buildingPrefab = Resources.Load<ZM_Building>("Prefabs/ZM Populated Building");
		if (buildingPrefab == null) {
			Debug.LogWarning ("unable to load building prefab");
		}
	}

	public void UpdateBuildings () {

		//each pair of fucntions goes together. 60 is supposed to accomodate page refresh, and no number is the default 20 results.
		if (bldgsNeedUpdate == true) {
			//StartCoroutine(GetNearbyBuildingsGoogle());
			StartCoroutine(GetGoogleBuildings());
		}  else {
			TurnGoogleJsonIntoBuildings();
			//Turn60GoogleJsonIntoBuildings();
		}
	}

	IEnumerator GetGoogleBuildings () {
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
		myWwwString += "&radius=500";
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
		bldgsNeedUpdate = false;	
	}


	private void TurnGoogleJsonIntoBuildings () {
		//destroy the existing buildings
		GameObject[] oldBldgs = GameObject.FindGameObjectsWithTag("building");
		foreach (GameObject oldBldg in oldBldgs) {
			Destroy(oldBldg.gameObject);
		}

		//string jsonString = File.ReadAllText(Application.dataPath + "/Resources/googlelocations.json");
		Debug.Log(GameManager.instance.locationJsonText);
		JsonData bldgJson = JsonMapper.ToObject(GameManager.instance.locationJsonText);
		//Debug.Log(JsonMapper.ToJson(bldgJson));
		//JsonData foursquareJson = JsonMapper.ToObject(jsonReturn);

		double m_per_pixel_mapBG = GetMetersPerPixelOfGoogleMapImage();
		int map_img_size = Mathf.FloorToInt(Screen.height/2);
		double map_height_in_meters = (m_per_pixel_mapBG*map_img_size);
		double m_per_screen_pixel = map_height_in_meters / Screen.height;
		Debug.Log("Calculating m/px-BG: " + m_per_pixel_mapBG + "  Map width in meters: "+map_height_in_meters+"  and meters/pixel for building placement: " + m_per_screen_pixel);



		//cycle through json, placing and setting up zombie buildings
		for (int i = 0; i < bldgJson["results"].Count; i++) {
			//parse all relevant information for this location out of the json
			string myName = (string)bldgJson["results"][i]["name"];
			string myBldgID = (string)bldgJson["results"][i]["id"];
			JsonData thisEntry = JsonMapper.ToObject(JsonMapper.ToJson(bldgJson["results"][i]));
			string my_photo_ref = "";
			if (thisEntry.Keys.Contains("photos")) {
				my_photo_ref = (string)bldgJson["results"][i]["photos"][0]["photo_reference"];
			}
			float lat = (float)(double)bldgJson["results"][i]["geometry"]["location"]["lat"];
			float lng = (float)(double)bldgJson["results"][i]["geometry"]["location"]["lng"];

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



			ZM_Building instance = Instantiate(buildingPrefab);
			instance.SetUpTheBuilding (myName, myBldgID, lat, lng, my_photo_ref);

			float xCoord = (float)(screenCenter.x - (xScreenDist));
			float yCoord = (float)(screenCenter.y - (yScreenDist));
			Vector3 pos = new Vector3 (xCoord, yCoord, 0);

			instance.transform.SetParent(gameObject.transform);
			instance.transform.position = pos;
			instance.gameObject.transform.localScale = new Vector3 (1, 1, 1);

		}

		DeactivateBaitedBuildings();
	}

	void DeactivateBaitedBuildings () {
		if(GameManager.instance.myBaitedJsonText != ""){
			JsonData baited_json = JsonMapper.ToObject(GameManager.instance.myBaitedJsonText);
			GameObject[] zm_buildings = GameObject.FindGameObjectsWithTag("building");

			for (int i = 0 ; i < baited_json.Count; i++) {
				foreach (GameObject building in zm_buildings) {
					ZM_Building myBuildingScript = building.GetComponent<ZM_Building>();
					if (myBuildingScript.buildingID == baited_json[i]["building_id"].ToString()){
						myBuildingScript.ThisBuildingIsBaited();
					}
				}
			}
		}else{
			Debug.LogWarning("No baited buildings found on GameManager record");
		}
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
		//my_value = my_value * FindObjectOfType<ZombieModeManager>().zoomSlider.value; //scale the value to the current zoom level
		//my_value = 2.38865f;
		Debug.Log("Calculating m/pixel of original google image to be: " + my_value);
		return my_value;
	}

}
