using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlazeOGImageOscillator : MonoBehaviour {

    private Image my_image;
    private float minIntensity = .25f;
    private float maxIntensity = .85f;
    float random;
    
    void Start()
    {
        my_image = this.gameObject.GetComponent<Image>();
        random = Random.Range(0.0f, 65535.0f);
    }

	void Update () {
        float noise = Mathf.PerlinNoise(random, Time.time);
        Color temp = my_image.color;
        temp.a = Mathf.Lerp(minIntensity, maxIntensity, noise);
        my_image.color = temp;
	}
}
