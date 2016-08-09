using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SurvivorListElementManager : MonoBehaviour {

	public Text survivorNameText;
	public Text survivorStatsText;
	public Image survivorPortraitSprite;
	public GameObject mySurvivorCard;
	public Button teamButton;
	private MapLevelManager mapLevelManager;
	private SurvivorPlayCard survPlayCard;

	private int entry_id;
	private int team_pos;

	void Start () {
		survPlayCard = mySurvivorCard.GetComponent<SurvivorPlayCard>();
		entry_id = survPlayCard.entry_id;
		team_pos = survPlayCard.team_pos;
		mapLevelManager = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
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
}
