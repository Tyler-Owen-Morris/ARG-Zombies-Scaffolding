using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatWeaponListElementManager : MonoBehaviour {

    public Text weaponText;
    public GameObject myWeaponPlayCard;

    public int weaponID, equippedID, base_dmg, modifier, stam_cost, durability;

    void Start ()
    {
        this.transform.localScale = new Vector3(1, 1, 1); //This is intended to correct for scaling that occurs during spawning/parenting
    }

    public void SetUpMyData ()
    {
        if (myWeaponPlayCard != null) {
            BaseWeapon myWeapon = myWeaponPlayCard.GetComponent<BaseWeapon>();
            weaponID = myWeapon.weapon_id;
            equippedID = myWeapon.equipped_id;
            base_dmg = myWeapon.base_dmg;
            modifier = myWeapon.modifier;
            durability = myWeapon.durability;
            this.name = myWeapon.gameObject.name;
        }

    }

    public void EquipButtonPressed ()
    {
        BattleStateMachine my_BSM = GameObject.FindObjectOfType<BattleStateMachine>();
        if (my_BSM != null)
        {
            my_BSM.EquipNewWeapon(weaponID);
            Debug.Log("Attempting to equip: " + weaponID);
        }
        else
        {
            Debug.Log("Unable to find the battlestatemachine");
        }
    }
}
