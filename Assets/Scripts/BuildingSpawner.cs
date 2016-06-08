using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;


public class BuildingSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject[] buildings;

	private float minX, maxX, minY, maxY;

	private string googlePlacesAPIURL = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
    private string foursquareAPIURL = "https://api.foursquare.com/v2/venues/search";
	private string googleAPIKey = "AIzaSyC0Ly6W_ljFCk5sV4n-T73e-rhRdNpiEe4";
    private string foursquareClientId = "EHIQB3LGEP5IZSM5TENELQ3IJ4QSR2QZRSZK3PZDN4ZGMH2G";
    private string foursquareClientSecret = "WPTC5HRUGQ5E02P0AME5UCSPUYXQTCBL0SX4LVK0WH4LIX1Q";
    private string foursquareJsonReturn;
    private string googleJsonReturn;
    
    private static GameObject gameCanvas;
    private static GameObject bldgHolder;
    private static PopulatedBuilding populatedBuildingPrefab;

	// Use this for initialization
	void Start () {
        gameCanvas = GameObject.Find("Canvas");
        bldgHolder = GameObject.Find("BuildingHolder");
        populatedBuildingPrefab = Resources.Load<PopulatedBuilding>("Prefabs/Populated Building");
		CreateBuildings();
        //StartCoroutine(GetNearbyBuildingsFoursquare());
        StartCoroutine(GetNearbyBuildingsGoogle());
	}

	IEnumerator GetNearbyBuildingsGoogle () {
		string myWwwString = googlePlacesAPIURL;
		myWwwString += "?location=";
		if (Input.location.status == LocationServiceStatus.Running) {
			myWwwString += Input.location.lastData.latitude +","+Input.location.lastData.longitude;
		} else {
			myWwwString += "37.70897,-122.4292";
			//this is assuming my home location
		}
		myWwwString += "&radius=500";
		myWwwString += "&key="+ googleAPIKey;

		WWW www = new WWW(myWwwString);
		yield return www;

		//File.WriteAllText(Application.dataPath + "/Resources/googlelocations.json", www.text.ToString());
		googleJsonReturn = www.text;
		GameManager.instance.locationJsonText = www.text;
		TurnGoogleJsonIntoBuildings();

	}

	void TurnGoogleJsonIntoBuildings () {
    	
		//string jsonString = File.ReadAllText(Application.dataPath + "/Resources/googlelocations.json");
		string jsonString = googleJsonReturn;
		JsonData bldgJson = JsonMapper.ToObject(jsonString);
        //JsonData foursquareJson = JsonMapper.ToObject(jsonReturn);
       
       
        //this loop is currently only set to do 4 iterations. AKA- first 4 locations on the list.
        for (int i = 0; i < bldgJson["results"].Count; i++) {
			string myName = (string)bldgJson["results"][i]["name"];
        	float lat = (float)(double)bldgJson["results"][i]["geometry"]["location"]["lat"];
			float lng = (float)(double)bldgJson["results"][i]["geometry"]["location"]["lng"];
			
			//Debug.Log (name + lat + lng);

			//calculate the average latitude between the two locations, and then calculate the meters/DEGREE lat/lon
			float latMid =(Input.location.lastData.latitude + lat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			//Debug.Log ("for the " + name + " building, meters per degree calculated as " + m_per_deg_lat + " m/deg lat, and " + m_per_deg_lon +" m/deg lon");
			double deltaLatitude = 0;
			double deltaLongitude = 0;
			if (Input.location.status == LocationServiceStatus.Running){
				deltaLatitude = (Input.location.lastData.latitude - lat);
				deltaLongitude = (Input.location.lastData.longitude - lng);
			} else {
				deltaLatitude = (37.70883f - lat);
				deltaLongitude = (-122.4293 - lng);
			}
			double xDistMeters = deltaLongitude * m_per_deg_lon;
			double yDistMeters = deltaLatitude * m_per_deg_lat;

			//Debug.Log ("for "+myName+" change in lat/lng is "+deltaLatitude+" "+deltaLongitude+" and x/y dist calculated to be: "+ xDistMeters+" "+yDistMeters);
			/*
			float earthRadius = 6378.137f; // in KM
			double dLat = (lat - Input.location.lastData.latitude) * Mathf.Deg2Rad; //convert angular difference to radians
			double dLng = (lng - Input.location.lastData.longitude) * Mathf.Deg2Rad;
			double xDistKm = dLng * earthRadius;
			double yDistKm = dLat * earthRadius;
			float xDistM = (float)(xDistKm * 1000);
			float yDistM = (float)(yDistKm * 1000);
			*/
			//this was my first pass at the math.  Deg2Rad is not solid enough, nor does it take into account lat-long characteristics
			
			//Debug.Log ("The building named "+ name +" should be appearing " + xDistMeters+" meters in the x direction and " + yDistMeters + " meters in the y direction");
			//now we have the realive distance in meters translated to x,y offsets from our origin.
			//current Gamespace renders buildings x:260-1030 and y:30-580 -PROVIDED objects are childed to the canvas
			//origin is at 385,275 all buildings should be placed there with appropriate relative distance.
			
			PopulatedBuilding instance = Instantiate(populatedBuildingPrefab);
			instance.name = myName;
			instance.buildingName = myName;
			float xCoord = (float)(385 - (xDistMeters));
			float yCoord = (float)(275 - (yDistMeters));
			Vector3 pos = new Vector3 (xCoord, yCoord, 0);

			//instance.transform.SetParent(gameCanvas.transform);
			instance.transform.SetParent(bldgHolder.transform);
			instance.transform.position = pos;
				
			//Debug.Log("placed "+instance.name+" at coords: "+xCoord+" x and "+yCoord+" y");
      }

      StartCoroutine(GameManager.instance.DeactivateClearedBuildings());
        
    }



    //this is the same thing as the google call... just a different api.
    IEnumerator GetNearbyBuildingsFoursquare () {
        string myWwwString = foursquareAPIURL;
        myWwwString += "?client_id=" + foursquareClientId;
        myWwwString += "&client_secret=" + foursquareClientSecret;
        
        //foursquare API requires single digit months/days to have a 0 before them
        string day = System.DateTime.Now.Day.ToString();
        if (day.Length == 1) {
            day = "0"+ System.DateTime.Now.Day;
        }
        string month = System.DateTime.Now.Month.ToString();
        if (month.Length == 1){
            month = "0" + System.DateTime.Now.Month;
        }
            
        myWwwString += "&v=" + System.DateTime.Now.Year + month + day;
        if (Input.location.status == LocationServiceStatus.Running) {
            myWwwString += "&ll="+ Input.location.lastData.latitude +","+ Input.location.lastData.longitude;
            
            Debug.Log ("This is the string we are submitting to the api " + myWwwString);
            WWW www = new WWW(myWwwString);
            yield return www;
            
            foursquareJsonReturn = www.text;
            TurnJsonIntoBuildings();
            Debug.Log ("successful 4square api call. Return is: " + www.text);
            
        }else {
            myWwwString += "&ll=37.70884,-122.4292";
            
            WWW www = new WWW(myWwwString);
            yield return www;
            Debug.Log ("Location services aren't running, but a 4square api search near SF returned: "+www.text);
            
			File.WriteAllText(Application.dataPath + "/Resources/foursquarelocations.json", www.text.ToString());
            foursquareJsonReturn = www.text;
            TurnJsonIntoBuildings();
            
            GameManager.instance.PublicStartLocationServices();
            //StartCoroutine(GetNearbyBuildings());
            yield break;
        }
    }
    
    void TurnJsonIntoBuildings () {
    	
		string jsonString = File.ReadAllText(Application.dataPath + "/Resources/foursquarelocations.json");
		JsonData bldgJson = JsonMapper.ToObject(jsonString);
        //JsonData foursquareJson = JsonMapper.ToObject(jsonReturn);
       
       
        //this loop is currently only set to do 4 iterations. AKA- first 4 locations on the list.
        for (int i = 0; i < bldgJson["response"]["venues"].Count; i++) {
			string bldgName = (string)bldgJson["response"]["venues"][i]["name"];
        	float lat = (float)(double)bldgJson["response"]["venues"][i]["location"]["lat"];
			float lng = (float)(double)bldgJson["response"]["venues"][i]["location"]["lng"];
			
			Debug.Log (name + lat + lng);

			//calculate the average latitude between the two locations, and then calculate the meters/DEGREE lat/lon
			float latMid = lat;//(Input.location.lastData.latitude + lat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * (Mathf.Cos( 2 * latMid )) + (1.175 * Mathf.Cos( 4 * latMid));
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			Debug.Log ("for the " + bldgName + " building, meters per degree calculated as " + m_per_deg_lat + " m/deg lat, and " + m_per_deg_lon +" m/deg lon");

			double deltaLatitude = (Input.location.lastData.latitude - lat);
			double deltaLongitude = (Input.location.lastData.longitude - lng);
			double xDistMeters = deltaLongitude * m_per_deg_lon;
			double yDistMeters = deltaLatitude * m_per_deg_lat;

			/*
			float earthRadius = 6378.137f; // in KM
			double dLat = (lat - Input.location.lastData.latitude) * Mathf.Deg2Rad; //convert angular difference to radians
			double dLng = (lng - Input.location.lastData.longitude) * Mathf.Deg2Rad;
			double xDistKm = dLng * earthRadius;
			double yDistKm = dLat * earthRadius;
			float xDistM = (float)(xDistKm * 1000);
			float yDistM = (float)(yDistKm * 1000);
			*/
			//this was my first pass at the math.  Deg2Rad is not solid enough, nor does it take into account lat-long characteristics
			
			Debug.Log ("The building named "+ name +" should be appearing " + xDistMeters+" meters in the x direction and " + yDistMeters + " meters in the y direction");
			//now we have the realive distance in meters translated to x,y offsets from our origin.
			//current Gamespace renders buildings x:260-1030 and y:30-580 -PROVIDED objects are childed to the canvas
			//origin is at 385,275 all buildings should be placed there with appropriate relative distance.
			
			PopulatedBuilding instance = Instantiate(populatedBuildingPrefab);
			instance.buildingName = bldgName;
			float xCoord = (float)(385 + (xDistMeters/10000));
			float yCoord = (float)(275 + (yDistMeters/10000));
			Vector3 pos = new Vector3 (xCoord, yCoord, 0);

			instance.transform.SetParent(gameCanvas.transform);
			instance.transform.position = pos;
			
			Debug.Log("placed "+instance.name+" at coords: "+xCoord+" x and "+yCoord+" y");
      }
        
    }

	void CreateBuildings () {
		for (int i = 0; i < buildings.Length; i++) {		
			Vector3 temp = buildings[i].transform.position;
			Debug.Log ("the building "+ buildings[i].name + " is currently located at " + temp);
			//Buildings are relocated to values derrived from world space- RectTransform and Transform are different, but this is flubbed to make it work.
			temp.x = Random.Range (260, 590);
			temp.y = Random.Range (38, 590);
			//Debug.Log("and is being relocated to " + temp.x +"-X and " + temp.y + "-y");
			buildings[i].transform.position = temp;
		}
	} 
	
	// Update is called once per frame
	void Update () {
	
	}
}
