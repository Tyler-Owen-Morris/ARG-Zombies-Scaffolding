using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System;

public class SurvivorListElementManager : MonoBehaviour {

	public Text survivorNameText, survivorStatsText, myInjuredText, mySliderText;
	public Image survivorPortraitSprite;
	public GameObject mySurvivorCard, myMissionText;
	public Button teamButton, equipButton;
	public Slider myStamSlider;
	private MapLevelManager mapLevelManager;
	private SurvivorPlayCard survPlayCard;

	private int entry_id;
	private int team_pos;

	void Start () {
		survPlayCard = mySurvivorCard.GetComponent<SurvivorPlayCard>();
		entry_id = survPlayCard.entry_id;
		team_pos = survPlayCard.team_pos;
		mapLevelManager = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
		//StartCoroutine(UpdateMyProfilePic());
        SetProfilePic();
		this.transform.localScale = new Vector3(1,1,1);
		UpdateMyText();
	}

    void SetProfilePic ()
    {
        ProfileImageManager PIM = FindObjectOfType<ProfileImageManager>();
        survivorPortraitSprite.sprite = PIM.GetMyProfilePic(entry_id, survPlayCard.profilePicURL);

        if (survPlayCard.team_pos == 5)
        {
            Image survivorPic = survivorPortraitSprite;
            survivorPic.sprite = GameManager.instance.my_profile_pic;

        }
    }

	IEnumerator UpdateMyProfilePic() {
        if (survPlayCard.team_pos == 5)
        {
            Image survivorPic = survivorPortraitSprite;
            survivorPic.sprite = GameManager.instance.my_profile_pic;

        }
        else if (survPlayCard.profilePicURL != "") {
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
		//myStatsString += survPlayCard.survivor.curStamina + " stam";
		if (survPlayCard.survivor.weaponEquipped != null) {
			myStatsString += " wielding a " + survPlayCard.survivor.weaponEquipped.name;
		}else{
			myStatsString += " and is Unarmed";
		}
		survivorStatsText.text = myStatsString;

		int temp_stam = 0;
		if (survPlayCard.survivor.curStamina >0) {
			temp_stam = survPlayCard.survivor.curStamina;
		}
		string myStamString = temp_stam.ToString()+" / "+survPlayCard.survivor.baseStamina.ToString();
		mySliderText.text = myStamString;
		float val = (float)temp_stam / (float)survPlayCard.survivor.baseStamina;
		myStamSlider.value = val;
		//Debug.Log(val.ToString());
	}

	public void SetInjuryText (int injury_id) {
		JsonData injury_json = JsonMapper.ToObject(GameManager.instance.injuryJsonText);


		for (int i=0; i < injury_json.Count; i++) {
			if ((int)injury_json[i]["entry_id"] == injury_id) {
				DateTime healed_time = Convert.ToDateTime(injury_json[i]["expire_time"].ToString());
				string myText = "Injured for ";
				TimeSpan duration = healed_time - DateTime.Now;
				if (duration > TimeSpan.FromDays(1)) {
					myText += duration.Days.ToString()+" more Day";
					if (duration.Days > 2) {
						myText += "s";
					}
				} else if (duration > TimeSpan.FromHours(1)) {
					myText += duration.Hours.ToString()+" more hour";
					if (duration.Hours > 2) {
						myText += "s";
					}
				} else {
					myText = "recovering soon";
				}
				myInjuredText.text = myText;
				myInjuredText.gameObject.SetActive(true);
				myMissionText.SetActive(false);
			}
		}
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
		myInjuredText.gameObject.SetActive(false);
	}
}
