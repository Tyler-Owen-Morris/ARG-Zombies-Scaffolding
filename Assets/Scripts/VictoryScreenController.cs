using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VictoryScreenController : MonoBehaviour {

	[SerializeField]
	private Text text;

	// Use this for initialization
	void Start () {
		text.text = ConstructWinningTextString();
	}

	string ConstructWinningTextString () {
		string outputText = "";

		outputText += "You earned " + GameManager.instance.reportedSupply + " supply\n";
		outputText += GameManager.instance.reportedFood + " Food\n";
		outputText += GameManager.instance.reportedWater + " water.\n";

		if (GameManager.instance.reportedTotalSurvivor == 0) {
			return outputText;
		} else {
			
			outputText += "you found " + GameManager.instance.reportedTotalSurvivor + " people alive.\n";
			if (GameManager.instance.reportedActiveSurvivor == GameManager.instance.reportedTotalSurvivor) {
				outputText += "they are all able bodied, and join you gladly.";
			} else {
				outputText += "but " + (GameManager.instance.reportedTotalSurvivor - GameManager.instance.reportedActiveSurvivor) + " can't fight." ;
			}
			return outputText;
		}

	}
}
