using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;

public class OutpostPanelController : MonoBehaviour {

	public QRCodeEncodeController e_qrController;
	public RawImage qrCodeImage;
	public string qrGeneratedString;


	void Start () {
		if (e_qrController != null) {
			e_qrController.e_QREncodeFinished += QREncodeFinished;
		}
	}

	public void QREncodeFinished(Texture2D tex)
	{
		if (tex != null) {
			qrCodeImage.texture = tex;
		} else {

		}
	}
	public void Encode()
	{
		if (e_qrController != null) {
			e_qrController.e_QREncodeFinished += QREncodeFinished;
		}

		if (e_qrController != null) {
			string valueStr = qrGeneratedString;
			e_qrController.Encode(valueStr);
		}
	}
	
	public void SetQRtextAndEncode (int post_id) {
		string[] encodeArray = new string[3];
		encodeArray[0] = "outpost";
		encodeArray[1] = GameManager.instance.userId;
		encodeArray[2] = post_id.ToString();
		string json = JsonMapper.ToJson(encodeArray);
		qrGeneratedString = json;

		Encode();
	}

	public void Reset()
	{
		
	}


	public void CloseOutpostPanel () {
		this.gameObject.SetActive(false);
	}
}
