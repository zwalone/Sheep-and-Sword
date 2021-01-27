using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroController : MonoBehaviour
{
    // Event:
    private GameObject dialog;
    private GameObject mainCamera;
    private AudioSource[] sounds;

    private void Awake()
    {
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
        dialog = GameObject.Find("Dialog").gameObject;
        mainCamera = GameObject.Find("Main Camera");
        dialog.GetComponent<OutroDialogController>().StartDialog();
    }

    public void EndScene()
    {
        sounds[0].Stop();
        sounds[1].Play();
        Invoke(nameof(Credits), 1.0f);
    }
    private void Credits()
    {
        sounds[2].Play();
        StartCoroutine(mainCamera.GetComponent<CameraTrackController>().LightsOff());
        StartCoroutine(VolumeUp());

        // credits here

        Invoke(nameof(ReturnToMenu), 1.0f);
    }

    private IEnumerator VolumeUp()
    {
        AudioSource music = sounds[2];
        while (music.volume < 0.15f)
        {
            music.volume += 0.01f;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void SheepSound() { GameObject.Find("Music").GetComponents<AudioSource>()[1].Play(); }
    private void ReturnToMenu() { SceneManager.LoadScene(0); }
}
