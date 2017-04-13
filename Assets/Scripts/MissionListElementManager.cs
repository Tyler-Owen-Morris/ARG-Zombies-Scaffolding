using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MissionListElementManager : MonoBehaviour {

	public Slider myTimeSlider;
	public Text mySliderText, myBldgNameText;

	public int myMissionID;
	public string building_name;
	public float duration_minutes;
	public DateTime time_complete;

	void Start () {
		this.transform.localScale = new Vector3(1,1,1);
	}
	

	void Update () {
		//update the slider and counter every frame. when the time reaches complete, call out to spawn the acknowledge popup
		if (time_complete != null && time_complete < DateTime.Now) {
			//this mission is complete, spawn the popup window.
			CompleteThisMission();

		} else {
			//this mission is active, calculate and set the slider and text.
			TimeSpan to_complete = time_complete - DateTime.Now;
			TimeSpan duration = TimeSpan.FromMinutes(duration_minutes);
			float inverse_value = (float)(to_complete.TotalMinutes/duration.TotalMinutes);
			float slider_value = 1.0f - inverse_value;
			myTimeSlider.value = slider_value;

			//construct the clock text
			string clock_text = "";
			if (to_complete.Hours > 0) {
				clock_text += to_complete.Hours.ToString().PadLeft(2, '0')+":";
				to_complete = to_complete - TimeSpan.FromHours(to_complete.Hours);
			}
			if(to_complete.Minutes > 0){
				clock_text += to_complete.Minutes.ToString().PadLeft(2, '0')+":";
				to_complete = to_complete - TimeSpan.FromMinutes(to_complete.Minutes);
			}
			if(to_complete.Seconds > 0) {
				clock_text += to_complete.Seconds.ToString().PadLeft(2, '0');
			}

			//set the counter text
			mySliderText.text = clock_text;
		}
	}

	//this should take in all the data and set it within the class.
	public void SetData (int miss_id, float duration, DateTime time_comp, string bldg_name) {
		myMissionID = miss_id;
		duration_minutes = duration;
		time_complete = time_comp;
		building_name = bldg_name;
		myBldgNameText.text = building_name;
	}

	public bool missionCompleteSent = false;
	void CompleteThisMission () {
		if(missionCompleteSent == false) {
			missionCompleteSent = true;
			//send the data to the map level manager function which spawns the popup window with all needed data.
			MapLevelManager myMapLvlMgr = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();

			//call the function to spawn mission completion confirmation.
			myMapLvlMgr.NewMissionComplete(myMissionID);

			Destroy(this.gameObject);
		} 
	}
}
