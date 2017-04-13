using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System.Text;
using System.Security.Cryptography;
using System;

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
		string encryptedJson = encryptData(json);
		qrGeneratedString = json;

		Debug.Log("Outpost QR json: "+json+" encrypted Outpost QR string:"+encryptedJson);

		Encode();
	}

	public void Reset()
	{
		
	}


	public void CloseOutpostPanel () {
		this.gameObject.SetActive(false);
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

}
