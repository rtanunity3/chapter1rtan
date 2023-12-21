using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip bgMusic;
    public AudioClip startsound;

    void Start()
    {
        StartCoroutine("StartPlay");
    }

    IEnumerator StartPlay()
    {
        audioSource.PlayOneShot(startsound);
        yield return new WaitForSeconds(1f);

        if (!audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = bgMusic;
        audioSource.Play();
    }
}
