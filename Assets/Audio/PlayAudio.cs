using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GameObject.FindGameObjectWithTag("AudioSource").GetComponent<AudioSource>();
        }

    }

    public void playClip()
    {
        audioSource.clip = audioClip;
        audioSource.PlayOneShot(audioClip);
    }
}
