using UnityEngine;
using System.Collections;

public class BuildingSpawner : MonoBehaviour {

	[SerializeField]
	private GameObject[] buildings;

	private float minX, maxX, minY, maxY;

	// Use this for initialization
	void Start () {
		CreateBuildings();
	}

	void CreateBuildings () {
		for (int i = 0; i < buildings.Length; i++) {		
			Vector3 temp = buildings[i].transform.position;
			//Debug.Log ("the building "+ buildings[i].name + " is currently located at " + temp);
			//Buildings are relocated to values derrived from world space- RectTransform and Transform are different, but this is flubbed to make it work.
			temp.x = Random.Range (190, 920);
			temp.y = Random.Range (33, 570);
			//Debug.Log("and is being relocated to " + temp.x +"-X and " + temp.y + "-y");
			buildings[i].transform.position = temp;
		}
	} 
	
	// Update is called once per frame
	void Update () {
	
	}
}
