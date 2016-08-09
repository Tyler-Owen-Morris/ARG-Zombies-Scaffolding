using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System;

public class QRPanelController : MonoBehaviour {

	public GameObject cameraUi, qrDisplayUi, resetBtn, scanLineObj;
	public QRCodeEncodeController e_qrController;
	public QRCodeDecodeController d_qrController;
	public RawImage qrCodeImage;
	public Text UItext;
	public string qrGeneratedString, qrScannedString;

	private string qrScannedURL = "http://www.argzombie.com/ARGZ_SERVER/QR_FriendRequest.php";

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
			string[] encodeArray = new string[4];
			encodeArray[0] = GameManager.instance.userId.ToString();
			encodeArray[1] = System.DateTime.Now.ToString();
			if (Input.location.status == LocationServiceStatus.Running) {
				encodeArray[2] = Input.location.lastData.latitude.ToString();
				encodeArray[3] = Input.location.lastData.longitude.ToString();
			} else {
				encodeArray[2] = "0";
				encodeArray[3] = "0";
			}
			string json = JsonMapper.ToJson(encodeArray);
			Debug.Log(json);
			qrGeneratedString = json;
		}
	}

	void qrScanFinished(string dataText)
	{
		qrScannedString = dataText;
		UItext.text = dataText;

		if (resetBtn != null) {
			resetBtn.SetActive(true);
		}
		
		if(scanLineObj != null)
		{
			scanLineObj.SetActive(false);
		}

		StartCoroutine(SendQRPairToServer(dataText));
	}

	IEnumerator SendQRPairToServer(string requestingJSONtext) {
			//parse the json, verify time is close enough, and GPS is close enough.
			JsonData requestingJSON = JsonMapper.ToJson(requestingJSONtext);
			//set the ID of the requester
			string requestIDtext = requestingJSON[0].ToString();
			//verify the QR is not expired
			DateTime requestTime = DateTime.Parse(requestingJSON[1].ToString());
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
			float requestLat = (float)requestingJSON[2];
			float requestLng = (float)requestingJSON[3];
			if (CalculateDistanceToTarget(requestLat, requestLng) <= distanceAllowedInMeters) {
				Debug.Log("Players are in range of eachother");
			} else {
				Debug.Log("Players are NOT in range of eachother");
			}

			WWWForm form = new WWWForm();
			form.AddField("request_id", requestIDtext);
			form.AddField("accept_id", GameManager.instance.userId);

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

		if(UItext != null) {
			UItext.text = "Scan Another Players Device";
		}
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
		qrDisplayUi.SetActive(true);
	}

	public void AcceptButtonPressed () {
		UItext.gameObject.SetActive(true);
		cameraUi.SetActive(true);
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
			return 1000.0f;
		}
	}
}
