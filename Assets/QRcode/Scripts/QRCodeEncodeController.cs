/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>
/// 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using ZXing.Common;


public class QRCodeEncodeController : MonoBehaviour {

	private Texture2D m_EncodedTex;
	public int e_QRCodeWidth;
	public int e_QRCodeHeight;
	public delegate void QREncodeFinished(Texture2D tex);  
	public event QREncodeFinished e_QREncodeFinished;  
	BitMatrix byteMatrix;

	void Start ()
	{
		int targetWidth = Mathf.Min(e_QRCodeWidth,e_QRCodeHeight);
		targetWidth = Mathf.Clamp (targetWidth, 128, 1024);
		e_QRCodeWidth = e_QRCodeHeight = targetWidth;

		m_EncodedTex = new Texture2D(e_QRCodeWidth, e_QRCodeHeight);
	}

	void Update ()
	{

	}

	/// <summary>
	/// Encode the specified string .
	/// </summary>
	/// <param name="valueStr"> content string.</param>
	public void Encode(string valueStr)
	{
	//	var writer = new QRCodeWriter();
		var writer = new MultiFormatWriter();
		Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();  
		//set the code type
		hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");  

		byteMatrix = writer.encode( valueStr, BarcodeFormat.QR_CODE, e_QRCodeWidth, e_QRCodeHeight,hints); 
		//writer1.encode("ddd",BarcodeFormat.
		for (int i =0; i!= e_QRCodeWidth; i++) {
			for(int j = 0;j!= e_QRCodeHeight;j++)
			{
				if(byteMatrix[i,j])
				{
					m_EncodedTex.SetPixel(i,j,Color.black);
				}
				else
				{
					m_EncodedTex.SetPixel(i,j,Color.white);
				}
			}
		}

		///rotation the image 
		Color32[] pixels = m_EncodedTex.GetPixels32();
		pixels = RotateMatrixByClockwise(pixels, m_EncodedTex.width);
		m_EncodedTex.SetPixels32(pixels); 

		m_EncodedTex.Apply ();

		e_QREncodeFinished (m_EncodedTex);
	}

	/// <summary>
	/// Rotates the matrix.Clockwise
	/// </summary>
	/// <returns>The matrix.</returns>
	/// <param name="matrix">Matrix.</param>
	/// <param name="n">N.</param>
	static Color32[] RotateMatrixByClockwise(Color32[] matrix, int n) {
		Color32[] ret = new Color32[n * n];
		for (int i = 0; i < n; ++i) {
			for (int j = 0; j < n; ++j) {
				ret[i*n + j] = matrix[(n - i - 1) * n + j];
			}
		}
		return ret;
	}

	/// <summary>
	/// anticlockwise
	/// </summary>
	/// <returns>The matrix.</returns>
	/// <param name="matrix">Matrix.</param>
	/// <param name="n">N.</param>
	static Color32[] RotateMatrixByAnticlockwise(Color32[] matrix, int n) {
		Color32[] ret = new Color32[n * n];
		
		for (int i = 0; i < n; ++i) {
			for (int j = 0; j < n; ++j) {
				ret[i*n + j] = matrix[(n - j - 1) * n + i];
			}
		}
		return ret;
	}


}
