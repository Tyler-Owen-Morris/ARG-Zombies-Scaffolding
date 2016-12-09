using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class VictoryScreenController : MonoBehaviour {

	[SerializeField]
	private Text text;

	public Button returnToMapButton, abandonTheSurvivorButton, killKillKillEmAllButton, recruitTheSurvivorButton;
    public Text survivorReturnText, decisionResultText;
    public GameObject resultsWindow;

	private string destroySurvivorURL = GameManager.serverURL+"/DestroySurvivor.php";
    private string stealFromSurvivorsURL = GameManager.serverURL + "/StealFromSurvivors.php";

	void Awake () {
        //set default state
		returnToMapButton.gameObject.SetActive(true);
		abandonTheSurvivorButton.gameObject.SetActive(false);
		recruitTheSurvivorButton.gameObject.SetActive(false);
        killKillKillEmAllButton.gameObject.SetActive(false);
        survivorReturnText.gameObject.SetActive(false);
	}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    // Use this for initialization
    void OnLevelFinishedLoading (Scene scene, LoadSceneMode mode) {
		text.text = ConstructWinningTextString();
	}

	string ConstructWinningTextString () {
		string outputText = "";
		if (GameManager.instance.activeBldg_name != "zomb") {
			outputText += "You earned " + GameManager.instance.reportedSupply + " supply\n";
			outputText += GameManager.instance.reportedFood + " Food\n";
			outputText += GameManager.instance.reportedWater + " water.\n\n";


			if(GameManager.instance.survivorFound){
                //adjust the UI options
				returnToMapButton.gameObject.SetActive(false);
				abandonTheSurvivorButton.gameObject.SetActive(true);
				recruitTheSurvivorButton.gameObject.SetActive(true);
                killKillKillEmAllButton.gameObject.SetActive(true);

                //update the text
                JsonData foundSurvivorJSON = JsonMapper.ToObject(GameManager.instance.foundSurvivorJsonText);
                string foundSurvivorString = "You found " + foundSurvivorJSON.Count;
                if (foundSurvivorJSON.Count > 1)
                {
                    foundSurvivorString += " survivors alive\n";
                }else
                {
                    foundSurvivorString += " lone survivor\n";
                }
                foundSurvivorString += "they have weapons and gear\nthey are willing to join you";
                survivorReturnText.text = foundSurvivorString;
                survivorReturnText.gameObject.SetActive(true);

                /* //this is from when you could only find 1 survivor at a time
				outputText += "\n You found "+GameManager.instance.foundSurvivorName;
				outputText += "\nthey have "+GameManager.instance.foundSurvivorAttack+" attack and "+GameManager.instance.foundSurvivorMaxStam+" stamina";
				outputText += "\nWhat will you do?";
                */
			} 
		} else {
			if (GameManager.instance.zombie_to_kill_id != "") {
				outputText += "You successfully killed another player's Zombie!\n";
				outputText += "You earned 50 supply\n";
				outputText += "20 food\n";
				outputText += "and 20 water\n";
			} else {
				outputText += "That zombie has already been killed\n";
				outputText += "You get nothing.\n\n";
				outputText += "way to murder people needlessly...";
			}
		}

		/*
		if (GameManager.instance.reportedTotalSurvivor == 0) {
			return outputText;
		} else {
			
			outputText += "you found " + GameManager.instance.reportedTotalSurvivor + " people alive.\n";
			if (GameManager.instance.reportedActiveSurvivor == GameManager.instance.reportedTotalSurvivor) {
				outputText += "they are all able bodied, and join you gladly.";
			} else {
				outputText += "but " + (GameManager.instance.reportedTotalSurvivor - GameManager.instance.reportedActiveSurvivor) + " can't fight." ;
			}

		}
		*/
		return outputText;
	}

    private bool last_survivor = false;
	public void AbandonTheSurvivor () {
        JsonData foundSurvivorJSON = JsonMapper.ToObject(GameManager.instance.foundSurvivorJsonText);
        int sent = 0;
        for (int i=0; i<foundSurvivorJSON.Count; i++)
        {
            int id_abandoned = (int)foundSurvivorJSON[i]["entry_id"];
            StartCoroutine(SendDeadSurvivorToServer(id_abandoned));
            sent++;
            if (sent == foundSurvivorJSON.Count) last_survivor = true;
        }
	}

	IEnumerator SendDeadSurvivorToServer(int idToDestroy) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("survivor_id", idToDestroy);

		WWW www = new WWW(destroySurvivorURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			Debug.Log(www.text);
            if (last_survivor == true)
            {
                SceneManager.LoadScene("02a Map Level");
            }
		}else {
			Debug.Log(www.error);
		}
	}

    IEnumerator StealFromSurvivors (int food, int water, int supply)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

        form.AddField("food", food);
        form.AddField("water", water);
        form.AddField("supply", supply);

        WWW www = new WWW(stealFromSurvivorsURL, form);
        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
        }else
        {
            Debug.Log(www.error);
        }
    }

    public void KillThemAndTakeTheirStuff ()
    {
        addem = false;
        //calculate the rewards
        JsonData foundSurvivorJson = JsonMapper.ToObject(GameManager.instance.foundSurvivorJsonText);
        int found_food = 0;
        int found_water = 0;
        int found_supply = 0;
        int found_surv_count = foundSurvivorJson.Count;
        int escaped = 0;
        for (int i=0; i<found_surv_count;i++)
        {
            //did they escape?
            float run_odds = 30.0f;
            float run_roll = UnityEngine.Random.Range(0.0f, 100.0f);
            if (run_roll < run_odds)
            {
                Debug.Log("survivor escaped");
                escaped++;
                continue;
            }else
            {
                //what did they have?
                found_food += Random.Range(1, 15);
                found_water += Random.Range(1, 15);
                found_supply += Random.Range(5, 50);
            }
        }
        //send the results to the server
        string attack_string = "";
        if (escaped < found_surv_count) //if you actually caught someone
        {
            StartCoroutine(StealFromSurvivors(found_food, found_water, found_supply));
            attack_string += "your attack took them by surprise\n";
            attack_string += "surveying the "+(found_surv_count-escaped).ToString()+" dead reveals\n";
            attack_string += found_food + " food " + found_water + " water & " + found_supply + " for the taking";
            if (escaped > 0)
            {
                attack_string += "\n"+escaped + " got away...";
            }
        }else
        {
            attack_string += "They escaped\nyou killed nobody and found nothing";
        }
        decisionResultText.text = attack_string;
        resultsWindow.SetActive(true);

    }

    public void AddThemToTheTeam()
    {
        decisionResultText.text = "They gladly join the team\nyour ranks grow.";
        addem = true;
        resultsWindow.SetActive(true);
    }

    private bool addem = false;
    public void ContinueButtonPressed ()
    {
        if (addem == true)
        {
            SceneManager.LoadScene("02a Map Level");
        }else
        {
            //loot has been handlded- destroy all the survivors
            AbandonTheSurvivor();
        }
    }

    public void ReturnToMapLevel ()
    {
        SceneManager.LoadScene("02a Map Level");
    }
}
