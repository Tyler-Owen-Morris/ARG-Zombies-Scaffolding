using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using LitJson;

public class ProfileImageManager : MonoBehaviour {

	public List<ProfileImageHolder> profileImages = new List<ProfileImageHolder>();
    public Sprite default_profile_pic;
    public GameObject profilePicHolderPrefab;
    public Texture2D player_profile_pic;

    private string profileImageUploadURL = GameManager.serverURL+"/UploadProfileImage.php";

    void Awake()
    {
        profilePicHolderPrefab = Resources.Load<GameObject>("Prefabs/ProfileImageHolderPrefab");
        
    }

    public void SyncPlayerProfileImageWithServer ()
    {
        //generate CRC and compare to stored CRC

        //if they DONT match
        /*if ()
        {*/

        //var bytes = GameManager.instance.profile_image_texture.EncodeToPNG();
        
        StartCoroutine(UpdateMyProfileImageOnTheServer()); //for now just start the coroutine 1-26-17
        
        //}
    }

    IEnumerator UpdateMyProfileImageOnTheServer()
    {
        //declare the texture we're going to use
        var tex = new Texture2D(200, 200, TextureFormat.RGB24, false);

        //temporary tricky bullshit to see if reading pixels requires being active in the render field
        MapLevelManager myMapMgr = FindObjectOfType<MapLevelManager>();
        if (myMapMgr != null)
        {
            //activate inventory panel, and wait for end of frame
            myMapMgr.InventoryButtonPressed();
            yield return new WaitForEndOfFrame(); //ensure that the panel has loaded
            //locate the player profile picture, and write it's contents to the texture we've declared
            GameObject[] survivorListElements = GameObject.FindGameObjectsWithTag("survivorlistelement");
            foreach (GameObject survivorListObject in survivorListElements)
            {
                SurvivorListElementManager mySLEM = survivorListObject.GetComponent<SurvivorListElementManager>();
                if (mySLEM.mySurvivorCard.GetComponent<SurvivorPlayCard>().team_pos == 5)
                {
                    //this is the player character- get the texture being displayed and read it into the tex object
                    tex.ReadPixels(mySLEM.survivorPortraitSprite.rectTransform.rect, 0, 0);
                    tex.Apply();
                    break;
                }
            }
            //close the panel and carry on reading the binary 
            myMapMgr.InventoryButtonPressed();
        }

        var bytes = tex.EncodeToPNG()/*GameManager.instance.my_profile_pic.texture.EncodeToPNG()*/;
        Debug.Log("******************************<<<<<<<<<<<<<<<Encoding profile image to binary:" + bytes.ToString());


        WWWForm form = new WWWForm();
        form.AddField("id", GameManager.instance.userId);
        form.AddField("login_ts", GameManager.instance.lastLoginTime.ToString());
        form.AddField("client", "mob");

        form.AddField("action", "upload image");    
        form.AddBinaryData("profileImage", bytes, GameManager.instance.userId + ".png");
        Debug.Log("attempting to send player profile pic to the server as a BLOB "+bytes.ToString());

        //upload to the server
        WWW www = new WWW(profileImageUploadURL, form);
        yield return www;
        Debug.Log(www.text);

        if (www.error == null)
        {
            Debug.Log("no web error reported from upload");
            JsonData jsonReturn = JsonMapper.ToObject(www.text);
        }else
        {
            Debug.Log(www.error);
        }
    }

    public Sprite GetMyProfilePic(int surv_id, string img_url)
    {
        //look for the image in the current list.
        foreach(ProfileImageHolder profileImage in profileImages)
        {
            if (profileImage.survivor_id == surv_id)
            {
                return profileImage.profile_image;

            }
        }

        //if not found- retun default and start the coroutine to find and create the record.
        StartCoroutine(FetchProfilePic(surv_id, img_url));
        return default_profile_pic;
    }

    public void LoadProfilePictures ()
    {
        //SyncPlayerProfileImageWithServer();
        JsonData survivorJson = JsonMapper.ToObject(GameManager.instance.survivorJsonText);
        if (survivorJson!=null)
        {
            for (int i=0; i<survivorJson.Count;i++)
            {
                //search for preexisting record
                int my_id = (int)survivorJson[i]["entry_id"];
                bool rec_found = false;
                foreach (ProfileImageHolder profileImage in profileImages)
                {
                    
                    if (profileImage.survivor_id==my_id)
                    {
                        //update the existing entry
                        rec_found = true;
                        if (profileImage.profile_image == default_profile_pic) { AttemptPictureRefresh(profileImage); }
                        break;
                    }
                }
                if (rec_found) { break; }//must break the 2nd loop to avoid creating a redundant entry.

                //no match- means we need to query for the image
                StartCoroutine(FetchProfilePic(my_id, survivorJson[i]["profile_pic_url"].ToString()));
            }
        }else
        {
            Debug.Log("Json failed to encode");
        }
    }

    void AttemptPictureRefresh (ProfileImageHolder profile_image)
    {
        if (profile_image.img_url!="")
        {
            profileImages.Remove(profile_image);
            StartCoroutine(FetchProfilePic(profile_image.survivor_id, profile_image.img_url));
            Destroy(profile_image.gameObject);
        }else
        {
            //Debug.Log("Invalid URL- this guy just gets a default profile pic");
        }
    }

    

    IEnumerator FetchProfilePic (int survivor_id, string url)
    {
        if (url != "")
        {
            WWW www = new WWW(url);
            yield return www;

            if (www.error == null)
            {
                profilePicHolderPrefab = Resources.Load<GameObject>("Prefabs/ProfileImageHolderPrefab");
                GameObject instance = Instantiate(profilePicHolderPrefab, gameObject.transform) as GameObject;
                ProfileImageHolder my_imageHolder = instance.GetComponent<ProfileImageHolder>();
                my_imageHolder.profile_image = Sprite.Create(www.texture, new Rect(0, 0, 200, 200), new Vector2());
                my_imageHolder.survivor_id = survivor_id;
                my_imageHolder.img_url = url;
                profileImages.Add(my_imageHolder);


                //Debug.Log("Created new ProfileImage record");
            }
            else
            {
                profilePicHolderPrefab = Resources.Load<GameObject>("Prefabs/ProfileImageHolderPrefab");
                GameObject instance = Instantiate(profilePicHolderPrefab, gameObject.transform) as GameObject;
                ProfileImageHolder my_imageHolder = instance.GetComponent<ProfileImageHolder>();
                my_imageHolder.profile_image = default_profile_pic;
                my_imageHolder.survivor_id = survivor_id;
                my_imageHolder.img_url = url;
                profileImages.Add(my_imageHolder);
               // Debug.Log("Failed to fetch Image from: " + url+" loading default pic instead");
            }
        }else
        {
            profilePicHolderPrefab = Resources.Load<GameObject>("Prefabs/ProfileImageHolderPrefab");
            GameObject instance = Instantiate(profilePicHolderPrefab, gameObject.transform) as GameObject;
            ProfileImageHolder my_imageHolder = instance.GetComponent<ProfileImageHolder>();
            my_imageHolder.profile_image = default_profile_pic;
            my_imageHolder.survivor_id = survivor_id;
            my_imageHolder.img_url = url;
            profileImages.Add(my_imageHolder);
            //Debug.Log("creating default sprite for " + survivor_id+" suv id" );
        }
    }
}


