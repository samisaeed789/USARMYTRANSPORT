using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // Sound effect audio sources
    public AudioSource soundEffectSource1;
    public AudioSource soundEffectSource2;

    // Sound effect clips
    public AudioClip jumpSound;
    public AudioClip coinSound;
    public AudioClip explosionSound;

    // Volume for sound effects
    //[Range(0f, 1f)]
    //public float soundEffectVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
       
    }

    public void PlayJumpSound(float soundEffectVolume)
    {
        soundEffectSource1.clip = jumpSound;
        soundEffectSource1.volume = soundEffectVolume;
        soundEffectSource1.Play();
    }

    public void PlayCoinSound(float soundEffectVolume)
    {
        soundEffectSource1.clip = coinSound;
        soundEffectSource1.volume = soundEffectVolume;
        soundEffectSource1.Play();
    }

    public void PlayExplosionSound(float soundEffectVolume)
    {
        soundEffectSource2.clip = explosionSound;
        soundEffectSource2.volume = soundEffectVolume;
        soundEffectSource2.Play();
    }

}

