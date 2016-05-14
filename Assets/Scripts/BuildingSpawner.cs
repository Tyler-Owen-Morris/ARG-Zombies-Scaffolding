using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;


public class BuildingSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject[] buildings;

	private float minX, maxX, minY, maxY;
    
    private string foursquareAPIURL = "https://api.foursquare.com/v2/venues/search";
    private string foursquareClientId = "EHIQB3LGEP5IZSM5TENELQ3IJ4QSR2QZRSZK3PZDN4ZGMH2G";
    private string foursquareClientSecret = "WPTC5HRUGQ5E02P0AME5UCSPUYXQTCBL0SX4LVK0WH4LIX1Q";
    private string jsonReturn;
    
    private static GameObject gameCanvas;
    private static PopulatedBuilding populatedBuildingPrefab;

	// Use this for initialization
	void Start () {
        gameCanvas = GameObject.Find("Canvas");
        populatedBuildingPrefab = Resources.Load<PopulatedBuilding>("Prefabs/Populated Building");
		CreateBuildings();
        StartCoroutine(GetNearbyBuildings());
	}
    
    IEnumerator GetNearbyBuildings () {
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
            
            jsonReturn = www.text;
            TurnJsonIntoBuildings();
            Debug.Log ("successful 4square api call. Return is: " + www.text);
            
        }else {
            myWwwString += "&ll=37.70884,-122.4292";
            
            WWW www = new WWW(myWwwString);
            yield return www;
            Debug.Log ("Location services aren't running, but a 4square api search near SF returned: "+www.text);
            
			File.WriteAllText(Application.dataPath + "/Resources/locations.json", www.text.ToString());
            jsonReturn = www.text;
            TurnJsonIntoBuildings();
            
            GameManager.instance.PublicStartLocationServices();
            //StartCoroutine(GetNearbyBuildings());
            yield break;
        }
    }
    
    void TurnJsonIntoBuildings () {
    	
		string jsonString = File.ReadAllText(Application.dataPath + "/Resources/locations.json");
		JsonData bldgJson = JsonMapper.ToObject(jsonString);
        //JsonData foursquareJson = JsonMapper.ToObject(jsonReturn);
       
       
        //this loop is currently only set to do 4 iterations. AKA- first 4 locations on the list.
        for (int i = 0; i < bldgJson["response"]["venues"].Count; i++) {
			string name = (string)bldgJson["response"]["venues"][i]["name"];
        	float lat = (float)(double)bldgJson["response"]["venues"][i]["location"]["lat"];
			float lng = (float)(double)bldgJson["response"]["venues"][i]["location"]["lng"];
			
			Debug.Log (name + lat + lng);
			
			float earthRadius = 6378.137f; // in KM
			double dLat = (lat - Input.location.lastData.latitude) * Mathf.Deg2Rad; //convert angular difference to radians
			double dLng = (lng - Input.location.lastData.longitude) * Mathf.Deg2Rad;
			double xDistKm = dLng * earthRadius;
			double yDistKm = dLat * earthRadius;
			float xDistM = (float)(xDistKm * 1000);
			float yDistM = (float)(yDistKm * 1000);
			
			
			Debug.Log ("The building named "+ name +" should be appearing " + xDistM+" meters in the x direction and " + yDistM + " meters in the y direction");
			//now we have the realive distance in meters translated to x,y offsets from our origin.
			//current Gamespace renders buildings x:260-1030 and y:30-580 -PROVIDED objects are childed to the canvas
			//origin is at 385,275 all buildings should be placed there with appropriate relative distance.
			
			PopulatedBuilding instance = Instantiate(populatedBuildingPrefab);
			instance.name = name;
			float xCoord = 385 + (xDistM/10);
			float yCoord = 275 + (yDistM/10);
			Vector3 pos = new Vector3 (xCoord, yCoord, 0);
			instance.transform.position = pos;
			instance.transform.SetParent(gameCanvas.transform);
			
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
