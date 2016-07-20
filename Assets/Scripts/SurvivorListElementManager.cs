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

	private int survivor_id;
	private int entry_id;
	private int team_pos;

	void Start () {
		SurvivorPlayCard survPlayCard = mySurvivorCard.GetComponent<SurvivorPlayCard>();
		survivor_id = survPlayCard.survivor_id;
		entry_id = survPlayCard.entry_id;
		team_pos = survPlayCard.team_pos;
		mapLevelManager = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
	}

	public void InventoryButtonPressed () {
		
	}

	public void TurnOffMyTeamButton () {
		teamButton.gameObject.SetActive(false);
	}
}
