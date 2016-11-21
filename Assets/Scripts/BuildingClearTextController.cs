using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingClearTextController : MonoBehaviour {

    private Animator animator;
    public Text stamText;
    public AudioClip[] bldg_clear_audioclip;
    public AudioSource myAudioSource;

    // Use this for initialization
    void Start()
    {
        int clip_pos = UnityEngine.Random.Range(0, bldg_clear_audioclip.Length - 1);
        myAudioSource = GetComponent<AudioSource>();
        myAudioSource.playOnAwake = false;
        myAudioSource.PlayOneShot(bldg_clear_audioclip[clip_pos]);
        animator = GetComponent<Animator>();
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Debug.Log(clipInfo[0].clip.length);
        Destroy(this.gameObject, clipInfo[0].clip.length);
    }

    
}
