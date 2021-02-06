using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider music;
    public Slider dialog;
    public Slider effect;
    private void Awake()
    {
        float val;

        if (audioMixer.GetFloat("Effect", out val))
            effect.value = Mathf.Pow(10, val/20);

        if (audioMixer.GetFloat("Dialog", out val))
            dialog.value = Mathf.Pow(10, val/20);

        if (audioMixer.GetFloat("Music", out val))
            music.value = Mathf.Pow(10, val/20 );
    }

    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume)*20);
    }

    public void SetDialog(float volume)
    {
        audioMixer.SetFloat("Dialog", Mathf.Log10(volume)*20);
    }

    public void SetEffect(float volume)
    {
        audioMixer.SetFloat("Effect", Mathf.Log10(volume)*20);
    }
}
