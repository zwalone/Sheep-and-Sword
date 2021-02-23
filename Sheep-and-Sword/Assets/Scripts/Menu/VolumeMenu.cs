using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider music;
    public Slider dialog;
    public Slider effect;

    // Get and save starting values of all of the sliders:
    private void Awake()
    {
        float val;

        if (audioMixer.GetFloat("Effect", out val))
            effect.value = Mathf.Pow(10, val / 20);

        if (audioMixer.GetFloat("Dialog", out val))
            dialog.value = Mathf.Pow(10, val / 20);

        if (audioMixer.GetFloat("Music", out val))
            music.value = Mathf.Pow(10, val / 20 );
    }

    // Set new value for music's slider:
    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    // Set new value for dialogs' slider:
    public void SetDialog(float volume)
    {
        audioMixer.SetFloat("Dialog", Mathf.Log10(volume) * 20);
    }

    // Set new value for sound effects' slider:
    public void SetEffect(float volume)
    {
        audioMixer.SetFloat("Effect", Mathf.Log10(volume) * 20);
    }
}
