using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroDialogController : MonoBehaviour
{
    // Displaying text:
    public Text textDisplay;
    public string[] sentences;
    private int index = 0;
    public float typingSpeed;
    private Coroutine typing;

    // General:
    private bool isDisplayed = false;

    // UI:
    private GameObject skipButton;

    private void Start()
    {
        skipButton = GameObject.Find("MobileControls").transform.Find("SkipButton").gameObject;
        skipButton.GetComponent<Button>().onClick.AddListener(() => Skip());
    }

    private void Skip() { if (Time.timeScale == 1 && isDisplayed) NextSentence(); }

    public void StartDialog()
    {
        // Enable dialog element:
        textDisplay.text = "";
        isDisplayed = true;

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
            // Hide and reset the text and the button:
            skipButton.SetActive(false);
            gameObject.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;

            // Stop making the sound (if is still hearable):
            gameObject.GetComponent<AudioSource>().Stop();

            // End the scene:
            GameObject.Find("IntroMaster").GetComponent<IntroController>().EndScene();
        }
    }
}
