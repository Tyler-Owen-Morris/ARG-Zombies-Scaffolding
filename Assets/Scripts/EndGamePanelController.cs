using UnityEngine;
using System.Collections;

public class EndGamePanelController : MonoBehaviour {

	public GameObject[] suicideVerificationPanelArray;

	void Start () {
		ShuffleTheArray(suicideVerificationPanelArray);
	}

	void ShuffleTheArray (GameObject[] array) {
		for (int i = array.Length-1; i > 0; i--) {
			int pos = Random.Range(0, i);
			GameObject tmp = array[i];
			array[i] = array[pos];
			array[pos] = tmp;
		}
	}

	public void ContinueWithSuicide () {
		for (int i=0; i < suicideVerificationPanelArray.Length; i++) {
			if (suicideVerificationPanelArray[i].activeInHierarchy == false) {
				suicideVerificationPanelArray[i].SetActive(true);
				break;
			}
			//if we're out of list items and they're all on, call out to send the suicide
			if (i+1 >= suicideVerificationPanelArray.Length) {
				Debug.Log("Out of panels to verify- Sending suicide to GameManager");
				StartCoroutine(GameManager.instance.KillUrself());
			}
		}
	}

	public void CancelSuicide () {
		for (int i=0; i < suicideVerificationPanelArray.Length; i++) {
			suicideVerificationPanelArray[i].SetActive(false);
		}
	}
}
