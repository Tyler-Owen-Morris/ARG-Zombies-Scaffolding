using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public int currentHealth, totalHealth, baseAttack;
	public int shivTopAttack, shivBottomAttack, clubTopAttack, clubBottomAttack, gunTopAttack, gunBottomAttack;
	public float chanceToGetBit, oddsToMissShiv, oddsToMissClub, oddsToMissGun, oddsToCritShiv, oddsToCritClub, oddsToCritGun;

	// I am exposing these all as public variables to help with testing in the editor. later they don't need to be exposed like this,
	// and will probably be dynamic to specific weapons equipped.

	private int attk;


	[SerializeField]
	private Slider myHealthSlider;

	[SerializeField]
	private CombatManager combatManager;
	
	// Use this for initialization
	void Start () {
		chanceToGetBit = 5.0f;
		totalHealth = 100;// this is a placeholder for 100 total health.
		currentHealth = GetCurrentHealthFromGameManager ();
		baseAttack = 7;
		combatManager = FindObjectOfType<CombatManager>();
	}

	void Update () {
		myHealthSlider.value = ConvertCurrentHealthToSliderValue();
	}

	public void ResetHealthToFull () {
		currentHealth = totalHealth;
	}

	private int GetCurrentHealthFromGameManager () {
		return GameManager.instance.playerCurrentHealth;
	}

	public void SetBaseDamageBasedOnWeapon (int dmg) {
		baseAttack = dmg;
	}

	public int Hit () {
		StartCoroutine (ProcessHit());

		//Debug.Log ("Player hits for " + attk +" dmg");
		return attk;
	}
    
    //this is to return a float that becomes the attack damage modifier. Also
    private float DidICriticalHit (float odds) {

        float roll = Random.Range(0.0f, 100.0f);
        
        if (roll <= odds) {

            //player DID critical, now calculate multiplier % to return
            Debug.Log ("***************** PLAYER CRIT *******************");
            float multiplier = System.Convert.ToInt32(1.0 + Random.Range (1.0f, 3.0f));
            FloatingTextController.CreateFloatingText ( "CRIT!", gameObject.transform, false);
            Debug.Log ("Damage multiplier for crit is: " + multiplier.ToString());
            return multiplier;

        } else {
        	//NO you did not critical

            return 1.0f; //this represents 100% normal damage
        }
    }

    // true is a misss, and false is a hit.
    private bool DidIMiss (float odds) {
    	float roll = Random.Range(0.0f, 100.0f);

    	if (odds >= roll) {
    		return true;
    	} else {
    		return false;
    	}

    }


    IEnumerator ProcessHit () {
	//int attk = attack;
		if (Button.weaponSelected == "shiv") {
			attk = baseAttack + Random.Range(shivBottomAttack, shivTopAttack); 
			Debug.Log ("Before crit, player shiv swung: " + attk.ToString());
			SendProcessDurability();

            if (DidIMiss(oddsToMissShiv) == true) {
            	//if DidIMiss returns true, then attk is none, and text warning goes off
            	attk = 0;
            	FloatingTextController.CreateFloatingText("MISS!", gameObject.transform, false);
            } else {
				attk = Mathf.RoundToInt(attk * DidICriticalHit(oddsToCritShiv)) ;
            }

			Debug.Log ("Shiv Used w/ final attack after multiplier: " + attk.ToString());
		} else if (Button.weaponSelected == "club") {
			attk = baseAttack + Random.Range(clubBottomAttack, clubBottomAttack);
			Debug.Log ("Before crit, player club swung: " + attk.ToString());
			SendProcessDurability();

			if (DidIMiss(oddsToMissClub) == true) {
            	//if DidIMiss returns true, then ?
            	attk = 0;
            	FloatingTextController.CreateFloatingText("MISS!", gameObject.transform, false);
            } else {
				attk = Mathf.RoundToInt(attk * DidICriticalHit(oddsToCritClub));
            }
			

			Debug.Log ("Club Used w/ final attack after multiplier: " + attk.ToString());
		} else if (Button.weaponSelected == "gun") {
			attk = baseAttack + Random.Range(gunBottomAttack, gunTopAttack);
			Debug.Log ("Before crit, player gun swung: " + attk.ToString());
			SendProcessDurability();

			if (DidIMiss(oddsToMissGun) == true) {
            	//if DidIMiss returns true, then ?
            	attk = 0;
            	FloatingTextController.CreateFloatingText("MISS!", gameObject.transform, false);
            } else {
            	//process if critical strike, and set public attack damage
				attk = Mathf.RoundToInt(attk * DidICriticalHit(oddsToCritGun));
            }

			Debug.Log ("Gun Used w/ final attack after multiplier" + attk.ToString());
		}
		yield return attk;
    }

	private void SendProcessDurability () {
		GameManager.instance.ProcessDurability();
			//call to local combatManager to update the ui- which reads to GameManager data.
		combatManager.UpdateTheUI();
	}

	public void GetHit (int dmg) {
		currentHealth -= dmg; //first take the damage.

		RollForPlayerGettingBit();
        FloatingTextController.CreateFloatingText( dmg.ToString(), gameObject.transform, true);
		//wether bitten or not- report the attack, notify game manager of current health, and set the slider to correct health.
		Debug.Log ("Player got Hit for " + dmg + " damage and now has " + currentHealth + " current health of " + totalHealth + " possible total health");
		FindObjectOfType<GameManager>().SetPublicPlayerHealth (currentHealth);//this updates the permanent memory as the GameManager is permanent, and stores to Prefs
		myHealthSlider.value = ConvertCurrentHealthToSliderValue();
		if (currentHealth <= 0) {
			Die();
		}
	}

	public void RollForPlayerGettingBit () {
		if (HasPlayerBeenBitten()){ //then find out if the damage resulted in a bite.
			currentHealth = 0;
			Debug.Log ("Player was Bitten, and must now die");
		}
	}

	public bool HasPlayerBeenBitten () {
		float roll = Random.Range(0.0f, 100.0f);
		Debug.Log ("Player has rolled to see if he was bitten, and the roll was " + roll);
		if ( roll <= chanceToGetBit ) { //this is meant to simulate a direct % of odds, out of 100, with float decimal places to further spread the odds.
			combatManager.PlayerHasBeenBitten(); //Notifies combatManager which notifies the animator.
			return true;
		} else {
			return false;
		}
		// this function should roll and return if the player has or has not been bitten, when he takes damage. GetHit()
	}

	void Die () {//the animator notifies itself as it has internal player, this notifies combat and game managers, as well as updating long term memory
		GameManager.instance.totalSurvivors--;
		GameManager.instance.survivorsActive--;
		combatManager.PlayerHasDied();
	}

	private float ConvertCurrentHealthToSliderValue () {
		float health = currentHealth;
		float temp = health / totalHealth;
		//Debug.Log ("HealthToSliderValue returned a value of: " + temp + " current health: " + health + " and maxHealth: " + maxHealth);
		return temp;
	}
}
