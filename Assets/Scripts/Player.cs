using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public int currentHealth, totalHealth, baseAttack;
	public int shivTopAttack, shivBottomAttack, clubTopAttack, clubBottomAttack, gunTopAttack, gunBottomAttack;

	private int attk;
	private float chanceToGetBit;

	[SerializeField]
	private Slider myHealthSlider;

	[SerializeField]
	private CombatManager combatManager;
	
	// Use this for initialization
	void Start () {
		chanceToGetBit = 5.0f;
		totalHealth = 100;// this is a placeholder for 100 total health.
		currentHealth = GetCurrentHealthFromGameManager ();
		baseAttack = 5;
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

	public int Hit () {
		//int attk = attack;
		if (Button.weaponSelected == "shiv") {
			attk = baseAttack + Random.Range(shivBottomAttack, shivTopAttack); 
			//Debug.Log ("Shiv Used");
		} else if (Button.weaponSelected == "club") {
			attk = baseAttack + Random.Range(clubBottomAttack, clubBottomAttack);
			//Debug.Log ("Club Used");
		} else if (Button.weaponSelected == "gun") {
			attk = baseAttack + Random.Range(gunBottomAttack, gunTopAttack);
			//Debug.Log ("Gun Used");
		}
		//Debug.Log ("Player hits for " + attk +" dmg");
		return attk;
	}

	public void GetHit (int dmg) {
		currentHealth -= dmg; //first take the damage.

		RollForPlayerGettingBit();

		//wether bitten or not- report the attack, notify game manager of current health, and set the slider to correct health.
		Debug.Log ("Player got Hit for " + dmg + " damage and now has " + currentHealth + " current health of " + totalHealth + " possible total health");
		GameManager.instance.SetPublicPlayerHealth (currentHealth);//this updates the permanent memory as the GameManager is permanent, and stores to Prefs
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
