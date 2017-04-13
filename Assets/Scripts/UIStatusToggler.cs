using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIStatusToggler : MonoBehaviour {

    public GameObject leftStatusPanel;
    public Text buttonText;

    public void ToggleUIstatus()
    {
        if (leftStatusPanel.activeInHierarchy)
        {
            leftStatusPanel.SetActive(false);
            buttonText.text = "show status";
        }else
        {
            leftStatusPanel.SetActive(true);
            buttonText.text = "hide status";
        }
    }
}
