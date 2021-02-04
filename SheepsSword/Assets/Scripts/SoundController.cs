using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> audioClip;

    void Awake()
    {
        audioSource = GetComponents<AudioSource>()[0];
    }

    public void PlaySound(int indexOfAudioClip)
    {
        audioSource.Stop();
        audioSource.clip = audioClip[indexOfAudioClip];
        audioSource.Play();
    }
}
