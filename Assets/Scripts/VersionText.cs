using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour {

	private Text myTextObject;

	void Start () {
		myTextObject = gameObject.GetComponent<Text> ();

		myTextObject.text = "v" + Application.version;
	}
}
