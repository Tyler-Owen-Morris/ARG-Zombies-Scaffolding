using UnityEngine;
using System.Collections;

public class HeartbeatSoundManager : MonoBehaviour {

    public AudioSource myAudioSource;
    public AudioClip[] myHeartbeatSounds;


    void Start ()
    {
        myAudioSource.volume = GamePreferences.GetSFXVolume();
    }


    //this is intended to be called from maplevel manager after calculating the players current stamina slider.
    public void SetMyHeartbeatSound (float value)
    {
        if (value > 0)
        {
            myAudioSource.clip = myHeartbeatSounds[0];//slow
        }else if (value > -1)
        {
            myAudioSource.clip = myHeartbeatSounds[1];//faster
        }else
        {
            myAudioSource.clip = myHeartbeatSounds[2];//fastest
        }
    }


}
