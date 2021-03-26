using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager sharedInstance  { get; set; }

    public AudioSource SOUND_SRC;

    public AudioClip    clear_clip;
    public AudioClip    slide_clip;

    public bool updatingSound = false;

    private void Awake()
    {
        sharedInstance = this;
    }

    public void OnClearCookies()
    {
        if (!updatingSound)
        {
            SOUND_SRC.PlayOneShot(clear_clip);
            updatingSound = true;
        }
    }

    public void OnSlide()
    {
        SOUND_SRC.PlayOneShot(slide_clip);
    }


    public void OnFinishUpdate()
    {
        updatingSound = false;
    }
}
