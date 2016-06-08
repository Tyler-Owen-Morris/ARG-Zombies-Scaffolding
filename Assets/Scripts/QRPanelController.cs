using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

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
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void qrEncodeFinished(Texture2D tex)
	{
		if (tex != null && tex != null) {
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
			qrGeneratedString = GameManager.instance.userId.ToString();
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

	IEnumerator SendQRPairToServer(string acceptIDtext) {
//		int value;
//		if (int.TryParse(acceptIDtext, out value)) {
			WWWForm form = new WWWForm();
			form.AddField("request_id", acceptIDtext);
			form.AddField("accept_id", GameManager.instance.userId);

			WWW www = new WWW(qrScannedURL, form);
			yield return www;

			if (www.error == null) {
				Debug.Log ("qr raw php return from web is: "+www.text);
				string jsonReturn = www.text.ToString();
				JsonData qrJson = JsonMapper.ToObject(jsonReturn);

				if (qrJson[0].ToString() == "Success") {
					UItext.text = "Players successfully paired";
				} else {
					Debug.Log ("server returned qr failure "+ jsonReturn);
				}

			} else {
				Debug.Log (www.error);
				UItext.text = "failed to contact webserver";
			}

//		} else {
//			UItext.text = "Invalid Player Code";
//		}
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
		UItext.gameObject.SetActive(false);
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
}
