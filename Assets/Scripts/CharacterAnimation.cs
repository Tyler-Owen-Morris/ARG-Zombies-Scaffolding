using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

	private Animator animator;
	private Zombie zombie;
	private Player player;
	private CombatManager combatManager;
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		zombie = Zombie.FindObjectOfType<Zombie>();
		player = Player.FindObjectOfType<Player>();
		combatManager = CombatManager.FindObjectOfType<CombatManager>();
		animator.SetBool("CurrentZombieDead", false);
		animator.SetBool("CurrentPlayerDead", false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RefreshZombie () {
		zombie.ResetZombie();
		animator.SetBool("CurrentZombieDead", false);
	}

	public void RefreshPlayer () {
		player.ResetHealthToFull();
		animator.SetBool ("CurrentPlayerDead", false);
		animator.SetBool ("PlayerHasBeenBitten", false);
	}
	
	public void Attack () {
		//need to write a function here to stop the animator from proceeding at all. animation parameter "HasWeapon" bool?
		if (CheckHasCurrentWeapon()) {
			//this sets whether to run melee or not.
			if (GameManager.instance.weaponEquipped == "gun") {
				animator.SetBool("CharactersAttack-Shoot", true);
			} else {
				animator.SetBool("CharactersAttack-Shoot", false);
			}


			//legacy name, but it sends the character down either animation path depending on above bool set.
			animator.SetTrigger("CharactersAttack-melee");
		} else {
			Debug.LogWarning ("Player does not have a valid weapon equipped");

			//this should possibly call to a warning animation trigger? notify the player in some way "no weapon equipped"
		}
	}



	private bool CheckHasCurrentWeapon () {
		if (GameManager.instance.weaponEquipped == "shiv") {

			if (GameManager.instance.shivCount <= 0) {
				return false;
			} else {
				return true;
			}


		} else if (GameManager.instance.weaponEquipped == "club") {

			if (GameManager.instance.clubCount <= 0) {
				return false;
			} else {
				return true;
			}


		} else if (GameManager.instance.weaponEquipped == "gun") {

			if (GameManager.instance.gunCount <= 0) {
				return false;
			}else {
				return true;
			}

		} else {
			Debug.Log ("Animator did not find a weapon, not setting current weapon status");
			return false;
		}


	}

	public void ZombieTakesDmg () {
		zombie.GetHit(player.Hit());
		if (zombie.health <= 0) {
			animator.SetBool("CurrentZombieDead", true);
		} else {
			animator.SetBool("CurrentZombieDead", false);
		}

	}

	public void PlayerTakesDmg () {
		player.GetHit(zombie.Hit());
		if (player.currentHealth <= 0) {
			animator.SetBool("CurrentPlayerDead", true); //this switches the animation bool and should send player to Death/Rebirth animations- which contain the send for the reset on the local instance of player
		} else {
			animator.SetBool("CurrentPlayerDead", false);
		}
	}

	public void PlayerHasBeenBitten () {
		//this has to set the trigger to go from either attack state to Bite>Playerdead>refreshplayer.
		animator.SetBool ("PlayerHasBeenBitten", true);
	}

	public void CheckForAutoAttack () {
		//first check if it's on
		if (combatManager.autoAttackEngaged){

				//then check that weapon equipped can be used.
				if ( CheckHasCurrentWeapon() ) {
					//this should call near enough to continue the attack cycle based on current weapon.
					animator.SetTrigger("CharactersAttack-melee");
				}

			} else {
				//I don't know why I put an else clause here... I feel like I should be setting something else to false... ?????
			}

	}

	// this will be called from end of player animation
	public void CheckForGameOver () { //leaveing name for now due to many declaraions as string
		if (GameManager.instance.totalSurvivors <= 0) {
			combatManager.EndGameCalled();

			//this should start the coroutine to end the game.
		} else {
			// if the game is continuing

		}

		//include here to check if auto attack toggle is checked- so that player "dwell" also triggers "charachters attack button again in UI, and continues loop.
	}
	
}
