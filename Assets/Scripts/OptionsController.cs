using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour {
	
    //*******NOTE TO SELF***** OPTIONS CONTROLLER IS INDEX 7 IN THE BUILD LIST: THIS IS TO NOT DISTURB OTHER "on level loaded" level index checks
    //this SHOULD be the 3rd scene in the build index, but it's the last, and that's the way it's gonna be.

	public Slider musicVolumeSlider, sfxVolumeSlider;
    public Toggle allowPublicPicToggle;
	//public Slider difficultySlider;
	public LevelManager levelManager;
	
	private MusicManager musicManager;
	
	// Use this for initialization
	void Start () {
		musicManager = GameObject.FindObjectOfType<MusicManager>();
		musicVolumeSlider.value = GamePreferences.GetMusicVolume();
        sfxVolumeSlider.value = GamePreferences.GetSFXVolume();
        int pic_pref = GamePreferences.GetPermitPublicFBPic();
        if (pic_pref == 1)
        {
            allowPublicPicToggle.isOn=true;
        }else
        {
            allowPublicPicToggle.isOn = false;
        }
		//difficultySlider.value = PlayerPrefsManager.GetDiffficulty();
		
	}
	
	// Update is called once per frame
	void Update () {
		musicManager.ChangeVolume (musicVolumeSlider.value);
		
	}
	
	public void SaveAndExit () {
        //PlayerPrefsManager.SetMasterVolume (musicVolumeSlider.value);
        //PlayerPrefsManager.SetDifficulty (difficultySlider.value);
        GamePreferences.SetMusicVolume(musicVolumeSlider.value);
        GamePreferences.SetSFXVolume(sfxVolumeSlider.value);
        if (allowPublicPicToggle.isOn)
        {
            GamePreferences.SetPermitPublicFBPic(1);
        }else
        {
            GamePreferences.SetPermitPublicFBPic(0);
        }
		levelManager.LoadLevel ("01a Login");
	}
	
	public void SetDefaults () {
		musicVolumeSlider.value = 0.8f;
		//difficultySlider.value = 2f;
	}
}
