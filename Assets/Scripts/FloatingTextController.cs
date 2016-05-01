using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingTextController : MonoBehaviour {

	private static FloatingText popUpTextZombie;
	private static FloatingText popUpTextPlayer;
	private static FloatingText popUpCritText;
	private static WeaponGFX gunBlast;
	private static WeaponGFX clubSwing;
	private static WeaponGFX knifeStab;
	private static GameObject gameCanvas;

	public static void Initialize () {

		gameCanvas = GameObject.Find("Level Canvas");
		popUpTextZombie = Resources.Load<FloatingText>("Prefabs/PopupTextZombie");
		popUpTextPlayer = Resources.Load<FloatingText>("Prefabs/PopupTextPlayer");
		popUpCritText = Resources.Load<FloatingText>("Prefabs/PopupCritText");

		gunBlast = Resources.Load<WeaponGFX>("Prefabs/GunBlast");
		clubSwing = Resources.Load<WeaponGFX>("Prefabs/ClubSwing");
		knifeStab = Resources.Load<WeaponGFX>("Prefabs/KnifeStab");

	}

	public static void CreateFloatingText (string text, Transform location, bool isPlayer) {
		Debug.Log ("Create Floating Text called text of: " + text + " and a location of " + location.position.ToString());
		Color red = new Color(1, 0, 0, 1);


		if (text != "CRITICAL!" && text != "MISS!" ){

		//this is for damage text
			if (isPlayer == true) {
				FloatingText instance = Instantiate(popUpTextPlayer);
				instance.transform.SetParent(gameCanvas.transform, false);
				instance.SetText(text, red);
				Debug.Log("Position of floating text ended up at: " + instance.transform.position.ToString());
			} else {
				FloatingText instance = Instantiate(popUpTextZombie);
				instance.transform.SetParent(gameCanvas.transform, false);
				instance.SetText(text, red);
				Debug.Log("Position of floating text ended up at: " + instance.transform.position.ToString());
			}
		} else {
			// this is for "crit" and "miss" calls
			FloatingText instance = Instantiate(popUpCritText);

//			Transform tempTrans = gameCanvas.transform;
//			Vector3 tempPos = gameCanvas.transform.position;
//			tempPos.y = tempPos.y + 35.0f;
//			tempTrans.position = tempPos;
//			instance.transform.SetParent(tempTrans, false);

			instance.transform.SetParent(gameCanvas.transform, false);
			Color blue = new Color(0, 0, 1, 1);
			instance.SetText(text, blue);
			Debug.Log("Position of floating text ended up at: " + instance.transform.position.ToString());
		}
	}

	public static void SendWeaponAnimation () {
		//Vector3 origin = new Vector3(0,0,0);

		if (GameManager.instance.weaponEquipped == "shiv" ) {
			WeaponGFX instance = Instantiate(knifeStab);
			instance.gameObject.SetActive(true);
		} else if (GameManager.instance.weaponEquipped == "club" ) {
			WeaponGFX instance = Instantiate(clubSwing);
			instance.gameObject.SetActive(true);
		} else if (GameManager.instance.weaponEquipped == "gun" ) {
			WeaponGFX instance = Instantiate(gunBlast);
			instance.gameObject.SetActive(true);
		} else {
			Debug.Log ("failed to send weapon FX animation call, no matching weapon found");
		}
	}
}
