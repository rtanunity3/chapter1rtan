using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip bgMusic;

    void Start()
    {
        audioSource.clip = bgMusic;
        audioSource.Play();
    }
}
