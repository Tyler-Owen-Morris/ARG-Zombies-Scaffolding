using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Zombie : MonoBehaviour {

	public int health, attack, maxHealth;


	[SerializeField]
	private Slider healthSlider;

	private CombatManager combatManager;

	// Use this for initialization
	void Start () {
		ResetZombie();
		maxHealth = health;
		combatManager = CombatManager.FindObjectOfType<CombatManager>();
	}

	void Update () {
		healthSlider.value = HealthToSliderValue();

	}

	public int Hit () {
		int attk = attack;
		return attk;
	}

	public void GetHit (int dmg) {
		int tmp = health - dmg;
		health = tmp;
		if (this.health <= 0) {
			combatManager.ZombieIsKilled(); // 
		}
	}

	public void ResetZombie () {
		health = 30;
		attack = 5;
	}

	private float HealthToSliderValue () {
		float currentHealth = health;
		float totalHealth = maxHealth;
		float temp = currentHealth / totalHealth;
		//Debug.Log ("HealthToSliderValue returned a value of: " + temp + " current health: " + health + " and maxHealth: " + maxHealth);
		return temp;
	}
}
