using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseWeapon : MonoBehaviour {

	public int base_dmg, modifier, stam_cost, durability, weapon_id;
	public int equipped_id;

	public enum WeaponType {
		KNIFE,
		CLUB,
		GUN
	}

	public WeaponType weaponType;
}
