using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackgroundRandomizer : MonoBehaviour {

	public Sprite[] backgroundsArray;
	public Image bg;
	
	void Awake () {
		RollAndSetBackground();
	}
	
	void RollAndSetBackground () {
		float roll = Random.Range(0.0f, 100.0f);
		
		//There are currently 10 backgrounds, so that's how I've decided to split the rolls.
		if (roll <= 10.0f) {
			bg.sprite = backgroundsArray[0];
			
		} else if (roll <= 20.0f) {
			bg.sprite = backgroundsArray[1];
			
		} else if (roll <= 30.0f) {
			bg.sprite = backgroundsArray[2];
			
		} else if (roll <= 40.0f) {
			bg.sprite = backgroundsArray[3];
			
		} else if (roll <= 50.0f) {
			bg.sprite = backgroundsArray[4];
		} else if (roll <= 60.0f) {
			bg.sprite = backgroundsArray[5];
		} else if (roll <= 70.0f) {
			bg.sprite = backgroundsArray[6];
		} else if (roll <= 80.0f) {
			bg.sprite = backgroundsArray[7];
		} else if (roll <= 90.0f) {
			bg.sprite = backgroundsArray[8];
		} else if (roll <= 100.0f) {
			bg.sprite = backgroundsArray[9];
		} 
	}
}
