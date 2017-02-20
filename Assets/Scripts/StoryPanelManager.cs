using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryPanelManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Vector2 startPosition;
    private string storyImgBaseURL = "http://game.argzombie.com/story_images/";//the photo name is added in code
    

    void Start()
    {
        //store beginning location
        startPosition = this.gameObject.transform.position;
    }

    public void FetchMyStoryImage (string img_name)
    {
        string URL = storyImgBaseURL + img_name;
        StartCoroutine(FetchStoryImg(URL));
        Debug.Log("Fetching story IMG with NAME OF: " + img_name +" And a URL of: "+URL);
    }

    IEnumerator FetchStoryImg (string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {
            Image my_image = this.GetComponent<Image>();
            //Vector2 vec2 = GetComponent<RectTransform>().pivot;
            RectTransform myRT = GetComponent<RectTransform>();
            int wid = www.texture.width;
            int heit = www.texture.height;
            Sprite my_loaded_img = Sprite.Create(www.texture, myRT.rect, new Vector2());
            my_image.sprite = my_loaded_img;
            Debug.Log("loading in image: " + url);
            
            

        }
        else
        {
            Debug.Log(www.error);
        }
    }

    #region this handles the drag and destroy

    public void OnBeginDrag(PointerEventData eventData)
    {
        //store the offset
        //offset = startPosition + eventData.position;
        startPosition = this.gameObject.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {


        Vector2 eventPosition = eventData.position;
        if (eventData.position.x > startPosition.x)
        {
            eventPosition.x = startPosition.x;
        }
        eventPosition.y = startPosition.y;

        this.transform.position = eventPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragDist = 3.0f;
        if (transform.position.x <= (startPosition.x - dragDist))
        {
            Destroy(this.gameObject);
        }
    }
    #endregion end drag and destroy

}
