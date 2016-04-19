using UnityEngine;
using System.Collections;

public class TestWWWScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(FetchUserData());
	}

	IEnumerator FetchUserData () {
		WWW playerData = new WWW("localhost/ARGZ_DEV_PHP/PlayerData.php");
		yield return playerData;
		string playerDataString = playerData.text;
		Debug.Log ("here is some text, and also: " + playerDataString);
	}
	

}
