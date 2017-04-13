using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponGFX : MonoBehaviour {

	public Animator animator;

	void OnEnable () {
		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		//Destroy(gameObject, clipInfo[0].clip.length);
		StartCoroutine(TurnMeBackOff(clipInfo[0].clip.length));
	}
	IEnumerator TurnMeBackOff (float length) {
		yield return new WaitForSeconds(length);
		gameObject.SetActive(false);
	}
}
