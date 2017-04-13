using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;
using System;
using LitJson;

public class ZombieQRController : MonoBehaviour {

	public QRCodeEncodeController e_qrController;
	public RawImage qrCodeImage;
	public string qrGeneratedString;

	// Use this for initialization
	void Start () {
		if (e_qrController != null) {
			e_qrController.e_QREncodeFinished += qrEncodeFinished;
		}
		string[] encodeArray = new string[2];
		encodeArray[0] = "zombie";
		encodeArray[1] = GameManager.instance.userId.ToString();
		JsonData zombieJSON = JsonMapper.ToJson(encodeArray);
		qrGeneratedString = zombieJSON.ToString();
		Encode();
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
			//string valueStr = m_inputfield.text;
			string encrypted_qr_string = encryptData(qrGeneratedString);//encrypt
			e_qrController.Encode(encrypted_qr_string);
			Debug.Log("Encrypted Zombie QR string: "+encrypted_qr_string+"  pre-encrypted string: "+qrGeneratedString);
		}
	}

	public void ClearCode()
	{
		qrCodeImage.texture = null;
		//m_inputfield.text = "";
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

	/*public string decryptData(string toDecrypt)
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
	}*/
}
