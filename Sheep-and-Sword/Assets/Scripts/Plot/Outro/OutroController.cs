using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroController : MonoBehaviour
{
    // Event:
    private OutroDialogController dialog;
    private GameObject mainCamera;
    private GameObject credits;
    private GameObject UI;
    private AudioSource[] sounds;

    private void Awake()
    {
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
        mainCamera = GameObject.Find("Main Camera");
        UI = GameObject.Find("UI");
        dialog = UI.transform.Find("Dialog").GetComponent<OutroDialogController>();
        credits = GameObject.Find("UI").transform.Find("CreditBoard").gameObject;

        // Don't stop the music if in pause-menu:
        sounds[0].ignoreListenerPause = true;
        sounds[2].ignoreListenerPause = true;
    }

    // Start the dialog:
    private void Start() { dialog.StartDialog(); }

    public void EndScene()
    {
        // Stop the heartbeat:
        sounds[0].Stop();

        // Make a sheep sound:
        sounds[1].Play();

        // Wait a second:
        Invoke(nameof(EndScene2), 1.0f);
    }

    private void EndScene2()
    {
        // Start ending music:
        sounds[2].Play();

        // Turn the lights off:
        StartCoroutine(mainCamera.GetComponent<CameraTrackController>().LightsOff());

        // Volume the ending music up:
        StartCoroutine(VolumeUp());

        // Show credits:
        Invoke(nameof(ShowCredits), 2.0f);

        // Volume the ending music down:
        Invoke(nameof(StartVolumeDown), 10.0f);

        // Go back to the main menu:
        Invoke(nameof(ReturnToMenu), 18.0f);
    }

    private void ShowCredits() 
    { 
        // Enable credits and show animation:
        credits.SetActive(true);
        UI.GetComponent<Animator>().Play("CreditsAnimation");
    }

    private void StartVolumeDown() { StartCoroutine(VolumeDown());  }

    // Make the ending music louder and louder:
    private IEnumerator VolumeUp()
    {
        AudioSource music = sounds[2];
        while (music.volume < 0.15f)
        {
            music.volume += 0.01f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Make the ending music quieter and quieter:
    private IEnumerator VolumeDown()
    {
        AudioSource music = sounds[2];
        while (music.volume > 0)
        {
            music.volume -= 0.01f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Load main menu scene and destroy GM:
    private void ReturnToMenu() { SceneManager.LoadScene(0); Destroy(GameObject.Find("GameMaster")); }
}
