using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Intro_ImageLoader : MonoBehaviour {

    public string img_url;
    public bool done = false;

    private Image my_image;

	void Start ()
    {
        my_image = this.GetComponent<Image>();
        StartCoroutine(GetMyImage());
    }

    IEnumerator GetMyImage()
    {
        if (img_url!="" && img_url != null)
        {
            WWW www = new WWW(img_url);
            yield return www;
            Debug.Log(www);

            if (www.error == null)
            {
                RectTransform my_RecTrans = GetComponent<RectTransform>();
                Rect my_rect = my_RecTrans.rect;
                Vector2 vec2= my_RecTrans.pivot;
                int wid = www.texture.width;
                int heit = www.texture.height;
                Sprite my_loaded_img = Sprite.Create(www.texture, new Rect(0,0,wid,heit), vec2);
                my_image.sprite = my_loaded_img;
                Debug.Log("loading in image: "+img_url);
                done = true;

            }else
            {
                Debug.Log(www.error);
            }
        }
    }

}
