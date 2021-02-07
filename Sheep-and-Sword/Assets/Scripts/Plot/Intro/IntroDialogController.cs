using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroDialogController : MonoBehaviour
{
    // displaying text:
    public Text textDisplay;
    public string[] sentences;
    private int index = 0;
    public float typingSpeed;
    private Coroutine typing;

    // general:
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
        textDisplay.text = "";
        isDisplayed = true;
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
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            if(sentences[index] == "")
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
            skipButton.SetActive(false);
            gameObject.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;
            gameObject.GetComponent<AudioSource>().Stop();
            GameObject.Find("IntroMaster").GetComponent<IntroController>().EndScene();
        }
    }
}
