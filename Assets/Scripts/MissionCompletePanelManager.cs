using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class MissionCompletePanelManager : MonoBehaviour {

	public Text missionInfoText;
    public MissionListPopulator my_missListPoper;
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
				
                if (returnJson[2] != null)
                {
                    GameManager.instance.missionJsonText = returnJson[2].ToString();
                    my_missListPoper.LoadMissionsFromGameManager();
                }else
                {
                    Debug.Log("No more missions to confirm- according to server return");
                }
                
                //GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
				//StartCoroutine(GameManager.instance.LoadAllGameData());

				//The gamemanager will update the map level UI, destroy this panel
				//yield return new WaitForSeconds(0.1f);
                this.gameObject.SetActive(false);
			} else {
				Debug.Log(returnJson[1].ToString());
                this.gameObject.SetActive(false);
			}
		}else{
			Debug.Log(www.error);
		}
	}
}
