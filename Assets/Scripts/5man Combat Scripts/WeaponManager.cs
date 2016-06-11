using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour {

	public GameObject knife;
	public GameObject club;
	public GameObject gun;

	public GameObject selectedSurvivor;

	private BattleStateMachine BSM;

	void Start () {
		BSM = FindObjectOfType<BattleStateMachine>();
		LocateAndSetSelectedSurvivor();
	}
	
	public void KnifeSelectPressed () {
		LocateAndSetSelectedSurvivor();
		SurvivorStateMachine SSM = selectedSurvivor.GetComponent<SurvivorStateMachine>();
		if (SSM.survivor.weaponEquipped != null) {
			//Unequip the weapon- add it back to gameManager #'s and server

			//subtract the weapon from inventory and add it to the player

			SSM.survivor.weaponEquipped = knife;
		} else {
			//remove weapon from gamemanager and server #'s

			//Add the weapon to the player class.        *****|||||this needs to update the game data, and store to the player class that that player has a weapon equipped. drops from tallies, stores within the player class |||||*****
			SSM.survivor.weaponEquipped = knife;
		}


	}

	public void ClubSelectPressed () {
		LocateAndSetSelectedSurvivor();
		SurvivorStateMachine SSM = selectedSurvivor.GetComponent<SurvivorStateMachine>();
		if (SSM.survivor.weaponEquipped != null) {
			//Unequip the weapon- add it back to gameManager #'s and server

			//subtract the weapon from inventory and add it to the player

			SSM.survivor.weaponEquipped = club;
		} else {
			//remove weapon from gamemanager and server #'s

			//Add the weapon to the player class.       
			SSM.survivor.weaponEquipped = club;
		}

	}

	public void GunSelectPressed () {
		LocateAndSetSelectedSurvivor();
		SurvivorStateMachine SSM = selectedSurvivor.GetComponent<SurvivorStateMachine>();
		if (SSM.survivor.weaponEquipped != null) {
			//Unequip the weapon- add it back to gameManager #'s and server

			//subtract the weapon from inventory and add it to the player

			SSM.survivor.weaponEquipped = gun;
		} else {
			//remove weapon from gamemanager and server #'s

			//Add the weapon to the player class.       
			SSM.survivor.weaponEquipped = gun;
		}
	}

	void LocateAndSetSelectedSurvivor () {
		foreach (GameObject survivor in BSM.survivorList) {
			if (survivor.GetComponent<SurvivorStateMachine>().isSelected) {
				//Debug.Log ("Found the selected survivor- attempting to set gameobject");
				selectedSurvivor = survivor;
			}
		}
	}
}
