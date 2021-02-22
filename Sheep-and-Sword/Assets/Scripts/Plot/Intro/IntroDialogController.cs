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
            // Hide and reset the text:
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
