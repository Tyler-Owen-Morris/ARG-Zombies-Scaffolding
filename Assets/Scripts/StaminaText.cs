using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StaminaText : MonoBehaviour {
	private Animator animator;
	public Text stamText;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
		//Debug.Log(clipInfo[0].clip.length);
		Destroy(this.gameObject, clipInfo[0].clip.length);
	}

	public void SetStaminaText (string myText) {
		stamText.text = "+"+myText+" stamina";
		Debug.Log("+"+myText+" stamina");
	}

}
