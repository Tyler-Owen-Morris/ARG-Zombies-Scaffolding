using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class AdManager : MonoBehaviour {

	public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("", new ShowOptions(){resultCallback = HandleAdResult});
        }
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                //restore survivor
                BattleStateMachine BSM = FindObjectOfType<BattleStateMachine>();
                if (BSM != null)
                {
                    BSM.PlayerChoosePurchaseSurvivorSave();
                }else
                {
                    Debug.Log("unable to find battlestatemachine to reward player for successful ad watch");
                }
                break;
            case ShowResult.Skipped:
                //kill the survivor horribly

                break;
            case ShowResult.Failed:
                //kill the survivor horribler...

                break;
        }
    }
}
