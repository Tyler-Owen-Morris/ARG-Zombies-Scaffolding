using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeToCombatPanelManager : MonoBehaviour {

    private MapLevelManager myMapMgr;
    public GameObject animated_bldg;
    private bool animate = false, anim_started=false;
    public float animSpeed = 700.0f;
    public float scaleSpeed = 50.0f;
    private Color currentColor = Color.black;
    private Color startingColor = Color.white, iconStartingColor=Color.white;
    private MusicManager theMusicManager;

    public Image myImage;


    void Start () {
        myMapMgr = FindObjectOfType<MapLevelManager>();
        myImage = GetComponent<Image>();
        currentColor.a = 0;
        theMusicManager = FindObjectOfType<MusicManager>();
	}
	
	
	void Update () {
        if (animate)
        {
            StartCoroutine(TakeAnimation());
        }
	}

    IEnumerator TakeAnimation()
    {
        if (anim_started)
        {
            yield break;
        }else
        {
            Debug.Log("animation running..." + Time.time);
            anim_started = true;

            //get the active building object- move it above the panel. child to self
            myMapMgr = FindObjectOfType<MapLevelManager>();
            PopulatedBuilding active_bldg = null;
            if (myMapMgr.activeBuilding != null)
            {
                active_bldg = myMapMgr.activeBuilding;
                animated_bldg = active_bldg.gameObject;
                active_bldg.gameObject.transform.SetParent(this.gameObject.transform);
            }
            //fade-up panel to 100% 
            while (FadePanelUp()) { yield return null; }

            //animate to screen center
            Vector3 screen_center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            while (MoveTowardsCenter(screen_center)) { yield return null; }
            
            //scale building up to simulate zoom
            while (ScaleBldgUp()) { yield return null; }

            //fade music down
            while (ScaleMusicDown()) { yield return null; }

            //fade building out
            while (FadeBldgOut()) { yield return null; }

            //fade icon out
            while (FadeIconOut()) { yield return null; }

            //final stepis to call out to the GameManager for the actual load
            GameManager.instance.LoadBuildingCombat();
            Debug.Log("animation complete");
            animate = false;
        }

    }


    private bool MoveTowardsCenter (Vector3 mid)
    {
        Debug.Log("I see mid @:" + mid + " my building object is @:" + animated_bldg.transform);
        return mid != (animated_bldg.gameObject.transform.position = Vector3.MoveTowards(animated_bldg.gameObject.transform.position, mid, animSpeed * Time.deltaTime));
    }

    private bool ScaleBldgUp() {
        Vector3 new_scale = animated_bldg.transform.localScale += new Vector3(scaleSpeed*Time.deltaTime, scaleSpeed*Time.deltaTime, 0);
        animated_bldg.transform.localScale = new_scale;
        return 10 >= (animated_bldg.transform.localScale.x);
    }

    private bool ScaleMusicDown()
    {
        float new_volume = theMusicManager.audioSource.volume -= (Time.deltaTime);
        theMusicManager.audioSource.volume = new_volume;
        return 0 <= new_volume;//return true until 0, then return false
    }

    private bool FadePanelUp ()
    {
        float alphaChange = Time.deltaTime;
        currentColor.a += alphaChange;
        myImage.color = currentColor;
        //Debug.Log(myImage.color.a.ToString());
        return 1 >= myImage.color.a;
    }

    private bool FadeBldgOut ()
    {
        float alphaChange = Time.deltaTime;
        startingColor.a -= alphaChange;
        animated_bldg.GetComponent<Image>().color = startingColor;
        return 0 < animated_bldg.GetComponent<Image>().color.a;
    }

    private bool FadeIconOut()
    {
        PopulatedBuilding myBldgObject = animated_bldg.GetComponent<PopulatedBuilding>();
        iconStartingColor = myBldgObject.populated_bldg_image.GetComponent<Image>().color;
        float alphaChange = Time.deltaTime;
        iconStartingColor.a -= alphaChange;
        if (myBldgObject.populated_bldg_image.activeInHierarchy)
        {
            myBldgObject.populated_bldg_image.GetComponent<Image>().color = iconStartingColor;
            return 0 < iconStartingColor.a;
        }
        if (myBldgObject.building_unknown_image.activeInHierarchy)
        {
            myBldgObject.building_unknown_image.GetComponent<Text>().color = iconStartingColor;
            return 0 < iconStartingColor.a;
        }else
        {
            return true;
        }
    }

    public void BeginCombatTransitionAnimation ()
    {
        animate = true;
    }
}
