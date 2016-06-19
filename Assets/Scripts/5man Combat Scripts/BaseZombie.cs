using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseZombie {

	public string name;

	public int baseHP;
	public int curHP;

	public int baseAttack;

	public enum Type {
		SKINNY,
		NORMAL,
		FAT
	}

	public Type zombieType;
}
