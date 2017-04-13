using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Fitbit;

public class FitbitInterfaceManager : MonoBehaviour {

    public Button fitBitLoginButton;
    public Text fitBitStatusText;

    private FitBitAPI myFitbitApi;

	// Use this for initialization
	void Start () {
        //Grab the API interface object attached to GameManager
        myFitbitApi = GameManager.instance.gameObject.GetComponent<FitBitAPI>();
	}
	

    //Create the functions for the UI to interface with.
    public void LoginToFitbit ()
    {
        myFitbitApi.LoginToFitbit();
    }

    public void GetAllFitbitData()
    {
        myFitbitApi.GetAllData();
    }
}
