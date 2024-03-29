﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class QRPanelController : MonoBehaviour {

	public GameObject cameraUi, qrDisplayUi, resetBtn, scanLineObj;
	public QRCodeEncodeController e_qrController;
	public QRCodeDecodeController d_qrController;
	public RawImage qrCodeImage;
	public Text UItext, response_text, camera_on_text, result_text;
	public string qrGeneratedString, qrScannedString;
	public MapLevelManager mapLvlMgr;
    public bool is_homebaseQRpanel; //set in UI to determine which code runs for the different panels.

	private string qrScannedURL = GameManager.serverURL+"/QR_FriendRequest.php";
	private string outpostRequestURL = GameManager.serverURL+"/QR_OutpostRequest.php";
	private string homebaseRequestURL = GameManager.serverURL+"/QR_HomebaseRequest.php";
	private string playerJoinHomebaseURL = GameManager.serverURL+"/QR_JoinHomebase.php";
	private string homebaseCheckinURL = GameManager.serverURL+"/QR_CheckinAtHome.php";

	// Use this for initialization
	void Start () {
		if (e_qrController != null) {
			e_qrController.e_QREncodeFinished += qrEncodeFinished;
		}
		if (e_qrController != null) {
			d_qrController.e_QRScanFinished += qrScanFinished;
		}
		SetQrText();
		Encode();
		AcceptButtonPressed();
		mapLvlMgr = MapLevelManager.FindObjectOfType<MapLevelManager>();
	}

	void qrEncodeFinished(Texture2D tex)
	{
		if (tex != null) {
			qrCodeImage.texture = tex;
		} else {

		}
	}

	public void Encode()
	{
		
		if (e_qrController != null) {
			e_qrController.e_QREncodeFinished += qrEncodeFinished;
			string valueStr = qrGeneratedString;
			valueStr = encryptData(valueStr);
			e_qrController.Encode(valueStr);
			Debug.Log("Unencrypted player string: "+qrGeneratedString+" || ENCRYPTED QR player string: "+valueStr);
		}
	}

	public void ClearCode()
	{
		qrCodeImage.texture = null;
		qrGeneratedString = "";
	}

	public void SetQrText () {
		if (GameManager.instance.userId != null) {
			string[] encodeArray = new string[5];
			encodeArray[0] = "player";
			encodeArray[1] = GameManager.instance.userId.ToString();
			encodeArray[2] = System.DateTime.Now.ToString();
			if (Input.location.status == LocationServiceStatus.Running) {
				encodeArray[3] = Input.location.lastData.latitude.ToString();
				encodeArray[4] = Input.location.lastData.longitude.ToString();
			} else {
				encodeArray[3] = "0";
				encodeArray[4] = "0";
			}
			string json = JsonMapper.ToJson(encodeArray);
			Debug.Log(json);
			//json = encryptData(json.ToString());//this encrypts the QRto be encoded
			qrGeneratedString = json;
		}
	}

	//this function is used entirely to send fake scan data to the server.
	public void FakeCompleteScan () {
		//this can be manually changed from the inspector
		DetermineTypeOfScannedCode(qrScannedString);
	}

	void qrScanFinished(string dataText)
	{	
		//dataText = decryptData(dataText);//before the json can be read, it must be decrypted back into json.
		qrScannedString = dataText;
		//UItext.text = dataText; //We don't want to show the scan to our players
		camera_on_text.gameObject.SetActive(false);

		if (resetBtn != null) {
			resetBtn.SetActive(true);
		}

		if (response_text != null) {
			response_text.gameObject.SetActive(true);
		}
		
		if(scanLineObj != null)
		{
			scanLineObj.SetActive(false);
		}

        //This needs to first verify the type of scan that it is.
        //DetermineTypeOfScannedCode(dataText);

        CheckForBossQRScan(dataText); //boss check comes before decryption check.
	}

    void CheckForBossQRScan (string scanText)
    {
        
        Debug.Log("String scanned as: " + scanText );
       
        if (scanText == "http://www.argzombies.com/owen1.php")
        {
           GameManager.instance.LoadBossCombat("owen");
        }
        else if (scanText == "http://www.argzombies.com/david1.php")
        {
            GameManager.instance.LoadBossCombat("david");
        }
        else if (scanText == "http://www.argzombies.com/superzombie.php" || scanText == "http://www.argzombies.com/superzombie")
        {
            GameManager.instance.LoadBossCombat("superzombie");
		}
		else if (scanText == "https://youtu.be/Sq8WyCNHw1U")  //if they scanned a barcode for removed from reality the film... super magic secret.
		{
        	if (mapLvlMgr==null) {
        		mapLvlMgr = FindObjectOfType<MapLevelManager>();
        	}
        	mapLvlMgr.ToggleSuperSecretPanel();
        	Reset();
        }else
        {
            DetermineTypeOfScannedCode(scanText);
        }
    }

	void DetermineTypeOfScannedCode (string scannedText) {
		Debug.Log ("Determining type of code scanned");

        if (scannedText.Contains("http://"))
        {
            Debug.Log("this is not a valid game barcode");
            PostQRResultText("ARG Zombies does not recognize this barcoe");
            return;
        }
        mapLvlMgr = FindObjectOfType<MapLevelManager>();

        //internal game codes need to be decrypted
		string decrypted_text = decryptData(scannedText);
		JsonData scannedJson = JsonMapper.ToObject(decrypted_text);
		Debug.Log("Scanned Text: "+scannedText+" || DECRYPTED TEXT: "+decrypted_text);

		if (scannedJson[0].ToString() == "player") {

			//register the event for analytics
			Analytics.CustomEvent("QR_playerScanned", new Dictionary<string, object>
			{
				{"userID", GameManager.instance.userId},
				{"scanned_userID", scannedJson[1].ToString()}
			});

            //Check that the scanned player is not our user
			if(scannedJson[1].ToString() != GameManager.instance.userId) {
				StartCoroutine(mapLvlMgr.PostTempLocationText("pairing with survivor"));
				StartCoroutine(SendQRPairToServer(decrypted_text));
			}else{
				StartCoroutine(mapLvlMgr.PostTempLocationText("Players may not pair with themselves"));
                PostQRResultText("Players may not pair with themselves");
				Debug.Log("Player can not pair with themselves");
			}
		} else if (scannedJson[0].ToString() == "outpost") {
			StartCoroutine(mapLvlMgr.PostTempLocationText("attempting to join outpost"));
			StartCoroutine(SendOutpostRequestToServer(decrypted_text));
		} else if (scannedJson[0].ToString() == "homebase") {

			//check if this homebase belongs to the player.
			string base_owner_id = scannedJson[1].ToString();
			float base_lat = float.Parse(scannedJson[2].ToString());
			float base_lng = float.Parse(scannedJson[3].ToString());

			//log the scan to analyitics
			Analytics.CustomEvent("QR_homebaseScanned", new Dictionary<string, object>				
			{
				{"userID", GameManager.instance.userId},
				{"ownerID", base_owner_id},
				{"lat", base_lat},
				{"lng", base_lng}
			});

			if (base_owner_id.ToString() == GameManager.instance.userId) {
				//start the coroutine to regenerate player stamina... or do nothing...
				Debug.Log("Player has scanned their own homebase");

                if (GameManager.instance.homebaseLat== 0 && GameManager.instance.homebaseLong == 0) {
                    Debug.Log("This is the first time this player has scanned homebase-this game.");
                    mapLvlMgr.SetNewHomebaseLocation();
                }
                else if (CalculateDistanceToTarget(base_lat, base_lng) <= 100.0f)
                {
                    //player is in range of their homebase
                    StartCoroutine(mapLvlMgr.PostTempLocationText("Checking in at Homebase"));
                    PostQRResultText("Checking in at Homebase");
                    StartCoroutine(PlayerCheckinToHomebase(base_lat, base_lng));
                }else
                {
                    StartCoroutine(mapLvlMgr.PostTempLocationText("not in range"));
                    PostQRResultText("You must be within 50m of home to check in");
                    Debug.Log("Player is not in range of their own Homebase");
                }

				
			} else {
                //player has scanned another players homebase
				if (CalculateDistanceToTarget(base_lat, base_lng) <= 50.0f) {
					StartCoroutine(mapLvlMgr.PostTempLocationText("adding players homebase as outpost"));
					StartCoroutine(JoinHomebaseAsOutpost(base_owner_id, base_lat, base_lng));
				} else {
					StartCoroutine(mapLvlMgr.PostTempLocationText("out of range of homebase"));
					Debug.Log("Player is not in range of the homebase they are attempting to join");
                    PostQRResultText("You are not in range of this homebase");
				}
			}

		} else if (scannedJson[0].ToString() == "zombie") {
			GameManager.instance.zombie_to_kill_id = scannedJson[1].ToString();
			GameManager.instance.LoadAltCombat(1, "zomb");
		} else {
			Debug.Log("json format does not meet with any known QR encoding");
		}
	}

    private bool qrResultTextActive = false;
    void PostQRResultText(string my_text) {
        if (qrResultTextActive == false)
        {
            qrResultTextActive = true;
            StartCoroutine(PostTempQRText(my_text));
        }
        
    }

    IEnumerator PostTempQRText (string text)
    {
        response_text.text = text;
        response_text.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        response_text.text = "";
        response_text.gameObject.SetActive(false);
        qrResultTextActive = false;
        
    }

	IEnumerator JoinHomebaseAsOutpost (string baseOwnerID, float my_lat, float my_lng) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		form.AddField("homebase_owner_id", baseOwnerID);
		form.AddField("base_lat", my_lat.ToString());
		form.AddField("base_lng", my_lng.ToString());

		WWW www = new WWW(playerJoinHomebaseURL, form);
		yield return www;

		Debug.Log(www.text);
		if (www.error == null) {
            JsonData outpostJoinJSON = JsonMapper.ToObject(www.text);
            if (outpostJoinJSON[0].ToString() == "Success")
            {
                Debug.Log(outpostJoinJSON[1].ToString());
                PostQRResultText(outpostJoinJSON[1].ToString());
            }else
            {
                Debug.Log(outpostJoinJSON[1].ToString());
                PostQRResultText(outpostJoinJSON[1].ToString());
            }
		} else {
			Debug.Log(www.error);
		}
	}

	IEnumerator SendOutpostRequestToServer (JsonData outpostData) {
		WWWForm form = new WWWForm();
		form.AddField("id", GameManager.instance.userId);
		form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
		form.AddField("client", "mob");

		form.AddField("owner_id", outpostData[1].ToString());
		form.AddField("outpost_id", outpostData[2].ToString());

		WWW www = new WWW(outpostRequestURL, form);
		yield return www;
		Debug.Log(www.text);

		if (www.error == null) {
			JsonData outpostReturnJson = JsonMapper.ToObject(www.text);
			if(outpostReturnJson[0].ToString() == "Success") {

                PostQRResultText(outpostReturnJson[1].ToString());

			} else if (outpostReturnJson[0].ToString() == "Failed") {

				Debug.Log (outpostReturnJson[1].ToString());
				PostQRResultText(outpostReturnJson[1].ToString());

			}
		} else {
			Debug.Log(www.error);
		}
	}

	IEnumerator SendQRPairToServer(string requestingJSONtext) {
			//parse the json, verify time is close enough, and GPS is close enough.
			JsonData requestingJSON = JsonMapper.ToObject(requestingJSONtext);
			//set the ID of the requester
			string requestIDtext = requestingJSON[1].ToString();
			//verify the QR is not expired
			DateTime requestTime = DateTime.Parse(requestingJSON[2].ToString());
			TimeSpan QRValidWindow = TimeSpan.FromMinutes(5);
			DateTime upperLimit = requestTime + QRValidWindow;
			DateTime lowerLimit = requestTime - QRValidWindow;
			if (DateTime.Now < upperLimit && DateTime.Now > lowerLimit) {
				Debug.Log("The QR code contains a valid time");

				float distanceAllowedInMeters = 100.0f;
				float requestLat = float.Parse(requestingJSON[3].ToString());
				float requestLng = float.Parse(requestingJSON[4].ToString());

				if (CalculateDistanceToTarget(requestLat, requestLng) <= distanceAllowedInMeters) {

					Debug.Log("Players are in range of eachother");

					WWWForm form = new WWWForm();
					form.AddField("request_id", requestIDtext);
					form.AddField("id", GameManager.instance.userId);
					form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
					form.AddField("client", "mob");

					WWW www = new WWW(qrScannedURL, form);
					yield return www;
					Debug.Log (www.text);

					if (www.error == null) {
						
						string jsonReturn = www.text;
						JsonData qrJson = JsonMapper.ToObject(jsonReturn);

						if (qrJson[0].ToString() == "Success") {

							PostQRResultText("You have successfully paired with "+qrJson[1]["name"].ToString());
                            //store the new survivor to the Game data, and map it into the rest of the game.
                            GameManager.instance.survivorJsonText = JsonMapper.ToJson(qrJson[2]);
							Debug.Log(GameManager.instance.survivorJsonText);
							GameManager.instance.CreateSurvivorsFromGameManagerJson();

						} else {
							string myString = jsonReturn[1].ToString();
                            PostQRResultText(myString);
							Debug.Log ("server returned qr failure "+ myString);
						}

					} else {
						Debug.Log (www.error);
						PostQRResultText("failed to contact webserver");
					}

				} else {
					Debug.Log("Players are NOT in range of eachother");
					PostQRResultText("Players are not in range of eachother.");
				}


			} else {
				Debug.Log("the QR code is expired");
				PostQRResultText("the QR code is expired");
			}
			// ********************** WARNING ************************ //
			// The above DateTime check does not currently fail the coroutine.  This needs to give a fail message to the user and stop the coroutine.
			// likewise the below GPS check does not currently fail the coroutine, but it should.


	}

	IEnumerator PlayerCheckinToHomebase (float lat, float lng) {
		float baseLat = lat;
		float baseLng = lng;

		if (CalculateDistanceToTarget(baseLat, baseLng) <= 75.0f) {
			WWWForm form = new WWWForm();
			form.AddField("id", GameManager.instance.userId);
			form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
			form.AddField("client", "mob");

			WWW www = new WWW(homebaseCheckinURL, form);
			yield return www;
			Debug.Log(www.text);

			if (www.error == null) {
				JsonData checkinResponse = JsonMapper.ToObject(www.text);

				if(checkinResponse[0].ToString() == "Success") {
					Debug.Log(checkinResponse[1].ToString());
					GameManager.instance.updateWeaponAndSurvivorMapLevelUI = true;
					StartCoroutine(GameManager.instance.FetchSurvivorData());
				} else {
					Debug.Log(checkinResponse[1].ToString());
				}

			} else {
				Debug.Log(www.error);
			}

		} else {
			Debug.Log("Player is not in range of their homebase, no stamina awarded");
		}

	}

	/// <summary>
	/// reset the QRScanner Controller 
	/// </summary>
	public void Reset()
	{
		if (d_qrController != null) {
			d_qrController.Reset();
		}

		if (qrScannedString != null) {
			qrScannedString = "";	
		}
		
		if (resetBtn != null) {
			resetBtn.SetActive(false);
		}
		if (response_text != null) {
			response_text.gameObject.SetActive(false);
		}

		if(scanLineObj != null)
		{
			scanLineObj.SetActive(true);
		}
		//resume scanning.
		AcceptButtonPressed();
//		if(UItext != null) {
//			UItext.text = "Scan Another Players Device";
//		}
	}


	/// <summary>
	/// if you want to go to other scene ,you must call the QRCodeDecodeController.StopWork(),otherwise,the application will crashed on Mobile .
	/// </summary>
	/// <param name="scenename">Scenename.</param>
//	public void GotoNextScene(string scenename)
//	{
//		if (d_qrController != null) {
//			d_qrController.StopWork();
//		}
//		Application.LoadLevel (scenename);
//	}
	public void StopWork () {
		if (d_qrController != null) {
			d_qrController.StopWork();
		}
	}

	public void RequestButtonPressed () {
		//UItext.gameObject.SetActive(false);
		SetQrText();
		Encode();
		cameraUi.SetActive(false);
		camera_on_text.gameObject.SetActive(false);
		qrDisplayUi.SetActive(true);
	}

	public void AcceptButtonPressed () {
		cameraUi.SetActive(true);
		camera_on_text.gameObject.SetActive(true);
		qrDisplayUi.SetActive(false);
	}

	public float CalculateDistanceToTarget (float lat, float lng) {
		if (Input.location.status == LocationServiceStatus.Running) {
			float myLat = Input.location.lastData.latitude;
			float myLon = Input.location.lastData.longitude;
			float targetLat = lat;
			float targetLng = lng;

			float latMid = (myLat + targetLat)/2f;
			double m_per_deg_lat = 111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid);
			double m_per_deg_lon = 111132.954 * Mathf.Cos( latMid );

			double deltaLatitude = (myLat - targetLat);
			double deltaLongitude = (myLon - targetLng);
			double latDistMeters = deltaLatitude * m_per_deg_lat;
			double lonDistMeters = deltaLongitude * m_per_deg_lon;
			double directDistMeters = Mathf.Sqrt(Mathf.Pow((float)latDistMeters, 2f)+Mathf.Pow((float)lonDistMeters, 2f));

			string myText = "You are "+(float)directDistMeters+" meters from your target";
			Debug.Log (myText);

			return (float)directDistMeters;

		} else {
			Debug.Log ("Location services not running");
			return 50.0f;
		}
	}

	public string encryptData(string toEncrypt)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes(GameManager.QR_encryption_key);
		// 256 -AES key 
		byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
		RijndaelManaged rDel = new RijndaelManaged();
		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;
		rDel.Padding = PaddingMode.PKCS7;
		ICryptoTransform cTransform = rDel.CreateEncryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}

public string decryptData(string toDecrypt)
	{
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes(GameManager.QR_encryption_key);
		// AES-256 key 
		byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
		RijndaelManaged rDel = new RijndaelManaged();
		rDel.Key = keyArray;
		rDel.Mode = CipherMode.ECB;
		rDel.Padding = PaddingMode.PKCS7; // better lang support 
		ICryptoTransform cTransform = rDel.CreateDecryptor();
		byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

		return UTF8Encoding.UTF8.GetString(resultArray);
	}

//Note: These hash functions may create + signs, which are not compatible with php varchar columns. These signs should be filtered for something that is not created by the hash, like underscores ( _ ). Simply:  encryptedString = encryptedString.Replace('+','_'); Don't forget to do the inverse *before* decrypting.
}
