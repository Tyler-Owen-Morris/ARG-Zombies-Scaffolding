using UnityEngine;
using System.Collections;

public class Outpost : MonoBehaviour {

	public int capacity;
	public int outpost_id;
	public float outpost_lat, outpost_lng;

	private MapLevelManager mapManager;

	void Start () {
		mapManager = MapLevelManager.FindObjectOfType<MapLevelManager>();
	}

	public void SetMyOutpostData (int cap, int post_id, float lat, float lng) {
		capacity = cap;
		outpost_id = post_id;
		outpost_lat = lat;
		outpost_lng = lng;
        this.gameObject.SetActive(true);
        this.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
	}

	public void OutpostPressed () {
		if (capacity > 0) {
			//open the display barcode to add players.
			Debug.Log("open qr display panel for outpost, and encode/display qr code");

			mapManager.OutpostPressed(outpost_id);
		} else {
			//do nothing.
			Debug.Log("outpost does not have free slots");
		}
	}
}
