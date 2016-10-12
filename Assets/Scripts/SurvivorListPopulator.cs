﻿using UnityEngine;
using System.Collections;

public class SurvivorListPopulator : MonoBehaviour {

	public static GameObject listElementPrefab;

	void Start () {
		//listElementPrefab = Resources.Load<GameObject>("Prefabs/SurvivorElementHolder");
		listElementPrefab = Resources.Load<GameObject>("Prefabs/SurvivorListItemPanel");

		RefreshFromGameManagerList();
	}

	//moved this into a public function so it could be called externally upon changes to the list.
	public void RefreshFromGameManagerList () {

		//find any existing list game objects, and destroy them.
		GameObject[] oldSurvivorListElements = GameObject.FindGameObjectsWithTag("survivorlistelement");
		foreach (GameObject oldSurv in oldSurvivorListElements) {
			Destroy(oldSurv.gameObject);
		}

		//For each gameobject in GameManager.instance.activeSurvivorcardlist instantiate a list item, and populate it's data.
		foreach(GameObject survivorCard in GameManager.instance.activeSurvivorCardList) {
			//get the card data from the object in GameManager
			SurvivorPlayCard survPlayCard = survivorCard.GetComponent<SurvivorPlayCard>();
			//Debug.Log("loopadooba");

			//create the list item and parent it to the populator
			GameObject instance = Instantiate(listElementPrefab);
			instance.transform.SetParent(gameObject.transform);

			//set the list item data to match the card object data
			SurvivorListElementManager  SLEM = instance.GetComponent<SurvivorListElementManager>(); 
			SLEM.mySurvivorCard = survivorCard;

			if (survPlayCard.team_pos >= 1 ) {
				SLEM.TurnOffMyTeamButton();
			}

			if (survPlayCard.onMission == true) {
				SLEM.SetToOnMission();
			} else {
				SLEM.myMissionText.gameObject.SetActive(false);
			}
		}

		//For each gameobject in GameManager.instance.onMissionSurvivorcardlist instantiate a list item, and populate it's data.
		foreach(GameObject survivorCard in GameManager.instance.onMissionSurvivorCardList) {
			//get the card data from the object in GameManager
			SurvivorPlayCard survPlayCard = survivorCard.GetComponent<SurvivorPlayCard>();
			//Debug.Log("loopadooba");

			//create the list item and parent it to the populator
			GameObject instance = Instantiate(listElementPrefab);
			instance.transform.SetParent(gameObject.transform);

			//set the list item data to match the card object data
			SurvivorListElementManager  SLEM = instance.GetComponent<SurvivorListElementManager>(); 
			SLEM.mySurvivorCard = survivorCard;

			if (survPlayCard.team_pos >= 1 ) {
				SLEM.TurnOffMyTeamButton();
			}

			if (survPlayCard.onMission == true) {
				SLEM.SetToOnMission();
			} else {
				SLEM.myMissionText.gameObject.SetActive(false);
			}
		}
	}

}