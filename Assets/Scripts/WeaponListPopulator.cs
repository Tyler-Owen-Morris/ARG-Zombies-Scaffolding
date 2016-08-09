using UnityEngine;
using System.Collections;

public class WeaponListPopulator : MonoBehaviour {

	private static GameObject weaponListElementPrefab;

	void Start () {
		weaponListElementPrefab = Resources.Load<GameObject>("Prefabs/WeaponListElementPanel");

		PopulateWeaponsFromGameManager();
	}


	public void PopulateWeaponsFromGameManager() {
		//find any old list elements in the heirarchy, and destroy them
		GameObject[] oldWeaponListItems = GameObject.FindGameObjectsWithTag("weaponlistelement");
		foreach (GameObject oldWpn in oldWeaponListItems) {
			Destroy(oldWpn.gameObject);
		}

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
