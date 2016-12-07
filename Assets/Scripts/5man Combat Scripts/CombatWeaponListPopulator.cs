using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatWeaponListPopulator : MonoBehaviour {

    public GameObject unequippedWeaponListElementPrefab;

    void Awake ()
    {
        unequippedWeaponListElementPrefab = Resources.Load<GameObject>("Prefabs/UnequippedWeaponListElement");
    }

    public void PopulateUnequippedWeapons ()
    {
        unequippedWeaponListElementPrefab = Resources.Load<GameObject>("Prefabs/UnequippedWeaponListElement");
        if (unequippedWeaponListElementPrefab == null)
        {
            Debug.Log("unable to locate prefab for spawninanting");
        }

        //find any old list elements in the heirarchy, and destroy them
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));

        //clear and repopluate the list of weaponcard gameobjects
        GameManager.instance.weaponCardList.Clear();
        GameManager.instance.weaponCardList.AddRange(GameObject.FindGameObjectsWithTag("weaponcard"));

        int wep_count = 0;
        //for each weapon that is not assigned. create a game object 
        foreach (GameObject weapon in GameManager.instance.weaponCardList)
        {
            //only process the ones that are not already assigned.
            BaseWeapon weaponData = weapon.GetComponent<BaseWeapon>();
            if (weaponData.equipped_id == 0)
            {
                Debug.Log("found an unequipped weapon");
                wep_count++;
                //this can be equipped, go ahead with creating the object.
                GameObject instance = Instantiate(unequippedWeaponListElementPrefab);
                //this script runs on the vertical layout element that lays out the game objects. we want them to be childed to this object
                instance.transform.SetParent(this.gameObject.transform);

                //update the data in the weaponListElement via the WeaponListElementManager
                CombatWeaponListElementManager CWLEM = instance.GetComponent<CombatWeaponListElementManager>();
                CWLEM.myWeaponPlayCard = weapon.gameObject;
                CWLEM.SetUpMyData();

                string myDisplayString = "";
                myDisplayString += CWLEM.name;
                myDisplayString += " attk: " + CWLEM.base_dmg.ToString() + " modifier: " + weaponData.modifier.ToString();
                CWLEM.weaponText.text = myDisplayString;
            }else
            {
                Debug.Log("Found active weapon: " + weapon.name + " equipped to playerID: " + weaponData.equipped_id);
            }
        }

        if (wep_count == 0)
        {
            //no weapons are able to be equipped- notify BSM to carry on
            BattleStateMachine BSM = BattleStateMachine.FindObjectOfType<BattleStateMachine>();
            if (BSM != null)
            {
                BSM.DontEquipNewWeapon();
            }else
            {
                Debug.Log("Unable to locate BSM to notify of NO weapons to equip");
            }
        }
    }
}
