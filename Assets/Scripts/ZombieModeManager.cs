using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class ZombieModeManager : MonoBehaviour {

    
    public GameObject zombieQRpanel, zombieAdPanel;
    public Text ad_countText, qrPanelText;
    public int adsToRevive;
    public string[] zStatusFailedStrings, zStatusProcessingStrings;

    private int ads_watched = 0;
    private string zombieStatusURL = GameManager.serverURL + "/GetZombieStatus.php";

    void Start ()
    {
        UpdateAdCountText();
    }

    private bool requestInProgress = false;
    public void RequestToRevive ()
    {
        if (requestInProgress)
        {
            int num = Random.Range(0, zStatusProcessingStrings.Length);//minus 1 to convert to index #
            int index = num - 1;
            if (index < 0)
            {
                index = 0;
            }
            StartCoroutine(PostTempQRPanelText(zStatusProcessingStrings[index], 3.0f));
            return;
        }else
        {
            StartCoroutine(ZombieChecker());
        }
    }

    IEnumerator ZombieChecker()
    {
        requestInProgress = true;
        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", "12/31/1999 11:59:59");
        form.AddField("client", "mob");

        WWW www = new WWW(zombieStatusURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            JsonData zombStatJson = JsonMapper.ToObject(www.text);

            if (zombStatJson[0].ToString() == "Success")
            {
                int stat = (int)zombStatJson[1];

                if (stat == 0)
                {
                    //player is alive, and has a character active on server
                    Debug.Log("player is alive, and has an active character on the server, since we're in the zombie scene, and they haven't been allowed to restart, this is broken");

                }
                else if (stat == 1)
                {
                    Debug.Log("Player is still a zombie, checking again in 15 seconds");

                }
                else if (stat == 2)
                {
                    Debug.Log("Someone has successfully killed this player's zombie!");
                    StartCoroutine(PostTempQRPanelText("SWEET RELEASE! STARTING NEW GAME!", 3.0f));
                    yield return new WaitForSeconds(2.9f);
                    GameManager.instance.playerIsZombie = false;
                    // zombieQRpanel.SetActive(false);
                    GameManager.instance.StartNewCharacter();
                    
                }
                else
                {
                    Debug.Log("Zombie Check callback returned invalid status code");
                }

            }
            else if (zombStatJson[0].ToString() == "Failed")
            {
                Debug.Log(zombStatJson[1].ToString());
                Debug.Log("This implies that the player entry was deleted from the server... WTF, how are we on the zombie scene tho?");
            }


        }
        else
        {
            Debug.Log(www.error);
        }

        yield return new WaitForSeconds(7.0f);//after checking and posting- wait to allow another check.
        requestInProgress = false;
    }

    IEnumerator PostTempQRPanelText(string txt, float dur)
    {
        qrPanelText.text = txt;
        qrPanelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(dur);
        qrPanelText.text = "";
    }

    public void ToggleZombieQRPanel ()
    {
        if (zombieQRpanel.activeInHierarchy)
        {
            zombieQRpanel.SetActive(false);
        }else
        {
            zombieQRpanel.SetActive(true);
        }
    }
	
    public void ToggleZombieAdPanel ()
    {
        if (zombieAdPanel.activeInHierarchy)
        {
            zombieAdPanel.SetActive(false);
        }else
        {
            zombieAdPanel.SetActive(true);
        }
    }

    void UpdateAdCountText ()
    {
        string my_string = ads_watched + " / "+adsToRevive;
        ad_countText.text = my_string;
    }

    public void ZombieAdFinished ()
    {
        ads_watched++;
        if (ads_watched >= adsToRevive)
        {
            GameManager.instance.StartNewCharacter();
            //StartCoroutine(TrickyBullshitToStartNewGame());
        }else
        {
            UpdateAdCountText();
        }
    }

    public void AdPartialWatch()
    {
        Debug.Log("Player didn't finish the ad- they get no credit");
    }

    //good news, tricky bullshit works
    IEnumerator TrickyBullshitToStartNewGame()
    {
  
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("01a Login");
        yield return new WaitForSeconds(0.2f);
        LoginManager myLoginManager = GameObject.Find("Login Manager").GetComponent<LoginManager>();
        myLoginManager.StartNewCharacter();
        Destroy(this.gameObject);
        
    }
}
