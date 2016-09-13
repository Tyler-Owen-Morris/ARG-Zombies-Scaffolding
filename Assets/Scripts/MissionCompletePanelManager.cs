using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class MissionCompletePanelManager : MonoBehaviour {

	public Text missionInfoText;
	public int mission_id;

	private string missionCompleteURL = "http://www.argzombie.com/ARGZ_SERVER/MissionComplete.php";
	
	public void AcceptMissionResultsPressed () {
		StartCoroutine(SendMissionComplete());
	}

	IEnumerator SendMissionComplete() {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("mission_id", mission_id);

		WWW www = new WWW(missionCompleteURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData returnJson = JsonMapper.ToObject(www.text);
			if(returnJson[0].ToString() == "Success") {
				Debug.Log(returnJson[1].ToString());
				GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
				GameManager.instance.ResumeCharacter();

				//give the gameManager a chance to start on the updates, and then destroy the entire panel.
				yield return new WaitForSeconds(0.2f);
				Destroy(this.gameObject);
			} else {
				Debug.Log(returnJson[1].ToString());
			}
		}else{
			Debug.Log(www.error);
		}
	}
}
