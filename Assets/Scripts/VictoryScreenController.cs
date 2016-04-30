using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VictoryScreenController : MonoBehaviour {

	[SerializeField]
	private Text text;

	// Use this for initialization
	void Start () {
		text.text = "You earned " + GameManager.instance.reportedSupply + " supply\n" + GameManager.instance.reportedFood + " Food\nand, " + GameManager.instance.reportedWater + " water.";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
