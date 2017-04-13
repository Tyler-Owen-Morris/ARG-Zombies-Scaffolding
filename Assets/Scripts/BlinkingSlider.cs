using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkingSlider : MonoBehaviour {

	public Slider mySlider;
	public Image mySliderBackground;
	public float blink_frequency = 0.0f;

	public enum BlinkState {
		OFF,
		BLINKA,
		BLINKB
	}
	public BlinkState current_state;

	private float blink_timer = 0.0f;

	void Start () {
		current_state = BlinkState.OFF;
		mySlider = this.gameObject.GetComponent<Slider>();
	}
	// Update is called once per frame
	void Update () {
		switch(current_state)
		{
			case (BlinkState.OFF):
				//idle
			break;
			case (BlinkState.BLINKA):
				blink_timer += Time.deltaTime;
				mySliderBackground.color = Color.yellow;
				if (blink_timer >= blink_frequency) {
					blink_timer = 0;
					current_state = BlinkState.BLINKB;
				}
			break;
			case (BlinkState.BLINKB):
				blink_timer += Time.deltaTime;
				mySliderBackground.color = Color.gray;
				if (blink_timer >= blink_frequency) {
					blink_timer = 0;
					current_state = BlinkState.BLINKA;
				}
			break;
		}
	}

	public void StartBlinking (float freq) {
		blink_frequency = freq;
		current_state = BlinkState.BLINKA;
	}

	public void StopBlinking () {
		current_state = BlinkState.OFF;
		mySliderBackground.color = Color.white;
	}
}
