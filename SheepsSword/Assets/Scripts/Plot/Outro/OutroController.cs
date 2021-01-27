using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroController : MonoBehaviour
{
    // Event:
    private GameObject dialog;
    private GameObject mainCamera;
    private GameObject credits;
    [SerializeField]
    private GameObject UI;
    private AudioSource[] sounds;

    private void Awake()
    {
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
        dialog = GameObject.Find("Dialog").gameObject;
        mainCamera = GameObject.Find("Main Camera");
        credits = GameObject.Find("UI").transform.Find("CreditBoard").gameObject;
        dialog.GetComponent<OutroDialogController>().StartDialog();
    }

    public void EndScene()
    {
        sounds[0].Stop();
        sounds[1].Play();
        Invoke(nameof(EndScene2), 1.0f);
    }
    private void EndScene2()
    {
        sounds[2].Play();
        StartCoroutine(mainCamera.GetComponent<CameraTrackController>().LightsOff());
        StartCoroutine(VolumeUp());
        Invoke(nameof(ShowCredits), 3.0f);
        Invoke(nameof(StartVolumeDown), 10.0f);
        Invoke(nameof(ReturnToMenu), 15.0f);
    }
    private void ShowCredits() 
    { 
        credits.SetActive(true);
        UI.GetComponent<Animator>().Play("CreditsAnimation");
        Debug.Log("PLAYING elo");
    }
    private void StartVolumeDown() { StartCoroutine(VolumeDown());  }

    private IEnumerator VolumeUp()
    {
        AudioSource music = sounds[2];
        while (music.volume < 0.15f)
        {
            music.volume += 0.01f;
            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator VolumeDown()
    {
        AudioSource music = sounds[2];
        while (music.volume > 0)
        {
            music.volume -= 0.02f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SheepSound() { GameObject.Find("Music").GetComponents<AudioSource>()[1].Play(); }
    private void ReturnToMenu() { SceneManager.LoadScene(0); }
}
