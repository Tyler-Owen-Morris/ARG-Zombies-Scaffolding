using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurnHandler {

	public string attacker; //name of attacker
	public string type;
	public GameObject AttackersGameObject;
	public GameObject TargetsGameObject;

	//what type of attack- ranged/melee/zombie
}
