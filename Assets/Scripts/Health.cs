using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float health = 100f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}
	public void DealDamage (float dmg) {
		health -= dmg;
		if (health <= 0) {
			//optionally trigger animation
			DestroyObject ();
		}
	}
	
	public void DestroyObject () {
		Destroy(gameObject);
	}
}
