using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SuicideButtonShuffler : MonoBehaviour {

	public Button left_button, right_button;
	private Vector3 left_pos, right_pos;


	void Start () {
		left_pos = left_button.gameObject.transform.position;
		right_pos = right_button.gameObject.transform.position;

		//ShuffleButtonPositions();
	}

	public void ShuffleButtonPositions () {
		int shuf = UnityEngine.Random.Range(0, 4);

		//if it's odd, change the positions- otherwise let them stay.
		if (shuf%2==1) {
			left_button.gameObject.transform.position = right_pos;
			right_button.gameObject.transform.position = left_pos;
		}
	}
}
