using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

	private Animator animator;
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Attack () {
		animator.SetTrigger("CharactersAttack");
		
	}
}
