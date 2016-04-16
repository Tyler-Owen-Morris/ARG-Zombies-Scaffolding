using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	//This was the code for the Defenders from Glitch Garden, we are using public static enum to determine weapon selected
	//public GameObject defenderPrefab;
	//public static GameObject selectedDefender;

	public static string weaponSelected = "shiv";
	
	private Button[] buttonArray;

	void Awake () {
		buttonArray = GameObject.FindObjectsOfType<Button>();
		TurnButtonsOff();
	}

	// Use this for initialization
	void Start () {
		//default is initialized to shiv- turn that button on after turning them all off on Awake
		if  (gameObject.name == "Shiv") {
			//Debug.Log ("found the shiv and turning it on");
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
		} else if (gameObject.name == "Club") {
			weaponSelected = "club";
		}else if (gameObject.name == "Gun") {
			weaponSelected = "gun";
		}
	}
	
	void TurnButtonsOff () {
		foreach (Button thisButton in buttonArray) {
			thisButton.GetComponent<SpriteRenderer>().color = Color.black;
		}
	}
}
