using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using LitJson;

public class ProfileImageManager : MonoBehaviour {

	public List<ProfileImageHolder> profileImages = new List<ProfileImageHolder>();
    public Sprite default_profile_pic;
    public GameObject profilePicHolderPrefab;

    void Awake()
    {
        profilePicHolderPrefab = Resources.Load<GameObject>("Prefabs/ProfileImageHolderPrefab");
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
            Debug.Log("Invalid URL- this guy just gets a default profile pic");
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

                /*
                ProfileImage newProfileRecord = new ProfileImage();
                Image web_image = newProfileRecord.profile_image;
                web_image.sprite = Sprite.Create(www.texture, new Rect(0, 0, 200, 200), new Vector2());
                newProfileRecord.survivor_id = survivor_id;
                newProfileRecord.img_url = url;
                profileImages.Add(newProfileRecord);
                */
                Debug.Log("Created new ProfileImage record");
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
                Debug.Log("Failed to fetch Image from: " + url+" loading default pic instead");
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
            Debug.Log("creating default sprite for " + survivor_id+" suv id" );
        }
    }
}


