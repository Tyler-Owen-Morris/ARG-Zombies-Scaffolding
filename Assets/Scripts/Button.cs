using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public GameObject defenderPrefab;
	public static GameObject selectedDefender;
	
	private Button[] buttonArray;
	
	// Use this for initialization
	void Start () {
		buttonArray = GameObject.FindObjectsOfType<Button>();
		TurnButtonsOff();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseDown () {
		//print (name + "pressed");
		TurnButtonsOff();
		GetComponent<SpriteRenderer>().color = Color.white;
		selectedDefender = defenderPrefab;
		
	}
	
	void TurnButtonsOff () {
		foreach (Button thisButton in buttonArray) {
			thisButton.GetComponent<SpriteRenderer>().color = Color.black;
		}
	}
}
