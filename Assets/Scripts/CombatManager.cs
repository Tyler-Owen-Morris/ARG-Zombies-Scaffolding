using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour {

	public int zombiesToWin, playerHealthStatus;
	public bool zombiesAreLeft, autoAttackEngaged;

	private LevelManager levelManager;
	private CharacterAnimation characterAnimation;
	private int zombiesKilled, playerHealthLeavingCombat;

	[SerializeField]
	private Player player;


	[SerializeField]
	private Text zombiesLeft, survivorsAlive, gameOverText, shivCountText, clubCountText, gunCountText, oddsText;

	[SerializeField]
	private GameObject gameOverpanel;

	[SerializeField]
	private Toggle autoAttackToggle;

	void awake () {
		zombiesKilled = 0;

	}

	// Use this for initialization
	void Start () {
		levelManager = LevelManager.FindObjectOfType<LevelManager>();
		characterAnimation = CharacterAnimation.FindObjectOfType<CharacterAnimation>();


	}

	void OnLevelWasLoaded () {
		this.zombiesToWin = GameManager.instance.zombiesToFight;
		autoAttackEngaged = true;
		UpdateTheUI();
		FloatingTextController.Initialize();
	}

	public void SetWeaponEquipped (string weapon) {
		if (weapon == "gun") {
			characterAnimation.GetComponent<Animator>().SetBool("CharactersAttack-Shoot", true);
		} else {
			characterAnimation.GetComponent<Animator>().SetBool("CharactersAttack-Shoot", false);		
		}
		GameManager.instance.weaponEquipped = weapon;
	}

	public void UpdateTheUI () {
			zombiesLeft.text = zombiesToWin.ToString();
			survivorsAlive.text = GameManager.instance.survivorsActive.ToString();
			shivCountText.text = GameManager.instance.shivCount.ToString();
			clubCountText.text = GameManager.instance.clubCount.ToString();
			gunCountText.text = GameManager.instance.gunCount.ToString();
			autoAttackToggle.isOn = autoAttackEngaged;

			string myString = "";
			if (GameManager.instance.weaponEquipped == "shiv") {
				myString += player.oddsToCritShiv.ToString() + "% - to crit \n";
				myString += player.oddsToMissShiv.ToString() + "% - to miss \n";
				myString += player.chanceToGetBit.ToString() + "% - to get bit\n";
				myString += player.baseAttack.ToString() + " - base attack";
				
			} else if (GameManager.instance.weaponEquipped == "club") {
				myString += player.oddsToCritClub.ToString() + "% - to crit \n";
				myString += player.oddsToMissClub.ToString() + "% - to miss \n";
				myString += player.chanceToGetBit.ToString() + "% - to get bit\n";
				myString += player.baseAttack.ToString() + " - base attack";

			} else if (GameManager.instance.weaponEquipped == "gun") {
				myString += player.oddsToCritGun.ToString() + "% - to crit \n";
				myString += player.oddsToMissGun.ToString() + "% - to miss \n";
				myString += player.chanceToGetBit.ToString() + "% - to get bit\n";
				myString += player.baseAttack.ToString() + " - base attack";

			}
			Debug.Log ("Attempting to change stats text to: "+ myString);
			oddsText.text = myString ;

			//only seen if game ends
			int daysSurvived = GameManager.instance.daysSurvived;
		 	gameOverText.text = "You, and your entire party are now a part of the zombie horde. You managed to survive " + daysSurvived + " days before all dying terrible deaths. /n /n Would you like to Start all over from Day 1? or pay a lucky dollar that a lucky event occurs, and you're spared, and then thanks to 3d printer technology, the limb that you lost was replaced cheaply and quickly with little to no technical skill --- look I'll let you live with 75% of your shit for 1$... straight developer bribe... your call mr " + daysSurvived + " days..." ;
	}

	public void SetZombiesEncountered (int zombies) {
		zombiesToWin = zombies;

	}

	//this is for the game over- player takes full restart
	public void PlayerHasChosenEternalDeath () {
		Time.timeScale = 1;
		gameOverpanel.gameObject.SetActive(false);
		Debug.Log ("the player has chosen a full restart CombatManager is now calling to correct GameManager method (hopefully)");
		//call to game manager public function to reset the game, and return to menu screen.
		GameManager.instance.RestartTheGame();
	}

	//this is also game over condition, but they have spent the dollar
	public void PlayerHasPaidTheDevilToLive () {
		Time.timeScale = 1; //must resume timescale afer panel stopped everything
		gameOverpanel.gameObject.SetActive(false);
		Debug.Log ("The Player has chosen to pay a dollar, and now gets a 75% restart");
		GameManager.instance.PaidRestartOfTheGame();
	}

	public void PlayerWillTakeChancesWithDeath () {
		Time.timeScale = 1;
		gameOverpanel.gameObject.SetActive(false);

		float roll = Random.Range(0.0f, 100.0f);
		float odds = 25.0f;

		if ( roll <= odds ) {
			Debug.Log ("Player SUCCESSFULLY beat the odds with a roll of " +roll + "Game restarting with 75% of stuff");
			GameManager.instance.PaidRestartOfTheGame();
		} else {
			Debug.Log ("Player LOST their battle with death and the entire game will now restart");
			GameManager.instance.RestartTheGame();

		}


	}

	public void AutoAttackPressed () {

		//this sets the public bool for the animator to check in it's "checkBeforeContinue" animation trigger- should set attack trigger based on return.
		if (autoAttackToggle.isOn == true) {
			autoAttackEngaged = true;
		} else {
			autoAttackEngaged = false;
		}
	}

	IEnumerator LevelClearCalled () {
			GameManager.instance.BuildingIsCleared(CalculateSupplyEarned());
			GameManager.instance.SetPublicPlayerHealth(FindObjectOfType<Player>().currentHealth);


			Debug.Log ("The player is leaving combat SUCCESSFULLY with a current health of " + FindObjectOfType<Player>().currentHealth);
			//must pass out which building was cleared.
			levelManager.LoadLevel ("03a Win");


		yield return new WaitForSeconds(1);

	}

	public void ZombieIsKilled () {
		//characterAnimation.ZombieIsDead();//play death animation

		zombiesKilled ++;//add 1 to the #killed, this is for calculating 
		zombiesToWin --;//incriment the total # of zombies down
		zombiesLeft.text = zombiesToWin.ToString();//update the UI count

		//this is my *********WIN********** the building Condition
		if (zombiesToWin <= 0) { 
			StartCoroutine(LevelClearCalled());
		} else {
			//Notify the animator to play the replacement animation.
		}
	}
	
	int CalculateSupplyEarned () {
		Debug.Log ("Calculating the sum of supply earned from " + zombiesKilled + " zombies killed.");
		int sum = 0;
		for (int i = 0; i < zombiesKilled; i++) {
			int num = UnityEngine.Random.Range(0, 8);
			sum += num;
			Debug.Log ("adding "+ num +" supply to the list");
		}
		Debug.Log ("calculating total supply earned yields: " + sum);
		return sum;
	}

	public void PlayerHasBeenBitten () {
		characterAnimation.PlayerHasBeenBitten(); // 
	}

	public void PlayerHasDied () {
		GamePreferences.SetTotalSurvivors(GameManager.instance.totalSurvivors);
		GamePreferences.SetActiveSurvivors(GameManager.instance.survivorsActive);
		UpdateTheUI();
		//run this in update, so that it triggers the -- of # of players
	}

	public void EndGameCalled () {
		gameOverpanel.gameObject.SetActive(true);
		Time.timeScale = 0;

	}

	public void PlayerWantsToRunAway () {
		Player player = GameObject.FindObjectOfType<Player>();
		player.RollForPlayerGettingBit();
		GameManager.instance.SetPublicPlayerHealth(player.currentHealth);
		SceneManager.LoadScene("02a Map Level");
	}

	void Update () {
		if (zombiesToWin> 0) {
			zombiesAreLeft = true;
		} else if ( zombiesToWin <= 0) {
			zombiesAreLeft = false;
		}
	}
}
