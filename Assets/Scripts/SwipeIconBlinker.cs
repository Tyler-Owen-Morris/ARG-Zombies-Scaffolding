using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwipeIconBlinker : MonoBehaviour {

    private Image bg_image, finger_image;

    void Start()
    {
        bg_image = this.gameObject.GetComponent<Image>();
        finger_image = this.gameObject.GetComponentInChildren<Image>();
    }

	void Update ()
    {
        float pct_value = Mathf.Abs(Mathf.Sin(Time.time/.5f));
        //int bin_value = Mathf.RoundToInt(255*pct_value); //colors are 255, alpha value is 0.0-1.0

        Color temp = bg_image.color;
        temp.a = pct_value;
        bg_image.color = temp;

        Color tmp = finger_image.color;
        tmp.a = pct_value;
        bg_image.color = tmp;
    }
}
