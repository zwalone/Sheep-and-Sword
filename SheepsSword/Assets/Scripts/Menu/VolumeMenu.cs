using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void SetValume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
}
