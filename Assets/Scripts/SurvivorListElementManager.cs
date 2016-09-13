using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SurvivorListElementManager : MonoBehaviour {

	public Text survivorNameText;
	public Text survivorStatsText;
	public Image survivorPortraitSprite;
	public GameObject mySurvivorCard, myMissionText;
	public Button teamButton, equipButton;
	private MapLevelManager mapLevelManager;
	private SurvivorPlayCard survPlayCard;

	private int entry_id;
	private int team_pos;

	void Start () {
		survPlayCard = mySurvivorCard.GetComponent<SurvivorPlayCard>();
		entry_id = survPlayCard.entry_id;
		team_pos = survPlayCard.team_pos;
		mapLevelManager = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
		StartCoroutine(UpdateMyProfilePic());
		this.transform.localScale = new Vector3(1,1,1);
		UpdateMyText();
	}

	IEnumerator UpdateMyProfilePic() {
		if (survPlayCard.profilePicURL != "") {
			WWW www = new WWW(survPlayCard.profilePicURL);
			yield return www;

			if (www.error == null) {

				Image survivorPic = survivorPortraitSprite;
				survivorPic.sprite = Sprite.Create(www.texture, new Rect(0, 0, 200, 200), new Vector2());

			}else {
				Debug.Log(www.error);
			}
		}
	}

	void UpdateMyText () {
		survivorNameText.text = survPlayCard.survivor.name;
		string myStatsString = "";
		myStatsString += survPlayCard.survivor.baseAttack +" Attk ";
		myStatsString += survPlayCard.survivor.curStamina + " stam";
		if (survPlayCard.survivor.weaponEquipped != null) {
			myStatsString += " wielding a " + survPlayCard.survivor.weaponEquipped.name;
		}else{
			myStatsString += " and is Unarmed";
		}
		survivorStatsText.text = myStatsString;
	}

	public void equipButtonPressed () {
		mapLevelManager.SelectWeaponToEquip(survPlayCard.entry_id);
	}

	public void TurnOffMyTeamButton () {
		teamButton.gameObject.SetActive(false);
	}

	public void PromoteThisSurvivorPressed () {
		mapLevelManager.PromoteThisSurvivor(entry_id);
	}

	public void SetToOnMission () {
		teamButton.gameObject.SetActive(false);
		equipButton.gameObject.SetActive(false);
		myMissionText.gameObject.SetActive(true);
	}
}
