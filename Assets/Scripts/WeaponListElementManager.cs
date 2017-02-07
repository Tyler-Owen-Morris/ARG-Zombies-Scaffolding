using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponListElementManager : MonoBehaviour {

	public Text weaponText, stats_text;
	public Button myEquipButton;
    public Slider durabilitySlider;
    public Image durabilitySliderBG;
	public GameObject myWeaponPlayCard;
	private MapLevelManager mapLevelManager;

	public int weaponID, equippedID, base_dmg, modifier, stam_cost, durability, max_durability, miss_chance;

	void Start () {
		BaseWeapon myWeapon = myWeaponPlayCard.GetComponent<BaseWeapon>();
		weaponID = myWeapon.weapon_id;
		equippedID = myWeapon.equipped_id;
		base_dmg = myWeapon.base_dmg;
		modifier = myWeapon.modifier;
		stam_cost = myWeapon.stam_cost;
		durability = myWeapon.durability;
        max_durability = myWeapon.max_durability;
        miss_chance = myWeapon.miss_chance;
		mapLevelManager = GameObject.Find("Map Level Manager").GetComponent<MapLevelManager>();
		this.transform.localScale = new Vector3(1,1,1);
	}

	public void EquipButtonPressed () {
		mapLevelManager.EquipThisWeapon(weaponID);
	}

	public void UnequipButtonPressed () {
		mapLevelManager.UneqipThisWeapon(weaponID);
	}

    public void SetDurabilitySlider()
    {
        float dur = durability;
        float max_dur = max_durability;
        float value = (dur/max_dur);
        durabilitySlider.value = value;
        Debug.Log(value);
        if (value > 0.75f)
        {
            durabilitySliderBG.color = Color.green;
        }
        else if (value > 0.55f)
        {
            durabilitySliderBG.color = Color.yellow;
        }
        else if (value < 0.25f)
        {
            durabilitySliderBG.color = Color.red;
        }
    }

    public void SetupMyData ()
    {
        string stats_string = stam_cost + " stamina per attack\n";
        stats_string += miss_chance + "% chance to miss";
        stats_text.text = stats_string;

        SetDurabilitySlider();
    }
}


