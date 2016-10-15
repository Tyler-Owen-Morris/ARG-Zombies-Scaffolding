using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class MissionCompletePanelManager : MonoBehaviour {

	public Text missionInfoText;
	public int mission_id;

	private string missionCompleteURL = GameManager.serverURL+"/MissionComplete.php";
	
	public void AcceptMissionResultsPressed () {
		StartCoroutine(SendMissionComplete());
	}

	IEnumerator SendMissionComplete() {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("mission_id", mission_id);

		WWW www = new WWW(missionCompleteURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData returnJson = JsonMapper.ToObject(www.text);
			if(returnJson[0].ToString() == "Success") {
				Debug.Log(returnJson[1].ToString());
				//GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
				StartCoroutine(GameManager.instance.LoadAllGameData());

				//The gamemanager will update the map level UI, destroy this panel
				yield return new WaitForSeconds(0.1f);
				Destroy(this.gameObject);
			} else {
				Debug.Log(returnJson[1].ToString());
				Destroy(this.gameObject);
			}
		}else{
			Debug.Log(www.error);
		}
	}
}
