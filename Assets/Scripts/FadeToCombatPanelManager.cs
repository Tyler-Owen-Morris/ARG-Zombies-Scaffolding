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
    private Color startingColor = Color.white;

    public Image myImage;


    void Start () {
        myMapMgr = FindObjectOfType<MapLevelManager>();
        myImage = GetComponent<Image>();
        currentColor.a = 0;
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

            //fade building out
            while (FadeBldgOut()) { yield return null; }

            //final stepis to call out to the GameManager for the actual load
            GameManager.instance.LoadBuildingCombat();
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

    private bool FadePanelUp ()
    {
        float alphaChange = Time.deltaTime;
        currentColor.a += alphaChange;
        myImage.color = currentColor;
        Debug.Log(myImage.color.a.ToString());
        return 1 >= myImage.color.a;
    }

    private bool FadeBldgOut ()
    {
        float alphaChange = Time.deltaTime;
        startingColor.a -= alphaChange;
        animated_bldg.GetComponent<Image>().color = startingColor;
        return 0 < animated_bldg.GetComponent<Image>().color.a;
    }

    public void BeginCombatTransitionAnimation ()
    {
        animate = true;
    }
}
