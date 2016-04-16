using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VictoryScreenController : MonoBehaviour {

	[SerializeField]
	private Text text;

	// Use this for initialization
	void Start () {
		text.text = "You earned " + GameManager.instance.reportedSupply + " supply";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
