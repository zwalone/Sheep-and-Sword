using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroDialogController : MonoBehaviour
{
    public Text textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    private Coroutine typing;
    private bool isDisplayed = false;

    private void Update()
    {
        if (isDisplayed)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                NextSentence();
    }

    public void StartDialog()
    {
        textDisplay.text = "";
        isDisplayed = true;
        typing = StartCoroutine(Type());
    }

    public IEnumerator Type()
    {
        gameObject.GetComponent<AudioSource>().Play();
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        gameObject.GetComponent<AudioSource>().Stop();
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
            if(sentences[index - 1] != "") textDisplay.text = "";
            StopCoroutine(typing);
            typing = StartCoroutine(Type());
        }
        else
        {
            gameObject.SetActive(false);
            textDisplay.text = "";
            isDisplayed = false;
            GameObject.Find("GameMaster").GetComponent<IntroController>().EndScene();
        }
    }
}
