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
		animator.SetTrigger("CharactersAttack");
		//StartCoroutine(AfterPlayerAttack());
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

	public void CheckBeforeContinuing () { // this will be called from animation
		if (GameManager.instance.totalSurvivors <= 0) {
			combatManager.EndGameCalled();
		} 
	}
	
}
