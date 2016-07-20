using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseWeapon : MonoBehaviour {

	public int topDmg;
	public int botDmg;

	public int stamCost;

	public int durability;

	public enum WeaponType {
		KNIFE,
		CLUB,
		GUN
	}

	public WeaponType weaponType;
}
