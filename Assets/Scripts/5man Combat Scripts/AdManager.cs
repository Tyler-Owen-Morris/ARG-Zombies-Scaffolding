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
        BattleStateMachine BSM = FindObjectOfType<BattleStateMachine>();
        switch (result)
        {

            case ShowResult.Finished:
                //restore survivor
                if (BSM != null)
                {
                    BSM.PlayerChoosePurchaseSurvivorSave();
                }else
                {
                    Debug.Log("unable to find battlestatemachine to reward player for successful ad watch");
                }
                break;

            case ShowResult.Skipped:
                //unbite- but don't restore survivor.
                if (BSM != null)
                {
                    BSM.PlayerPartiallyWatchedAD();
                }else
                {
                    Debug.Log("unable to find battlestatemachine to reward player for partial ad watch");
                }

                break;
            case ShowResult.Failed:
                //kill the survivor horribler...

                break;
        }
    }
}
