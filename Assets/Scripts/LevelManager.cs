using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public float autoLoadNextLevelAfter;

	public int zombiesIntoCombat;
	
	void Start () {
		if(autoLoadNextLevelAfter <= 0) {
			//Debug.Log ("Level auto load disabled, use a positive number");
		}else{
			Invoke ("LoadNextLevel", autoLoadNextLevelAfter);
		}
	}
	//void DontDestroyOnLoad (){}
	
	public void LoadLevel(string name) {
		//Debug.Log("Level load requested for: " +name);
		SceneManager.LoadScene (name);
	}
	public void QuitRequest() {
		//Debug.Log("quit button pressed");
		Application.Quit();
	}
	public void LoadNextLevel () {
		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene (currentScene.buildIndex + 1);
	}

	public void LoadCombatLevel (int zombies) {
		SceneManager.LoadScene ("02b Combat Level");
	}
}
