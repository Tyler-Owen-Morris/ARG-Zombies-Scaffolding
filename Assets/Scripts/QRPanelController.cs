using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System;
using System.Text;
using System.Security.Cryptography;

public class QRPanelController : MonoBehaviour {

	public GameObject cameraUi, qrDisplayUi, resetBtn, scanLineObj;
	public QRCodeEncodeController e_qrController;
	public QRCodeDecodeController d_qrController;
	public RawImage qrCodeImage;
	public Text UItext, camera_on_text, result_text;
	public string qrGeneratedString, qrScannedString;
	public MapLevelManager mapLvlMgr;

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
		RequestButtonPressed();
		mapLvlMgr = MapLevelManager.FindObjectOfType<MapLevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
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
			e_qrController.Encode(valueStr);
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
		
		if(scanLineObj != null)
		{
			scanLineObj.SetActive(false);
		}

		//This needs to first verify the type of scan that it is.
		DetermineTypeOfScannedCode(dataText);
	}

	void DetermineTypeOfScannedCode (string scannedText) {
		JsonData scannedJson = JsonMapper.ToObject(scannedText);
		Debug.Log(scannedText);

		if (scannedJson[0].ToString() == "player") {
			if(scannedJson[1].ToString() != GameManager.instance.userId) {
				StartCoroutine(mapLvlMgr.PostTempLocationText("pairing with survivor"));
				StartCoroutine(SendQRPairToServer(scannedText));
			}else{
				StartCoroutine(mapLvlMgr.PostTempLocationText("Players may not pair with themselves"));
				Debug.Log("Player can not pair with themselves");
			}
		} else if (scannedJson[0].ToString() == "outpost") {
			StartCoroutine(mapLvlMgr.PostTempLocationText("attempting to join outpost"));
			StartCoroutine(SendOutpostRequestToServer(scannedJson));
		} else if (scannedJson[0].ToString() == "homebase") {
			//check if this homebase belongs to the player.
			string base_owner_id = scannedJson[1].ToString();
			float base_lat = float.Parse(scannedJson[2].ToString());
			float base_lng = float.Parse(scannedJson[3].ToString());

			if (base_owner_id.ToString() == GameManager.instance.userId) {
				//start the coroutine to regenerate player stamina... or do nothing...
				Debug.Log("Player has scanned their own homebase");
				StartCoroutine(mapLvlMgr.PostTempLocationText("Checking in at homebase"));
				StartCoroutine(PlayerCheckinToHomebase(base_lat, base_lng));
			} else {
				if (CalculateDistanceToTarget(base_lat, base_lng) <= 50.0f) {
					StartCoroutine(mapLvlMgr.PostTempLocationText("adding players homebase as outpost"));
					StartCoroutine(JoinHomebaseAsOutpost(base_owner_id, base_lat, base_lng));
				} else {
					StartCoroutine(mapLvlMgr.PostTempLocationText("out of range of homebase"));
					Debug.Log("Player is not in range of the homebase they are attempting to join");
				}
			}

		} else {
			Debug.Log("json format does not meet with any known QR encoding");
		}
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
				UItext.text = outpostReturnJson[1].ToString();
			} else if (outpostReturnJson[0].ToString() == "Failed") {
				Debug.Log (outpostReturnJson[1].ToString());
				UItext.text = outpostReturnJson[1].ToString();
			}
		} else {
			Debug.Log(www.error);
		}
	}

	IEnumerator SendQRPairToServer(string requestingJSONtext) {
			//parse the json, verify time is close enough, and GPS is close enough.
			JsonData requestingJSON = JsonMapper.ToJson(requestingJSONtext);
			//set the ID of the requester
			string requestIDtext = requestingJSON[1].ToString();
			//verify the QR is not expired
			DateTime requestTime = DateTime.Parse(requestingJSON[2].ToString());
			TimeSpan QRValidWindow = TimeSpan.FromMinutes(5);
			DateTime upperLimit = requestTime + QRValidWindow;
			DateTime lowerLimit = requestTime - QRValidWindow;
			if (DateTime.Now < upperLimit && DateTime.Now > lowerLimit) {
				Debug.Log("The QR code contains a valid time");
			} else {
				Debug.Log("the QR code is expired, but I'ma let you go ahead anyway since this is a debug build");
			}
			// ********************** WARNING ************************ //
			// The above DateTime check does not currently fail the coroutine.  This needs to give a fail message to the user and stop the coroutine.
			// likewise the below GPS check does not currently fail the coroutine, but it should.

			float distanceAllowedInMeters = 25.0f;
			float requestLat = (float)requestingJSON[3];
			float requestLng = (float)requestingJSON[4];
			if (CalculateDistanceToTarget(requestLat, requestLng) <= distanceAllowedInMeters) {
				Debug.Log("Players are in range of eachother");
			} else {
				Debug.Log("Players are NOT in range of eachother");
			}

			WWWForm form = new WWWForm();
			form.AddField("request_id", requestIDtext);
			form.AddField("id", GameManager.instance.userId);
			form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
			form.AddField("client", "mob");

			WWW www = new WWW(qrScannedURL, form);
			yield return www;

			if (www.error == null) {
				Debug.Log ("qr raw php return from web is: "+www.text);
				string jsonReturn = www.text.ToString();
				JsonData qrJson = JsonMapper.ToObject(jsonReturn);

				if (qrJson[0].ToString() == "Success") {
					UItext.text = "You have successfully paired with "+qrJson[1]["first_name"].ToString()+" "+qrJson[1]["last_name"].ToString();
				} else {
					Debug.Log ("server returned qr failure "+ jsonReturn);
				}

			} else {
				Debug.Log (www.error);
				UItext.text = "failed to contact webserver";
			}
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
		UItext.gameObject.SetActive(true);
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
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
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
		byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");
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
