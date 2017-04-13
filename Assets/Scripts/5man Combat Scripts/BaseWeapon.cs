using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseWeapon : MonoBehaviour {

	public int base_dmg, modifier, stam_cost, durability, max_durability, miss_chance, weapon_id;
	public int equipped_id;

	public enum WeaponType {
		KNIFE,
		CLUB,
		GUN
	}

	public WeaponType weaponType;
}
