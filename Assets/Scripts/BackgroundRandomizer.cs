using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackgroundRandomizer : MonoBehaviour {

	public Sprite[] backgroundsArray;
	public SpriteRenderer bg;
	
	void Start () {
		RollAndSetBackground();
		//ScaleToScreenSize();
	}

	void ScaleToScreenSize() {
		SpriteRenderer mySpriteRenderer = this.GetComponent<SpriteRenderer>();
		if (mySpriteRenderer == null) return;

		this.gameObject.transform.localScale = new Vector3(1, 1, 1);

		float width = mySpriteRenderer.sprite.bounds.size.x;
		float height = mySpriteRenderer.sprite.bounds.size.x;

		float worldScreenHeight = Camera.main.orthographicSize *2;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		float xScale = worldScreenWidth / width;
		float yScale = worldScreenHeight / height;
		transform.localScale = new Vector3(xScale, yScale, 0);
	}

	void RollAndSetBackground () {
        int pos = Random.Range(0, backgroundsArray.Length - 1);
        Debug.Log("background array position: "+pos);
        bg.sprite = backgroundsArray[pos];
	}
}
