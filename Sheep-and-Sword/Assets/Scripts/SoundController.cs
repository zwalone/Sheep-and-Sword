using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    AudioSource audioSource;
    public List<AudioClip> audioClip;

    private void Awake()
    {
        audioSource = gameObject.GetComponents<AudioSource>()[0];
    }

    public void PlaySound(int indexOfAudioClip)
    {
        // Make sure that you remember which audioSource you need to use:
        if (audioSource == null) 
            audioSource = gameObject.GetComponents<AudioSource>()[0];

        // Stop previous sound (if is still hearable):
        audioSource.Stop();

        // Play a sound from audioClip list that is on given index:
        audioSource.clip = audioClip[indexOfAudioClip];
        audioSource.Play();
    }
}
