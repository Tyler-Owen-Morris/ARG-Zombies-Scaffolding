using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class SurvivorWallElementManager : MonoBehaviour {

    public Text surv_name;
    public GameObject onTeamTextObject;
    public Button addToTeamButton;
    public string surv_id;

    public MapLevelManager myMapLvlMgr;

    void Awake()
    {
        onTeamTextObject.SetActive(false);
        myMapLvlMgr = GameObject.FindObjectOfType<MapLevelManager>();
    }

    public void SetUpThisListItem (string surv_nme, string survivor_id)
    {
        if (survivor_id == GameManager.instance.userId) { Destroy(this.gameObject); } //if this is players own tag- then destroy it

        //determine if this survivor is already recruited.
        JsonData rawSurvivorJson = JsonMapper.ToObject(GameManager.instance.survivorJsonText);
        for(int i = 0; i < rawSurvivorJson.Count; i++)
        {
            if (survivor_id==rawSurvivorJson[i]["entry_id"].ToString() && rawSurvivorJson[i]["dead"].ToString()=="0" && rawSurvivorJson[i]["abandonded"].ToString() == "0")
            {
                //if they're in the players json, not dead, and not abandoned, they're already added
                addToTeamButton.interactable = false;
                onTeamTextObject.SetActive(true);
            }
        } 

        surv_name.text = surv_nme;
        surv_id = survivor_id;
    }

    public void SurvivorPressed ()
    {
        if (myMapLvlMgr.survivorWallConfirmationPanel.activeInHierarchy)
        {
            myMapLvlMgr.survivorWallConfirmationPanel.SetActive(false);
        }
        else
        {
            myMapLvlMgr.survivorWallConfirmationPanel.SetActive(true);
        }
    }

    public void AddThisSurvivorPressed ()
    {
        if(GameManager.instance.foodCount >=4 && GameManager.instance.waterCount >= 4)
        {
            myMapLvlMgr.BuyThisSurvivor(surv_id, surv_name.text);
        }else
        {
            Debug.Log("player does not have enough food or water to hire this survivor");
        }
    }
}
