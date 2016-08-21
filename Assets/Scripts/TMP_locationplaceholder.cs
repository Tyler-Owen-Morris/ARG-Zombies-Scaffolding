using UnityEngine;
using System.Collections;

public class TMP_locationplaceholder : MonoBehaviour {

	public GameObject bldgHolder;

	void Awake () {
		ReportPosition();
	}

	void SetPosToCenter () {
		float xPos = Screen.width*0.5f;
		float yPos = Screen.height*0.5f;
		Vector3 myPos = new Vector3(xPos, yPos, 0.0f);
		transform.position = myPos;
	}

	void ReportPosition () {
		
		Vector3 myPos = gameObject.transform.position;
		Debug.Log("The Head in the middle reports his position at: "+myPos.x.ToString()+" -x, "+myPos.y.ToString()+" -y");
	}
}
