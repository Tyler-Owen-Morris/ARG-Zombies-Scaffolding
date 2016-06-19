using UnityEngine;
using System.Collections;

public class TMP_locationplaceholder : MonoBehaviour {

	public GameObject bldgHolder;

	void Awake () {
		ReportPosition();
	}

	void ReportPosition () {
		
		Vector3 myPos = gameObject.transform.position;
		Debug.Log("The Head in the middle reports his position at: "+myPos.x.ToString()+" -x, "+myPos.y.ToString()+" -y");
	}
}
