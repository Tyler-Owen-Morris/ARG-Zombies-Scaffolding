using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class WeaponSelectManager : MonoBehaviour {

	public string weaponSelected;
	public GameObject weaponConfirmationPanel, swipeIndicator, levelLoadingPanel;
	public Text weaponPanelText;

    private GameObject[] openingImagePanels;
	private string sendWeaponChoiceURL = GameManager.serverURL+"/TutorialWeaponChoice.php";
	private string knifeText = "The knife takes more stamina to use, but comes with consistient kill\nthe survivor who picks a knife is willing to take great risk for great reward\nWill you choose the knife?";
	private string clubText =  "The club can be easier to miss than a knife, but delivers a punch\neven the most hardened survivors can become exhaused swinging its weight all day\nWill you choose the club?";
	private string gunText = "The gun keeps the safest distance between you and you're target\nbut it's the easiest to miss a kill shot, and it requires bullets to use\nWill you choose the gun?";

    private int panel_count = 8;

    void Awake() {
        openingImagePanels = GameObject.FindGameObjectsWithTag("openingimagepanel");
        if (openingImagePanels.Length < 1) {
            Debug.LogWarning("did not successfully load the images panels to watch for disabling loading");
        }
    }

	void Start () {
        levelLoadingPanel.SetActive(true);
	}

    void Update() {
        if (CheckIfLoadIsComplete())
        {
            levelLoadingPanel.SetActive(false);
           // Debug.Log("Level Load complete nownownownownownoww");
        }
    }

    bool CheckIfLoadIsComplete ()
    {
        openingImagePanels = GameObject.FindGameObjectsWithTag("openingimagepanel");
        int completed = openingImagePanels.Length;
        int loaded = 0;
        foreach (GameObject panel in openingImagePanels )
        {
            Intro_ImageLoader myImgLoader = panel.GetComponent<Intro_ImageLoader>();
            if (myImgLoader.done)
            {
                loaded++;
            }
        }

        if (loaded == completed)
        {
            return true;
        }else
        {
            return false;
        }
    }

    public void PanelDestroyed ()
    {
        panel_count--;
        if (panel_count < 1)
        {
            swipeIndicator.SetActive(false);
        }
    }

	public void OpenWeaponConfirmation (string weaponChoice) {
		weaponSelected = weaponChoice;
		weaponConfirmationPanel.SetActive(true);
		if (weaponChoice == "knife") {
			weaponPanelText.text = knifeText;
		} else if (weaponChoice == "club") {
			weaponPanelText.text = clubText;
		} else if (weaponChoice == "gun") {
			weaponPanelText.text = gunText;
		}
	}

	public void WeaponConfirmed () {
		StartCoroutine(SendWeaponSelection(weaponSelected));
	}

	public void BackButton () {
		weaponSelected = null;
		weaponConfirmationPanel.SetActive(false);
	}

	IEnumerator SendWeaponSelection (string wepSelected) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");
		form.AddField("weapon_selected", wepSelected);

		WWW www = new WWW(sendWeaponChoiceURL, form);
		yield return www;

		if (www.error == null) {
			Debug.Log(www.text);
			JsonData weaponSelectJSON = JsonMapper.ToObject(www.text);

			if (weaponSelectJSON[0] != null) {
				if (weaponSelectJSON[0].ToString() == "Success") {
					//This will download the updated waeapons, and survivors to the game data, the bool lets it know to load into the tutorial combat
					GameManager.instance.weaponHasBeenSelected = true;
					StartCoroutine(GameManager.instance.LoadAllGameData());

				} else {
					Debug.Log(weaponSelectJSON[1].ToString());
				}
			}

		} else {
			Debug.Log(www.error);
		}
	}
}
