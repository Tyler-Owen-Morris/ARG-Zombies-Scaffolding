using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	//This was the code for the Defenders from Glitch Garden, we are using public static enum to determine weapon selected
	//public GameObject defenderPrefab;
	//public static GameObject selectedDefender;

	public static string weaponSelected = "shiv";
	
	private Button[] buttonArray;
    

	[SerializeField]
	private CombatManager combatManager;
    
    [SerializeField]
    private Player player;

	void Awake () {
		buttonArray = GameObject.FindObjectsOfType<Button>();
		TurnButtonsOff();
	}

	// Use this for initialization
	void Start () {
		//default is initialized to shiv- turn that button on after turning them all off on Awake
		if  (gameObject.name == "Shiv") {
			//Debug.Log ("found the shiv and turning it on");
			combatManager.SetWeaponEquipped ("shiv");
			weaponSelected = "shiv";
			GetComponent<SpriteRenderer>().color = Color.white;
			//StartCoroutine(ShowTheWeaponSelected());
		}
	}
	/*
	IEnumerator ShowTheWeaponSelected () {
		yield return new WaitForSeconds(5);
		Debug.Log ("the " + weaponSelected + " is currently selected");
		StartCoroutine(showTheWeaponSelected());
	}
	//this coroutine was used to verify that the currently selected string wsas updating correctly.
	*/
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseDown () {
		//print (name + "pressed");
		TurnButtonsOff();
		GetComponent<SpriteRenderer>().color = Color.white;
		//selectedDefender = defenderPrefab;
		if (gameObject.name == "Shiv") {
            
			weaponSelected = "shiv";
			combatManager.SetWeaponEquipped ("shiv");
            player.SetBaseDamageBasedOnWeapon (7);
            player.chanceToGetBit = 5.0f;
            combatManager.UpdateTheUI();
		} else if (gameObject.name == "Club") {
            
			weaponSelected = "club";
			combatManager.SetWeaponEquipped ("club");
            player.SetBaseDamageBasedOnWeapon (15);
            player.chanceToGetBit = 3.0f;
            combatManager.UpdateTheUI();
		}else if (gameObject.name == "Gun") {
            
			weaponSelected = "gun";
			combatManager.SetWeaponEquipped ("gun");
            player.SetBaseDamageBasedOnWeapon (20);
            player.chanceToGetBit = 1.0f;
            combatManager.UpdateTheUI();
		}
	}
	
	void TurnButtonsOff () {
		foreach (Button thisButton in buttonArray) {
			thisButton.GetComponent<SpriteRenderer>().color = Color.black;
		}
	}
}
