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

    private void Awake()
    {
        dialog = GameObject.Find("UI").transform.Find("Dialog").gameObject;
        playerInfo = GameObject.Find("Player").GetComponent<PlayerController>();
        sounds = GameObject.Find("Music").GetComponents<AudioSource>();
    }

    private void Update()
    {
        // Don't show new letters / make a sound if in pause-menu:
        if (Time.timeScale != 1) return;

        // Wait for player's input and go to next sentence:
        if (isDisplayed)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                NextSentence();
    }

    public void StartDialog()
    {
        // Enable dialog element:
        textDisplay.text = "";
        isDisplayed = true;
        dialog.SetActive(true);

        // Start showing letters:
        typing = StartCoroutine(Type());
    }

    public IEnumerator Type()
    {
        // Speak up:
        gameObject.GetComponent<SoundController>().PlaySound(index);

        // Show letters one after another in certain gaps of time:
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Make a "sheep sound" and "evil laugh sound" after third sentence:
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

            // If sentence is empty string, go to next sentence:
            if (sentences[index] == "") index++;

            // If previous wasn't empty string, reset the text,
            // Otherwise add space sign:
            if (sentences[index - 1] != "") textDisplay.text = "";
            else textDisplay.text = sentences[index - 2] + " ";

            // Stop typing current sentence:
            StopCoroutine(typing);

            // Start typing next sentence:
            typing = StartCoroutine(Type());
        }
        else
        {
            // Hide and reset the text:
            gameObject.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;

            // Update player's state (he can be attacked now):
            playerInfo.StopReading();

            // Stop making the sound (if is still hearable):
            gameObject.GetComponent<AudioSource>().Stop();

            // Enable boss fight music:
            sounds[0].Play();
        }
    }

    public void BossMusicVolumeDown() { StartCoroutine(VolumeDown()); }

    // Make the boss fight music quieter and quieter:
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
