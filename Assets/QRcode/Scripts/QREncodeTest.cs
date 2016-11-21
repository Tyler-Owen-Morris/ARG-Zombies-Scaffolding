/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>
/// 
/// 

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;
using System;

public class QREncodeTest : MonoBehaviour {
	public QRCodeEncodeController e_qrController;
	public RawImage qrCodeImage;
	public InputField m_inputfield;

	// Use this for initialization
	void Start () {
		if (e_qrController != null) {
			e_qrController.e_QREncodeFinished += qrEncodeFinished;
		}
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
			string valueStr = encryptData(m_inputfield.text);
            Debug.Log(valueStr);
			e_qrController.Encode(valueStr);
		}
	}

	public void ClearCode()
	{
		qrCodeImage.texture = null;
		m_inputfield.text = "";
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
