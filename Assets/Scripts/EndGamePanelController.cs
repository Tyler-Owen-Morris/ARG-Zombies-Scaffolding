using UnityEngine;
using System.Collections;
using System;

public class EndGamePanelController : MonoBehaviour {

	public GameObject[] suicideVerificationPanelArray;
	public int count =0, target;

	void Start () {
		ShuffleTheArray(suicideVerificationPanelArray);
	}

	void ShuffleTheArray (GameObject[] array) {
		for (int i = array.Length-1; i > 0; i--) {
			int pos = UnityEngine.Random.Range(0, i);
			GameObject tmp = array[i];
			array[i] = array[pos];
			array[pos] = tmp;
		}
	}

	public void BeginSuicideQuestioning () {
		count = 0;
        CalculateTargetValue();
		ContinueWithSuicide();
	}

	public void ContinueWithSuicide () {
		
		//deactivate all panels
		foreach (GameObject panel in suicideVerificationPanelArray) {
			panel.SetActive(false);
		}

		//choose a random panel to activate
		int panel_pos = UnityEngine.Random.Range(0, suicideVerificationPanelArray.Length-1);
		//suicideVerificationPanelArray[panel_pos].GetComponent<SuicideButtonShuffler>().ShuffleButtonPositions();
		suicideVerificationPanelArray[panel_pos].SetActive(true);

		//increment count up and check
		count ++;

		if (count >= target) {
			Debug.Log("player has verified suicide");
			StartCoroutine(GameManager.instance.KillUrself());
		}

		/* this old method relied on the order in heirarchy of the game objects.
		for (int i=0; i < suicideVerificationPanelArray.Length; i++) {
			if (suicideVerificationPanelArray[i].activeInHierarchy == false) {
				suicideVerificationPanelArray[i].SetActive(true);
				break;
			}
			//if we're out of list items and they're all on, call out to send the suicide
			if (i+1 >= suicideVerificationPanelArray.Length) {
				Debug.Log("Out of panels to verify- Sending suicide to GameManager");
				StartCoroutine(GameManager.instance.KillUrself());
			}
		}
		*/
	}

	public void CancelSuicide () {
		count = 0;
		foreach (GameObject panel in suicideVerificationPanelArray) {
			panel.SetActive(false);
		}
	}

    public void CalculateTargetValue()
    {
        TimeSpan time_alive = (DateTime.Now - GameManager.instance.timeCharacterStarted);
        int days_alive =  Mathf.FloorToInt((float)time_alive.TotalDays);

        if (days_alive < 3)
        {
            //it's 'harder' to kill yourself in the beginning.
            target = 18;
        }else if (days_alive < 8)
        {
            target = 15;
        }else if (days_alive < 13)
        {
            target = 11;
        }else if (days_alive < 21)
        {
            target = 8;
        }else if (days_alive < 34)
        {
            target = 6;
        }else
        {
            //after day 34, it will only require 3 correct answers to execute the suicide function.
            target = 3;
        }
    }
}
