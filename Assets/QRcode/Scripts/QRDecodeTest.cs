/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Security.Cryptography;
using System.Text;

public class QRDecodeTest : MonoBehaviour {

	public QRCodeDecodeController e_qrController;

	public Text UiText;
	public GameObject resetBtn;
	public GameObject scanLineObj;

	// Use this for initialization
	void Start () {
		if (e_qrController != null) {
			e_qrController.e_QRScanFinished += qrScanFinished;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void qrScanFinished(string dataText)
	{
        dataText = decryptData(dataText);
		UiText.text = dataText;
        Debug.Log(dataText);
		if (resetBtn != null) {
			resetBtn.SetActive(true);
		}
		
		if(scanLineObj != null)
		{
			scanLineObj.SetActive(false);
		}
	}

	/// <summary>
	/// reset the QRScanner Controller 
	/// </summary>
	public void Reset()
	{
		if (e_qrController != null) {
			e_qrController.Reset();
		}

		if (UiText != null) {
			UiText.text = "";	
		}
		
		if (resetBtn != null) {
			resetBtn.SetActive(false);
		}

		if(scanLineObj != null)
		{
			scanLineObj.SetActive(true);
		}
	}
	/// <summary>
	/// if you want to go to other scene ,you must call the QRCodeDecodeController.StopWork(),otherwise,the application will crashed on Mobile .
	/// </summary>
	/// <param name="scenename">Scenename.</param>
	public void GotoNextScene(string scenename)
	{
		if (e_qrController != null) {
			e_qrController.StopWork();
		}
		Application.LoadLevel (scenename);
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

}
