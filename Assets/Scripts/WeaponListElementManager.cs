using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponListElementManager : MonoBehaviour {

	public Text weaponText;
	public Button myEquipButton;
	public GameObject myWeaponPlayCard;
	private MapLevelManager mapLevelManager;

	public int weaponID, equippedID, base_dmg, modifier, stam_cost, durability;

	void Start () {
		BaseWeapon myWeapon = myWeaponPlayCard.GetComponent<BaseWeapon>();
		weaponID = myWeapon.weapon_id;
		equippedID = myWeapon.equipped_id;
		base_dmg = myWeapon.base_dmg;
		modifier = myWeapon.modifier;
		stam_cost = myWeapon.stam_cost;
		durability = myWeapon.durability;
		mapLevelManager = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
	}

	public void EquipButtonPressed () {
		mapLevelManager.EquipThisWeapon(weaponID);
	}

}


