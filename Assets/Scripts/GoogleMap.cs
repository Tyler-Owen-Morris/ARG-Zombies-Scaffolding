using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoogleMap : MonoBehaviour
{
	public enum MapType
	{
		RoadMap,
		Satellite,
		Terrain,
		Hybrid
	}
	public bool loadOnStart = true;
	public bool autoLocateCenter = true;
	public GoogleMapLocation centerLocation;
	public int zoom;
	public MapType mapType;
	public int size = 640;
	public bool doubleResolution = false;
	public GoogleMapMarker[] markers;
	public GoogleMapPath[] paths;
	
	void Start() {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            centerLocation.latitude = Input.location.lastData.latitude;
            centerLocation.longitude = Input.location.lastData.longitude;
        }
        else
        {
            //this.gameObject.SetActive(false);
        }

        if (loadOnStart) Refresh();

	}
	
	public void Refresh() {
		if(autoLocateCenter && (markers.Length == 0 && paths.Length == 0)) {
			Debug.LogError("Auto Center will only work if paths or markers are used.");	
		}
		StartCoroutine(_Refresh());
	}
	
	IEnumerator _Refresh ()
	{
		var url = "http://maps.googleapis.com/maps/api/staticmap";
		var qs = "";
		if (!autoLocateCenter) {
			if (centerLocation.address != "")
				qs += "center=" + WWW.UnEscapeURL (centerLocation.address);
			else {
				qs += "center=" + WWW.UnEscapeURL (string.Format ("{0},{1}", centerLocation.latitude, centerLocation.longitude));
			}
		
			qs += "&zoom=" + zoom.ToString ();
		}
        int wide = Screen.width / 2;
        int high = Screen.height / 2;
        if (wide > 1280 || high > 1280)
        {
            wide = wide / 2;
            high = high / 2;
        }
		qs += "&size=" + WWW.UnEscapeURL (string.Format ("{0}x{1}", wide, high));
		qs += "&scale=" + (doubleResolution ? "2" : "1");
		qs += "&maptype=" + mapType.ToString ().ToLower ();
		var usingSensor = false;
#if UNITY_IPHONE
		usingSensor = Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running;
#endif
		qs += "&sensor=" + (usingSensor ? "true" : "false");
		
		foreach (var i in markers) {
			qs += "&markers=" + string.Format ("size:{0}|color:{1}|label:{2}", i.size.ToString ().ToLower (), i.color, i.label);
			foreach (var loc in i.locations) {
				if (loc.address != "")
					qs += "|" + WWW.UnEscapeURL (loc.address);
				else
					qs += "|" + WWW.UnEscapeURL (string.Format ("{0},{1}", loc.latitude, loc.longitude));
			}
		}
		
		foreach (var i in paths) {
			qs += "&path=" + string.Format ("weight:{0}|color:{1}", i.weight, i.color);
			if(i.fill) qs += "|fillcolor:" + i.fillColor;
			foreach (var loc in i.locations) {
				if (loc.address != "")
					qs += "|" + WWW.UnEscapeURL (loc.address);
				else
					qs += "|" + WWW.UnEscapeURL (string.Format ("{0},{1}", loc.latitude, loc.longitude));
			}
		}
		
		
		var req = new WWW(url + "?" + qs);
        yield return req;
        //GetComponent<Renderer>().material.mainTexture = req.texture;
        int width = req.texture.width;
        int height = req.texture.height;
        Debug.Log("Screen HxW: " + Screen.height + ", " + Screen.width + "  google image dimensions: " + height + ", " + width);
        Sprite map_pic = Sprite.Create(req.texture, new Rect(0, 0, width, height), new Vector2());
        GetComponent<Image>().sprite = map_pic;
        GetComponent<SpriteRenderer>().sprite = map_pic;
        //InvertMySprite();
	}
	
    void InvertMySprite ()
    {
        Image my_sprite = GetComponent<Image>();
        Color inverted = Color.white - my_sprite.color;
        my_sprite.color = inverted;
    }
	
}

public enum GoogleMapColor
{
	black,
	brown,
	green,
	purple,
	yellow,
	blue,
	gray,
	orange,
	red,
	white
}

[System.Serializable]
public class GoogleMapLocation
{
	public string address;
	public float latitude;
	public float longitude;
}

[System.Serializable]
public class GoogleMapMarker
{
	public enum GoogleMapMarkerSize
	{
		Tiny,
		Small,
		Mid
	}
	public GoogleMapMarkerSize size;
	public GoogleMapColor color;
	public string label;
	public GoogleMapLocation[] locations;
	
}

[System.Serializable]
public class GoogleMapPath
{
	public int weight = 5;
	public GoogleMapColor color;
	public bool fill = false;
	public GoogleMapColor fillColor;
	public GoogleMapLocation[] locations;	
}