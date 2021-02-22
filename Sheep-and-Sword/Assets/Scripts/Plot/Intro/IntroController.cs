using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    // Event:
    private GameObject dialog;
    private GameObject mainCamera;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        dialog = GameObject.Find("Dialog").gameObject;

        // Start showing sentences:
        dialog.GetComponent<IntroDialogController>().StartDialog();

        // Don't stop the main music if in pause-menu:
        GameObject.Find("Music").GetComponents<AudioSource>()[0].ignoreListenerPause = true;
    }

    // Instructions for the end of the intro:
    public void EndScene()
    {
        // Stop turning the lights on, start turning them off:
        StopCoroutine(mainCamera.GetComponent<CameraTrackController>().lightsOn);
        StartCoroutine(mainCamera.GetComponent<CameraTrackController>().LightsOff());

        // Decrease the volume of main music:
        StartCoroutine(VolumeDown());

        // Make a sheep sound three times:
        Invoke(nameof(SheepSound), 6.0f);
        Invoke(nameof(SheepSound), 8.0f);
        Invoke(nameof(SheepSound), 10.0f);

        // Go to Level 1:
        Invoke(nameof(NewLevel), 12.0f);
    }

    // Make the main music less and less hearbale:
    private IEnumerator VolumeDown()
    {
        AudioSource music = GameObject.Find("Music").GetComponents<AudioSource>()[0];
        while (music.volume > 0)
        {
            music.volume -= 0.01f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Make a sheep sound:
    private void SheepSound()
    {
        GameObject.Find("Music").GetComponents<AudioSource>()[1].Play();
    }

    // Load new scene:
    private void NewLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
