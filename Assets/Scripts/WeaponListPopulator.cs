using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponListPopulator : MonoBehaviour {

	public GameObject weaponListElementPrefab, weaponUnequipPanelPrefab;
	public MapLevelManager myMapMgr;

	void Start () {
		//myMapMgr = MapLevelManager.FindObjectOfType<MapLevelManager>();
//		weaponListElementPrefab = Resources.Load<WeaponListElementManager>("Prefabs/WeaponListElementPanel");
//		weaponUnequipPanelPrefab = Resources.Load<WeaponListElementManager>("Prefaba/WeaponUnequipPanel");

		//PopulateWeaponsFromGameManager();
	}


	public void PopulateWeaponsFromGameManager() {

		//find any old list elements in the heirarchy, and destroy them
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
			children.ForEach(child => Destroy(child));

		/*
		GameObject[] oldWeaponListItems = GameObject.FindGameObjectsWithTag("weaponlistelement");
		foreach (GameObject oldWpn in oldWeaponListItems) {
			Debug.Log("Destroying: "+oldWpn.name);
			Destroy(oldWpn.gameObject);
		}
		*/

		//if the player selected has a weapon equipped, spawn the unequip option first.
		foreach (GameObject survivor in GameManager.instance.activeSurvivorCardList) {
			int myId = survivor.GetComponent<SurvivorPlayCard>().entry_id;

			if (myId == myMapMgr.active_gearing_survivor_id) {
				//if the weapon entry has been associated to this survivor- populate the weapon data and spawn the prefab
				if (survivor.GetComponent<SurvivorPlayCard>().survivor.weaponEquipped != null) {
					Debug.Log("my unequipped ID is "+myId);			
					BaseWeapon weaponData = survivor.GetComponent<SurvivorPlayCard>().survivor.weaponEquipped.GetComponent<BaseWeapon>();
					//spawn and parent the prefab
					GameObject instance = Instantiate(weaponUnequipPanelPrefab);
					instance.transform.SetParent(this.gameObject.transform);

					//populate the data
					WeaponListElementManager myWLEM = instance.GetComponent<WeaponListElementManager>();
					myWLEM.myWeaponPlayCard = survivor.GetComponent<SurvivorPlayCard>().survivor.weaponEquipped;
					myWLEM.weaponID = weaponData.weapon_id;
					myWLEM.equippedID = weaponData.equipped_id;
					myWLEM.base_dmg = weaponData.base_dmg;
					myWLEM.modifier = weaponData.modifier;
					myWLEM.stam_cost = weaponData.stam_cost;
					myWLEM.durability = weaponData.durability;

				}
				  
			}
		}

		GameManager.instance.weaponCardList.Clear();
		GameManager.instance.weaponCardList.AddRange(GameObject.FindGameObjectsWithTag("weaponcard"));

		//for each weapon that is not assigned. create a game object 
		foreach (GameObject weapon in GameManager.instance.weaponCardList) {
			//only process the ones that are not already assigned.
			BaseWeapon weaponData = weapon.GetComponent<BaseWeapon>();
			if (weaponData.equipped_id == 0) {
				//this can be equipped, go ahead with creating the object.
				GameObject instance = Instantiate(weaponListElementPrefab);
				//this script runs on the vertical layout element that lays out the game objects. we want them to be childed to this object
				instance.transform.SetParent(this.gameObject.transform);

				//update the data in the weaponListElement via the WeaponListElementManager
				WeaponListElementManager WLEM = instance.GetComponent<WeaponListElementManager>();
				WLEM.myWeaponPlayCard = weapon.gameObject;
				WLEM.weaponID = weaponData.weapon_id;
				WLEM.name = weapon.name;
				WLEM.equippedID = weaponData.equipped_id;
				WLEM.base_dmg = weaponData.base_dmg;
				WLEM.modifier = weaponData.modifier;
				WLEM.stam_cost = weaponData.stam_cost;
				WLEM.durability = weaponData.durability;

				string myDisplayString = "";
				myDisplayString += WLEM.name;
				myDisplayString += " attk: "+WLEM.base_dmg.ToString()+" modifier: "+ weaponData.modifier.ToString();
				WLEM.weaponText.text = myDisplayString;
			}
		}
	}

}
