using UnityEngine;
using System.Collections;
using LitJson;
using System;

public class MissionListPopulator : MonoBehaviour {

	//declare the prefab for the list element
	private static GameObject missionListElementPrefab;

	void Awake () {
		missionListElementPrefab = Resources.Load<GameObject>("Prefabs/MissionListElementPanel");
		LoadMissionsFromGameManager();
	}

	public void LoadMissionsFromGameManager () {
		//find any existing elements in the list and destroy them
		GameObject[] oldMissionListElements = GameObject.FindGameObjectsWithTag("missionlistelement");
		foreach (GameObject oldMiss in oldMissionListElements) {
			Destroy(oldMiss.gameObject);
		}

		if (GameManager.instance.missionJsonText != "") {
			//load in the json, and go through it creating, and populating the missions to the scroll list.
			JsonData mission_json = JsonMapper.ToObject(GameManager.instance.missionJsonText);

			for(int i=0; i < mission_json[1].Count; i++) {
				//instantiate the prefab
				GameObject instance = Instantiate(missionListElementPrefab);
				instance.transform.SetParent(this.gameObject.transform);

				//pull the data from json into C# variables
				int mission_id = (int)mission_json[1][i]["mission_id"];
				string building_name = mission_json[1][i]["building_name"].ToString();
				float mission_duration = float.Parse(mission_json[1][i]["duration"].ToString());
				DateTime complete_time = DateTime.Parse(mission_json[1][i]["time_complete"].ToString());

				//call the function on the prefab to set it's data
				instance.GetComponent<MissionListElementManager>().SetData(mission_id, mission_duration, complete_time, building_name);
			}
		}
	}
}
