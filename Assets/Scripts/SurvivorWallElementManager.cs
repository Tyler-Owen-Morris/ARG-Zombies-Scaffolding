using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class SurvivorWallElementManager : MonoBehaviour {

    public Text surv_name;
    public GameObject onTeamTextObject, deadTextObject;
    public Button addToTeamButton;
    public string surv_id;

    public MapLevelManager myMapLvlMgr;

    void Awake()
    {
        //turn off all of the text objects until we know our status
        onTeamTextObject.SetActive(false);
        deadTextObject.SetActive(false);
        
        myMapLvlMgr = GameObject.FindObjectOfType<MapLevelManager>();
    }

    public void SetUpThisListItem (string surv_nme, string survivor_id)
    {
        if (survivor_id == GameManager.instance.userId) { Destroy(this.gameObject); } //if this is players own tag- then destroy it

        //determine if this survivor is already recruited.
        JsonData rawSurvivorJson = JsonMapper.ToObject(GameManager.instance.survivorJsonText);
        for(int i = 0; i < rawSurvivorJson.Count; i++)
        {
            if (survivor_id==rawSurvivorJson[i]["entry_id"].ToString() )
            {
                if ( rawSurvivorJson[i]["dead"].ToString() == "1")
                {
                    //set me to dead
                    addToTeamButton.interactable = false;
                    onTeamTextObject.SetActive(false);
                    deadTextObject.SetActive(true);
                }else if (rawSurvivorJson[i]["abandonded"].ToString() == "0")
                {
                    //if not dead and not abandoned- set to alive and already recruited.
                    addToTeamButton.interactable = false;
                    onTeamTextObject.SetActive(true);
                    deadTextObject.SetActive(false);
                }
                else
                {
                    //the player lost the survivor in the past- they may recruit them now if they wish
                    addToTeamButton.interactable = true;
                    onTeamTextObject.SetActive(false);
                    deadTextObject.SetActive(false);
                }


                
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
