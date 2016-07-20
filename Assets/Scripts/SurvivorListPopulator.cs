using UnityEngine;
using System.Collections;

public class SurvivorListPopulator : MonoBehaviour {

	private static GameObject listElementPrefab;

	void Start () {
		//listElementPrefab = Resources.Load<GameObject>("Prefabs/SurvivorElementHolder");
		listElementPrefab = Resources.Load<GameObject>("Prefabs/SurvivorListItemPanel");

		RefreshFromGameManagerList();
	}

	//moved this into a public function so it could be called externally upon changes to the list.
	public void RefreshFromGameManagerList () {
		//For each gameobject in GameManager.instance.survivorcardlist instantiate a list item, and populate it's data.
		foreach(GameObject survivorCard in GameManager.instance.survivorCardList) {
			//get the card data from the object in GameManager
			SurvivorPlayCard survPlayCard = survivorCard.GetComponent<SurvivorPlayCard>();
			Debug.Log("loopadooba");

			//create the list item and parent it to the populator
			GameObject instance = Instantiate(listElementPrefab);
			instance.transform.SetParent(gameObject.transform);

			//set the list item data to match the card object data
			SurvivorListElementManager  SLEM = instance.GetComponent<SurvivorListElementManager>();
			SLEM.survivorNameText.text = survPlayCard.survivor.name;
			string myStatsString = "";
			myStatsString += survPlayCard.survivor.baseAttack +" Attk ";
			myStatsString += survPlayCard.survivor.curStamina + " stam";
			if (survPlayCard.survivor.weaponEquipped != null) {
				myStatsString += " wielding a " + survPlayCard.survivor.weaponEquipped.name;
			}else{
				myStatsString += " and is Unarmed";
			}
			SLEM.survivorStatsText.text = myStatsString; 
			SLEM.mySurvivorCard = survivorCard;

			if (survPlayCard.team_pos <= 5 ) {
				SLEM.TurnOffMyTeamButton();
			}
		}
	}

}
