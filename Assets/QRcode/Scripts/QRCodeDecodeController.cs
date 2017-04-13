/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.IO;

public class QRCodeDecodeController : MonoBehaviour
{
	public delegate void QRScanFinished(string str);  
	public event QRScanFinished e_QRScanFinished;  

	bool decoding = false;
	bool tempDecodeing = false;
	string dataText = null;
	public DeviceCameraController e_DeviceController = null;
	private Color32[] orginalc;
	private byte[] targetbyte;
	private int W, H, WxH;
	int z = 0;
	void Start()
	{
		if (!e_DeviceController) {
			e_DeviceController = GameObject.FindObjectOfType<DeviceCameraController>();
			if(!e_DeviceController)
			{
				Debug.LogError("the Device Controller is not exsit,Please Drag DeviceCamera from project to Hierarchy");
			}
		}
	}
	void Update()
	{
		if (!e_DeviceController.isPlaying  ) {
			return;
		}

		if (e_DeviceController.isPlaying && !decoding)
		{
			orginalc = e_DeviceController.cameraTexture.GetPixels32();
			W = e_DeviceController.cameraTexture.width;
			H = e_DeviceController.cameraTexture.height;
			WxH = W * H;
			targetbyte = new byte[ WxH ];
			z = 0;

			// convert the image color data
			for(int y = H - 1; y >= 0; y--) {
				for(int x = 0; x < W; x++) {
				//	targetbyte[z++] = (byte)( (((int)orginalc[y * W + x].r)+ ((int)orginalc[y * W + x].g) + ((int)orginalc[y * W + x].b))/3);

					targetbyte[z++]  = (byte)(((int)orginalc[y * W + x].r)<<16 | ((int)orginalc[y * W + x].g)<<8 | ((int)orginalc[y * W + x].b));
				}
			}

			Loom.RunAsync(() =>
			              {
				try
				{
					RGBLuminanceSource luminancesource = new RGBLuminanceSource(targetbyte, W, H, true);
					var bitmap = new BinaryBitmap(new HybridBinarizer(luminancesource.rotateCounterClockwise()));
					Result data;
					var reader = new MultiFormatReader();
		
					data = reader.decode(bitmap);
					if (data != null)
					{
						{
							decoding = true;
							dataText = data.Text;
						}
					}
					else 
					{
						for(int y = 0; y != targetbyte.Length; y++) {
							targetbyte[y] = (byte) ( 0xff - (targetbyte[y] &  0xff));
						}

						luminancesource = new RGBLuminanceSource(targetbyte, W, H, true);
						bitmap = new BinaryBitmap(new HybridBinarizer(luminancesource));

						data = reader.decode(bitmap);
						if (data != null)
						{
							{
								decoding = true;
								dataText = data.Text;
							}
						}
					}
				}
				catch (Exception e)
				{
					decoding = false;
				}
			});	
		}
		if(decoding)
		{
			if(tempDecodeing != decoding)
			{
				e_QRScanFinished(dataText);//triger the  sanfinished event;
			}
			tempDecodeing = decoding;
		}
	}

	public void Reset()
	{
		decoding = false;
		tempDecodeing = decoding;
	}

	public void StopWork()
	{
		decoding = true;
		if (e_DeviceController != null) {
			e_DeviceController.StopWork();
		}
	}

}