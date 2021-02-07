using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LastFightDialogShowController : MonoBehaviour
{
    // Text:
    public Text textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;

    // Event:
    private GameObject dialog;
    private bool isDisplayed = false;
    private PlayerController playerInfo;
    private Coroutine typing;
    private AudioSource[] sounds;

    // UI:
    private Button skipButton;

    private void Awake()
    {
        dialog = GameObject.Find("UI").transform.Find("Dialog").gameObject;
        playerInfo = GameObject.Find("Player").GetComponent<PlayerController>();
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
    }


    private void Start()
    {
        skipButton = GameObject.Find("MobileControls").transform.Find("SkipButton").gameObject.GetComponent<Button>();
        skipButton.onClick.AddListener(() => Skip());
    }

    private void Skip() { if (Time.timeScale == 1 && isDisplayed) NextSentence(); }

    public void StartDialog()
    {
        textDisplay.text = "";
        isDisplayed = true;
        dialog.SetActive(true);
        typing = StartCoroutine(Type());
    }

    public IEnumerator Type()
    {
        gameObject.GetComponent<SoundController>().PlaySound(index);
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (index == 2)
        {
            yield return new WaitForSeconds(0.5f);
            sounds[2].Play();
            yield return new WaitForSeconds(0.5f);
            sounds[1].Play();
        }
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            if (sentences[index] == "")
            {
                textDisplay.text += " ";
                index++;
            }
            if (sentences[index - 1] != "") textDisplay.text = "";
            else textDisplay.text = sentences[index - 2] + " ";
            StopCoroutine(typing);
            typing = StartCoroutine(Type());
        }
        else
        {
            dialog.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;
            playerInfo.StopReading();
            gameObject.GetComponent<AudioSource>().Stop();
            sounds[0].Play();
        }
    }

    public void BossMusicVolumeDown() { StartCoroutine(VolumeDown()); }

    private IEnumerator VolumeDown()
    {
        AudioSource music = GameObject.Find("Music").GetComponents<AudioSource>()[0];
        while (music.volume > 0)
        {
            music.volume -= 0.01f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
